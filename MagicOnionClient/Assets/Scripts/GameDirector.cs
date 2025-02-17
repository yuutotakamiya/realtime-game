//==========================================================
//
//インゲームを管理するスクリプト
//Author:高宮祐翔
//
//==========================================================
using Cinemachine;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using JetBrains.Annotations;
using MagicOnionServer.Model.Entity;
using Shared.Interfaces.StreamingHubs;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.TextCore.Text;
using UnityEngine.UI;
using static Shared.Interfaces.StreamingHubs.IRoomHubReceiver;

/// <summary>
/// ゲーム全体を管理しているスクリプト
/// </summary>
public class GameDirector : MonoBehaviour
{
    //キャラクター関係
    [SerializeField] GameObject[] characterPrefab;//キャラクターのPrefab
    [SerializeField] GameObject[] startposition;//最初のスタートポジション

    //クラスの設定
    [SerializeField] RoomHubModel roomHubModel;//RoomHubModelのクラスの設定
    [SerializeField] DefenceTarget defenceTarget;//DefenceTargetクラスの設定

    //カウントダウン
    [SerializeField] float timeLimit;//制限時間を設定
    [SerializeField] float currentTime;//現在のタイム
    [SerializeField] int countdownTime;//ゲームが始まる前のカウントダウン設定

    //UI
    [SerializeField] Text timerText;//タイマーText
    [SerializeField] Text countdownText;//カウントダウンText
    [SerializeField] Text Crrenttext;//現在のキル数
    [SerializeField] Text KillNum;//キル数
    [SerializeField] Text KillLog;//キル通知
    [SerializeField] Text killerKakeru;//×Text;
    [SerializeField] Text humanKakeru;//×Text;
    [SerializeField] Text ChestNumText;//宝箱の取得した数を入れるText
    [SerializeField] GameObject GameFinish;//ゲーム終了Text
    [SerializeField] GameObject GameStartText;//ゲームスタートText
    [SerializeField] GameObject Result;//リザルト画面に行くためのボタン
    [SerializeField] GameObject AttackButton1;//デフォルトの攻撃ボタン
    [SerializeField] GameObject WinText;//
    [SerializeField] GameObject WinText2;//
    [SerializeField] Image skullIamge;//頭蓋骨の画像
    [SerializeField] Image MiniMap;//ミニマップ
    [SerializeField] Image ChestImage;//宝箱の画像
    

    //virtualCameraカメラの宣言
    private CinemachineVirtualCamera virtualCamera; // Cinemachine Virtual Camera

    //フラグ関係
    private bool isEnemy = false;//自分が敵かどうか
    private bool ishave = false;//宝箱を持っているかどうか

    
    Vector3 position;

    //コンポーネントの宣言
    Animator animator;
    Rigidbody rigidbody;

    //キャラクターの情報を保存するためのフィールド
    private Dictionary<Guid, GameObject> characterList = new Dictionary<Guid, GameObject>();

    //名前と宝箱の名前をフィールドに保存
    public static Dictionary<string,int> keyValuePairs;

    //自分自身の名前をフィールドに保存
    private JoinedUser MyName; 

    //自分自身が敵かどうかを判断するフラグをプロパティ化
    public bool IsEnemy
    {
        get { return isEnemy; }
        set { isEnemy = value; }
    }

    /// <summary>
    /// 開始処理
    /// </summary>
    public async void Start()
    {
        //接続
        await roomHubModel.ConnectionAsync();

        //ユーザーが入室時にOnJoinedUserメソッドを実行するよう、モデルに登録
        roomHubModel.OnJoinedUser += this.OnJoinedUser;

        //ユーザーが退出時にOnLeaveメソッドを実行するよう、モデルに登録
        roomHubModel.OnExitUser += this.OnExitUser;

        //ユーザーが移動したときにOnMoveCharacterメソッドを実行するよう、モデルに登録
        roomHubModel.OnMoveCharacter += this.OnMoveCharacter;

        //ルーム内にいるユーザーが全員準備完了したらOnReadyメソッドを実行するよう、モデルに登録
        roomHubModel.OnReadyUser += this.OnReady;

        //ルーム内にいるユーザーが準備完了して、ゲームが開始されたらOnTimeメソッドを実行するよう、モデルに登録
        roomHubModel.OnTime += this.OnTimer;

        //ルーム内にいるユーザーが鬼にキルされたときにOnKillメソッドを実行するよう、モデルに登録
        roomHubModel.OnKillNum += this.OnKill;

        //宝箱が移動したときにOnMoveChestメソッドを実行するよう、モデルに登録
        roomHubModel.OnChest += this.OnMoveChest;

        //宝箱を全て取得したときにOnGainChestメソッドを実行するよう、モデルに登録
        roomHubModel.OnChestN += this.OnChestNum;

        //ゲームが終了したときにOnEndGameメソッドを実行するよう、モデルに登録
        roomHubModel.OnEndG += this.OnEndGame;

        currentTime = timeLimit; // 初期化: 残り時間を設定

        animator = GetComponent<Animator>();//Animatorコンポーネントの取得
        rigidbody = GetComponent<Rigidbody>();//Rigidbodyコンポーネントの取得
        virtualCamera = FindObjectOfType<CinemachineVirtualCamera>();//CinemachineVirtualCameraオブジェクトの取得

        KillNum.text = "0";

        await JoinRoom();
        await UniTask.Delay(TimeSpan.FromSeconds(4.0f));  // 非同期で4秒待機
        await Ready();
    }

    /// <summary>
    /// 次のシーンに行ったときに登録したものを解除する関数
    /// </summary>
    private void OnDestroy()
    {
        //OnJoinedUser通知の登録解除
        roomHubModel.OnJoinedUser -= this.OnJoinedUser;

        //OnLeave通知の登録解除
        roomHubModel.OnExitUser -= this.OnExitUser;

        //OnMoveCharacter通知の登録解除
        roomHubModel.OnMoveCharacter -= this.OnMoveCharacter;

        //OnReady通知の登録解除
        roomHubModel.OnReadyUser -= this.OnReady;

        //OnTime通知を登録解除
        roomHubModel.OnTime -= this.OnTimer;

        //OnKill通知の登録解除
        roomHubModel.OnKillNum -= this.OnKill;

        //OnMoveChestの通知登録解除
        roomHubModel.OnChest -= this.OnMoveChest;

        //OnGainChest通知の登録解除
        roomHubModel.OnChestN -= this.OnChestNum;

        //OnEndGame通知の登録解除
        roomHubModel.OnEndG -= this.OnEndGame;
    }

    /// <summary>
    /// 入室する時に呼び出す関数
    /// </summary>
    /// <returns></returns>
    public async UniTask JoinRoom()
    {
        //入室
        await roomHubModel.JoinAsync(LobbyManager.RoomName, UserModel.Instance.UserID);

        InvokeRepeating("Move", 0.1f, 0.1f);
    }

    /// <summary>
    /// ユーザーが入室した時の処理
    /// </summary>
    /// <param name="user"></param>
    private void OnJoinedUser(JoinedUser user)
    {
        //キャラクターを生成
        GameObject characterObject = Instantiate(characterPrefab[user.JoinOrder],
            startposition[user.JoinOrder].transform.position,
            startposition[user.JoinOrder].transform.rotation);

        //自分自身の接続IDが同じだったら
        if (roomHubModel.ConnectionId == user.ConnectionId)
        {
            MyName=user;
            characterObject.GetComponent<Character>().Name(user.UserData.Name);
        }

       //roomHubModelの接続IDと自分自身の接続IDが同じだったら
        if (roomHubModel.ConnectionId == user.ConnectionId)
        {
            //JoinOrderが0だったら
            if (user.JoinOrder == 0)
            {
                isEnemy = true;
                AttackButton1.SetActive(true);
                KillNum.gameObject.SetActive(true);
                skullIamge.gameObject.SetActive(true);
                MiniMap.gameObject.SetActive(true);
                killerKakeru.gameObject.SetActive(true);
            }
            else
            {
                ChestNumText.gameObject .SetActive(true);
                ChestImage.gameObject .SetActive(true);
                humanKakeru.gameObject.SetActive (true);
            }

            // 生成されたキャラクターをCinemachineのFollowとLook Atターゲットに設定
            Transform characterTransform = characterObject.transform;
            virtualCamera.Follow = characterTransform;//キャラクターにカメラをフォロー
            virtualCamera.LookAt = characterTransform;//キャラクターにカメラをロック
        }

        //自分自身の接続IDとroomHubModelの接続が一緒だったら
        if (roomHubModel.ConnectionId == user.ConnectionId)
        {
            characterObject.GetComponent<Character>().Isself = true;
        }

        characterObject.transform.position = startposition[user.JoinOrder].transform.position;
        characterList[user.ConnectionId] = characterObject;//フィールドで保持
    }

    /// <summary>
    /// 退室するときに呼び出す関数
    /// </summary>
    public async void ExitRoom()
    {
        await roomHubModel.LeaveAsync();

        // 全てのキャラクターオブジェクトを削除
        foreach (var entry in characterList)
        {
            Destroy(entry.Value);  // キャラクターオブジェクトを破棄
            CancelInvoke("Move");
        }

        // characterListをクリア
        characterList.Clear();

        // 自分のConnectionIdをリセット
        roomHubModel.ConnectionId = Guid.Empty;
    }

    /// <summary>
    /// ユーザーが退室したときの処理
    /// </summary>
    /// <param name="user"></param>
    private void OnExitUser(JoinedUser user)
    {
        // 退室したユーザーのキャラクターオブジェクトを削除
        if (characterList.ContainsKey(user.ConnectionId))
        {
            Destroy(characterList[user.ConnectionId]);  // オブジェクトを破棄
            characterList.Remove(user.ConnectionId);    // リストから削除
        }
    }

    /// <summary>
    /// キャラクターの位置を定期的に呼び出すメソッド
    /// </summary>
    public async void Move()
    {
        //自分自身のtransform.position、Quaternion.identity,アニメーションをサーバーに送信
        await roomHubModel.MoveAsync(characterList[roomHubModel.ConnectionId].gameObject.transform.position,
            characterList[roomHubModel.ConnectionId].gameObject.transform.rotation,
           (CharacterState)characterList[roomHubModel.ConnectionId].GetComponent<Animator>().GetInteger("state"));
    }

    /// <summary>
    /// ユーザーの移動、回転、アニメーション
    /// </summary>
    /// <param name="connectionId"></param>
    /// <param name="pos"></param>
    /// <param name="rotaition"></param>
    /// <param name="characterState"></param>
    private void OnMoveCharacter(Guid connectionId, Vector3 pos, Quaternion rotaition, CharacterState characterState)
    {
        if (characterList.ContainsKey(connectionId))
        {
            GameObject character = characterList[connectionId];

            // キャラクターの位置と回転をサーバーに更新
            character.transform.DOLocalMove(pos, 0.1f).SetEase(Ease.Linear);
            character.transform.DORotate(rotaition.eulerAngles, 0.1f).SetEase(Ease.Linear);

            //キャラクターのアニメーション
            Animator animator = character.GetComponent<Animator>();

            animator.SetInteger("state", (int)characterState);
        }
    }

    /// <summary>
    /// ユーザーが準備完了を押した時のメソッド
    /// </summary>
    /// <returns></returns>
    public async UniTask Ready()
    {
        await roomHubModel.ReadyAsync();
    }

    /// <summary>
    /// ルーム内のユーザー全員が準備完了していたらの処理
    /// </summary>
    /// <param name="connectionId"></param>
    /// <param name="isReady"></param>
    private void OnReady(Guid connectionId, bool isReady)
    {
        isReady = true;
        StartCoroutine(StartCountdown());
        StartCoroutine("Text");
    }

    /// <summary>
    /// ゲーム内制限時間
    /// </summary>
    /// <param name="time"></param>
    public async void Time(float time)
    {
        await roomHubModel.TimeAsync(time);
    }

    /// <summary>
    /// 定期的に呼ぶメソッド(ゲーム内制限時間)
    /// </summary>
    /// <param name="user"></param>
    /// <param name="time"></param>
    private async void OnTimer(JoinedUser user, float time)
    {
        currentTime = time;
        await Ready();
    }

    /// <summary>
    /// キルしたときのメソッド
    /// </summary>
    public async UniTask KillAsync()
    {
        await roomHubModel.KillAsync();
    }

    /// <summary>
    /// キルしたときの通知
    /// </summary>
    /// <param name="connectionId"></param>
    /// <param name="TotalKillNum"></param>
    /// <param name="userName"></param>
    public void OnKill(Guid connectionId, int TotalKillNum,string userName)
    {
        KillNum.text = TotalKillNum.ToString();//キルした数

        AnimateKillLog(userName);//キルログ
    }

    /// <summary>
    /// 宝箱の位置同期
    /// </summary>
    /// <param name="pos"></param>
    /// <param name="rotaition"></param>
    /// <param name="Namechest"></param>
    public async UniTask MoveChest(Vector3 pos, Quaternion rotaition, string Namechest)
    {
        await roomHubModel.MoveChest(pos, rotaition, Namechest);
    }

    /// <summary>
    /// 宝箱の位置を定期的に通知するメソッド
    /// </summary>
    /// <param name="pos"></param>
    /// <param name="rotaition"></param>
    /// <param name="Namechest"></param>
    public void OnMoveChest(Vector3 pos, Quaternion rotaition, string Namechest)
    {
        GameObject chest = GameObject.Find(Namechest);//宝箱の名前を検索

        chest.transform.rotation = rotaition;//宝箱の回転を代入
        chest.transform.position = pos;//宝箱の位置を代入
    }

    /// <summary>
    /// 宝箱の取得数同期
    /// </summary>
    /// <returns></returns>
    public async UniTask GainChest()
    {
        await roomHubModel.GainChest();//宝箱取得数を非同期で呼び出し
    }

    /// <summary>
    /// 宝箱の取得数通知
    /// </summary>
    /// <param name="TotalChestNum"></param>
    /// <param name="keyValuePairs"></param>
    public async void OnChestNum(int TotalChestNum,Dictionary<string,int> keyValuePairs)
    {
        //自分自身の場合
        if (keyValuePairs.ContainsKey(MyName.UserData.Name))
        {
            ChestNumText.text = keyValuePairs[MyName.UserData.Name].ToString();//宝箱の数をTextに代入
        }

        //宝箱を合計2個取得したら
        if (TotalChestNum == 2)
        {
           await EndGameAsync(true);//ゲーム終了同期を非同期で呼び出し
        }
    }

    /// <summary>
    /// ゲーム終了同期
    /// </summary>
    /// <returns></returns>
    public async UniTask EndGameAsync(bool isEndGame)
    {
        await roomHubModel.EndGameAsync(isEndGame);
    }

    /// <summary>
    /// ゲーム終了通知
    /// </summary>
    /// <param name="isEndGame"></param>
    public async  void OnEndGame(bool isEndGame, List<ResultData> resultData)
    {
        ResultManager.SetResult(resultData);

        //人間側が勝利した場合
        if (isEndGame == true)
        {
            characterList[roomHubModel.ConnectionId].GetComponent<Character>().Isstart = false;
            WinText.SetActive(true);
            WinText2.SetActive(true);
            StopCoroutine(CountdownTimer());
            Invoke("LoadResult", 3);
        }
        //追いかける側が勝利した場合
        else
        {
            characterList[roomHubModel.ConnectionId].GetComponent<Character>().Isstart = false;
            timerText.text = "0"; // 0秒になったら表示
            GameFinish.SetActive(true);
            Invoke("LoadResult", 3);
        }
    }

    /// <summary>
    /// リザルトシーンへ遷移する関数
    /// </summary>
    public void LoadResult()
    {
        Initiate.Fade("Result", Color.black, 1);//Resultシーンへ遷移
    }

    /// <summary>
    /// DoTweenを使ったキルログアニメーション
    /// </summary>
    /// <param name="userName"></param>
    private void AnimateKillLog(string userName)
    {
        // KillLog テキストの初期位置を保存
        Vector3 initialPosition = KillLog.transform.localPosition;

        // 名前を色付きで表示（例: 名前を赤色で）
        string KillMessage = "<color=red>" + userName + "</color>" + "が殺されました";

        // KillLogにメッセージを追加
        KillLog.text += KillMessage + "\n";

        // KillLog 全体のフェードインアニメーション
        KillLog.DOFade(1f, 0.5f)  // フェードイン
            .SetEase(Ease.OutQuad)  // イージングを設定
            .OnComplete(() =>
            {
                // フェードイン後、少し待機してから KillLog 全体をフェードアウト
                KillLog.DOFade(0f, 1f)  // フェードアウト
                    .SetDelay(2f)  // 2秒後にフェードアウト開始
                    .SetEase(Ease.InQuad)
                    .OnComplete(() =>
                    {
                        // フェードアウト後にKillLogをリセット
                        KillLog.text = ""; // KillLogのテキストを消す
                    });
            });

        // KillLog テキストを上にスライドさせるアニメーション
        KillLog.transform.DOLocalMoveY(initialPosition.y + 50f, 0.5f)
            .SetEase(Ease.OutQuad)
            .OnComplete(() =>
            {
                // スライドアニメーション後、元の位置に戻す
                KillLog.transform.DOLocalMoveY(initialPosition.y, 0.5f)
                    .SetEase(Ease.InQuad);
            });
    }


    /// <summary>
    /// カウントダウンを行うメソッド
    /// </summary>
    /// <returns></returns>
    private IEnumerator StartCountdown()
    {
        countdownTime = 3; //3秒のカウントダウン
        while (countdownTime > 0)
        {
            countdownText.text = countdownTime.ToString(); // カウントダウンを表示

            // 演出: 数字を拡大して、表示する
            countdownText.transform.DOScale(1.5f, 0.5f).SetEase(Ease.OutBounce); // 拡大（0.5秒で）
            countdownText.color = Color.red; // 数字を赤に変更

            yield return new WaitForSeconds(0.5f); // 0.5秒間待機（拡大表示される時間）

            // 元の状態に戻す
            countdownText.transform.DOScale(1f, 0.3f).SetEase(Ease.OutBounce); // 縮小（0.3秒で）
            countdownText.color = Color.white; // 色を白に戻す

            yield return new WaitForSeconds(0.5f); // 数字が表示される時間

            countdownTime--; // 次の数字に進む  
        }

        GameStartText.SetActive(true);
        characterList[roomHubModel.ConnectionId].GetComponent<Character>().Isstart = true;
        countdownText.gameObject.SetActive(false);
        StartCoroutine(HideGameStartText());

        // ゲーム開始
        StartCoroutine(CountdownTimer());
    }

    /// <summary>
    /// ゲームスタートオブジェクトを一秒後にテキストを非表示にする関数
    /// </summary>
    /// <returns></returns>
    private IEnumerator HideGameStartText()
    {
        yield return new WaitForSeconds(1.0f); // 1秒待機
        GameStartText.SetActive(false); // ゲーム開始メッセージを非表示
    }

    /// <summary>
    /// 一秒後にテキストを消す
    /// </summary>
    /// <returns></returns>
    private IEnumerator Text()
    {
        yield return new WaitForSeconds(1.0f);
        countdownText.text = "";
    }

    /// <summary>
    /// タイマーをカウントダウンするメソッド
    /// </summary>
    /// <returns></returns>
    public  IEnumerator CountdownTimer()
    {
        while (currentTime > 0)
        {
            timerText.text = currentTime.ToString(); // UIにタイマーを表示
            currentTime -= 1f; // 1秒減らす
            yield return new WaitForSeconds(1f); // 1秒待機
        }
    }

    /// <summary>
    /// 制限時間が0になったら
    /// </summary>
    private async void Timer()
    {
        if (currentTime == 0)
        {
          await  EndGameAsync(false);
        }
    }

    /// <summary>
    /// リザルトボタンが押された時の処理
    /// </summary>
    public void OnResult()
    {
        Initiate.Fade("Result", Color.black, 1);
    }

    /// <summary>
    /// デフォルト攻撃ボタンが押された時の処理
    /// </summary>
    public void AttackButton()
    {
        characterList[roomHubModel.ConnectionId].GetComponent<Character>().AttackButton();
    }

    /// <summary>
    /// 自分自身の接続IDを取得する関数
    /// </summary>
    /// <returns></returns>
    public Character GetCharacter()
    {
        //自分の接続IDを取得
        Character foundCharacter = characterList[roomHubModel.ConnectionId].GetComponent<Character>();

        return foundCharacter;
    }
}

