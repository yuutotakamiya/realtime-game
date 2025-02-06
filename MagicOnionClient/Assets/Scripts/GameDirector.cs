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
public class GameDirector : MonoBehaviour
{
    //キャラクター関係
    [SerializeField] GameObject[] characterPrefab;//キャラクターのPrefab
    [SerializeField] GameObject[] startposition;//最初のスタートポジション

    //クラスの設定
    [SerializeField] RoomHubModel roomHubModel;//RoomHubModelのクラスの設定
    [SerializeField] DefenceTarget defenceTarget;

    //カウントダウン
    [SerializeField] public float timeLimit;//制限時間を設定
    [SerializeField] float currentTime;//現在のタイム
    [SerializeField] int countdownTime;//ゲームが始まる前のカウントダウン設定

    //UI
    [SerializeField] Text timerText;//タイマーText
    [SerializeField] Text countdownText;//カウントダウンText
    [SerializeField] Text Crrenttext;//現在のキル数
    [SerializeField] Text KillNum;//キル数
    [SerializeField] Text KillLog;//キル通知
    [SerializeField] Image skullIamge;//頭蓋骨の画像
    [SerializeField] Text killerKakeru;//×Text;
    [SerializeField] Text humanKakeru;//×Text;
    [SerializeField] public GameObject GameFinish;//ゲーム終了Text
    [SerializeField] GameObject GameStartText;//ゲームスタートText
    [SerializeField] GameObject Result;//リザルト画面に行くためのボタン
    [SerializeField] GameObject AttackButton1;//デフォルトの攻撃ボタン
    [SerializeField] GameObject AttackButton2;//雷攻撃
    [SerializeField] Image MiniMap;//ミニマップ
    [SerializeField] Text ChestNumText;//宝箱の取得した数を入れるText
    [SerializeField] GameObject WinText;
    [SerializeField] GameObject WinText2;
    [SerializeField] Image ChestImage;

    /*[SerializeField] public GameObject openButton;
    [SerializeField] public GameObject closeButton;
    /*[SerializeField] public GameObject holdButton;
    [SerializeField] public GameObject notholdButton;
    [SerializeField] public GameObject placeButton;*/

    private CinemachineVirtualCamera virtualCamera; // Cinemachine Virtual Camera

    private bool isEnemy = false;//自分が敵かどうか
    private bool ishave = false;//宝箱を持っているかどうか

    Vector3 position;
    Animator animator;
    Rigidbody rigidbody;
    Character character;

    private Dictionary<Guid, GameObject> characterList = new Dictionary<Guid, GameObject>();

    public static Dictionary<string,int> keyValuePairs;//名前と宝箱の名前をフィールドに保存

    private JoinedUser MyName; //自分自身の名前をフィールドに保存

    //自分自身が敵かどうかのプロパティ
    public bool IsEnemy
    {
        get { return isEnemy; }
        set { isEnemy = value; }
    }
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

        animator = GetComponent<Animator>();
        rigidbody = GetComponent<Rigidbody>();
        virtualCamera = FindObjectOfType<CinemachineVirtualCamera>();

        KillNum.text = "0";

        await JoinRoom();
        await UniTask.Delay(TimeSpan.FromSeconds(4.0f));  // 非同期で4秒待機
        await Ready();
    }

    //入室する時に呼び出す関数
    public async UniTask JoinRoom()
    {
        //入室
        await roomHubModel.JoinAsync(LobbyManager.RoomName, UserModel.Instance.userId);

        InvokeRepeating("Move", 0.1f, 0.1f);
    }

    //ユーザーが入室した時の処理
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
                //AttackButton2.SetActive(true);
                KillNum.gameObject.SetActive(true);
                //Crrenttext.gameObject.SetActive(true);
                skullIamge.gameObject.SetActive(true);
                MiniMap.gameObject.SetActive(true);
                killerKakeru.gameObject.SetActive(true);
                ChestNumText.gameObject.SetActive(false);
                ChestImage.gameObject.SetActive(false);
                humanKakeru.gameObject.SetActive (false);

            }
            else
            {
                AttackButton1.SetActive(false);
                AttackButton2.SetActive(false);
                KillNum.gameObject.SetActive(false);
                Crrenttext.gameObject.SetActive(false);
                skullIamge.gameObject .SetActive(false);
                MiniMap.gameObject .SetActive(false);
                killerKakeru.gameObject .SetActive(false);
                ChestNumText.gameObject .SetActive(true);
                ChestImage.gameObject .SetActive(true);
                humanKakeru.gameObject.SetActive (true);
            }

            // 生成されたキャラクターをCinemachineのFollowとLook Atターゲットに設定
            Transform characterTransform = characterObject.transform;
            virtualCamera.Follow = characterTransform;//キャラクターにカメラをフォロー
            virtualCamera.LookAt = characterTransform;//キャラクターにカメラをロック
        }

        if (roomHubModel.ConnectionId == user.ConnectionId)
        {
            characterObject.GetComponent<Character>().Isself = true;
        }

        characterObject.transform.position = startposition[user.JoinOrder].transform.position;
        characterList[user.ConnectionId] = characterObject;//フィールドで保持
    }

    //退室するときに呼び出す関数
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

    //ユーザーが退室したときの処理
    private void OnExitUser(JoinedUser user)
    {
        // 退室したユーザーのキャラクターオブジェクトを削除
        if (characterList.ContainsKey(user.ConnectionId))
        {
            Destroy(characterList[user.ConnectionId]);  // オブジェクトを破棄
            characterList.Remove(user.ConnectionId);    // リストから削除
        }
    }

    //キャラクターの位置を定期的に呼び出すメソッド
    public async void Move()
    {
        //自分自身のtransform.position、Quaternion.identity,アニメーションをサーバーに送信
        await roomHubModel.MoveAsync(characterList[roomHubModel.ConnectionId].gameObject.transform.position,
            characterList[roomHubModel.ConnectionId].gameObject.transform.rotation,
           (CharacterState)characterList[roomHubModel.ConnectionId].GetComponent<Animator>().GetInteger("state"));
    }

    //ユーザーの移動、回転、アニメーション
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

            //Debug.Log(characterState);
        }
    }

    //ユーザーが準備完了を押した時のメソッド
    public async UniTask Ready()
    {
        await roomHubModel.ReadyAsync();
    }

    //ルーム内のユーザー全員が準備完了していたらの処理
    private void OnReady(Guid connectionId, bool isReady)
    {
        isReady = true;
        StartCoroutine(StartCountdown());
        StartCoroutine("Text");
    }

    //ゲーム内制限時間
    public async void Time(float time)
    {
        await roomHubModel.TimeAsync(time);
    }

    //定期的に呼ぶメソッド(ゲーム内制限時間)
    private async void OnTimer(JoinedUser user, float time)
    {
        currentTime = time;
        await Ready();
    }

    //キルしたときのメソッド
    public async void KillAsync()
    {
        await roomHubModel.KillAsync();
    }

    //キルしたときの通知
    public void OnKill(Guid connectionId, int TotalKillNum,string userName)
    {
        KillNum.text = TotalKillNum.ToString();

        AnimateKillLog(userName);
    }

    //宝箱の位置同期
    public async void MoveChest(Vector3 pos, Quaternion rotaition, string Namechest)
    {
        await roomHubModel.MoveChest(pos, rotaition, Namechest);
    }

    //宝箱の位置を定期的に通知するメソッド
    public void OnMoveChest(Vector3 pos, Quaternion rotaition, string Namechest)
    {
        GameObject chest = GameObject.Find(Namechest);

        chest.transform.rotation = rotaition;
        chest.transform.position = pos;
    }

    //宝箱の取得数同期
    public async UniTask GainChest()
    {
        await roomHubModel.GainChest();
    }

    //宝箱の取得数通知
    public async void OnChestNum(int TotalChestNum,Dictionary<string,int> keyValuePairs)
    {
        if (keyValuePairs.ContainsKey(MyName.UserData.Name))
        {
            ChestNumText.text = keyValuePairs[MyName.UserData.Name].ToString();
        }

        //宝箱を合計2個取得したら
        if (TotalChestNum == 2)
        {
           await EndGameAsync();
        }
    }

    //ゲーム終了同期
    public async UniTask EndGameAsync()
    {
        await roomHubModel.EndGameAsync();
    }

    //ゲーム終了通知
    public async void OnEndGame(bool isHumanEndGame)
    {
        if (isHumanEndGame==true)
        {
            characterList[roomHubModel.ConnectionId].GetComponent<Character>().Isstart = false;
            WinText.SetActive(true);
            WinText2.SetActive(true);
            StopCoroutine("CountdownTimer");
            Invoke("LoadResult", 3);
        }
        else
        {
            characterList[roomHubModel.ConnectionId].GetComponent<Character>().Isstart = false;
            timerText.text = "0"; // 0秒になったら表示
            GameFinish.SetActive(true);
            //await UniTask.Delay(TimeSpan.FromSeconds(4.0f));  // 非同期で4秒待機
            Invoke("LoadResult", 3);
        }
    }

    public void LoadResult()
    {
        Initiate.Fade("Result", Color.black, 1);
    }


    //DoTweenを使ったキルログアニメーション
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


    // カウントダウンを行うメソッド
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

    //ゲームスタートオブジェクトを一秒後にテキストを非表示にする関数
    private IEnumerator HideGameStartText()
    {
        yield return new WaitForSeconds(1.0f); // 1秒待機
        GameStartText.SetActive(false); // ゲーム開始メッセージを非表示
    }
    //一秒後にテキストを消す
    private IEnumerator Text()
    {
        yield return new WaitForSeconds(1.0f);
        countdownText.text = "";
    }

    // タイマーをカウントダウンするメソッド
    public  IEnumerator CountdownTimer()
    {
        while (currentTime > 0)
        {
            timerText.text = currentTime.ToString(); // UIにタイマーを表示
            currentTime -= 1f; // 1秒減らす
            yield return new WaitForSeconds(1f); // 1秒待機
        }

        if (currentTime == 0)
        {
            EndGameAsync();
        }
    }

    //リザルトボタンが押された時の処理
    public void OnResult()
    {
        Initiate.Fade("Result", Color.black, 1);
    }

    //デフォルト攻撃ボタンが押された時の処理
    public void AttackButton()
    {
        characterList[roomHubModel.ConnectionId].GetComponent<Character>().AttackButton();
    }

    //自分自身の接続IDを取得
    public Character GetCharacter()
    {
        Character foundCharacter = characterList[roomHubModel.ConnectionId].GetComponent<Character>();

        return foundCharacter;
    }

    public void LightningAttak()
    {
        characterList[roomHubModel.ConnectionId].GetComponent<Character>().LightningAttack();
    }

    // Update is called once per frame
    void Update()
    {

    }
}

