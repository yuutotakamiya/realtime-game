using Cinemachine;
using DG.Tweening;
using MagicOnionServer.Model.Entity;
using Shared.Interfaces.StreamingHubs;
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.TextCore.Text;
using UnityEngine.UI;
using static Shared.Interfaces.StreamingHubs.IRoomHubReceiver;
public class GameDirector : MonoBehaviour
{
    [SerializeField] GameObject[] characterPrefab;//キャラクターのPrefab
    [SerializeField] RoomHubModel roomHubModel;//RoomHubModelのクラスの設定
    [SerializeField] HumanManager humanManager;//HumanManagerのクラスの設定
    [SerializeField] InputField InpuTuserId;//ユーザーのIdを入力
    [SerializeField] InputField roomName;//ルームの名前を入力
    [SerializeField] Text roomname;
    [SerializeField] Text userId;
    [SerializeField] GameObject[] startposition;
    [SerializeField] Text timerText;
    [SerializeField] public float timeLimit;
    [SerializeField] float currentTime;
    [SerializeField] int countdownTime;
    [SerializeField] Text countdownText;
    [SerializeField] public GameObject GameFinish;
    [SerializeField] GameObject GameStartText;
    [SerializeField] GameObject Result;
    private CinemachineVirtualCamera virtualCamera; // Cinemachine Virtual Camera

    Vector3 position;
    /*private bool isGameStart = false;*/
    Animator animator;
    Rigidbody rigidbody;
    Character character;
    Dictionary<Guid, GameObject> characterList = new Dictionary<Guid, GameObject>();
    // Start is called before the first frame update
    public async void Start()
    {
        //ユーザーが入室時にOnJoinedUserメソッドを実行するよう、モデルに登録しておく
        roomHubModel.OnJoinedUser += this.OnJoinedUser;

        //ユーザーが退出時にOnLeaveメソッドを実行するよう、モデルに登録しておく
        roomHubModel.OnExitUser += this.OnExitUser;

        //ユーザーが移動したときにOnMoveCharacterメソッドを実行するよう、モデルに登録
        roomHubModel.OnMoveCharacter += this.OnMoveCharacter;

        //ルーム内にいるユーザーが全員準備完了したらOnReadyメソッドを実行するよう、モデルに登録
        roomHubModel.OnReadyUser += this.OnReady;

        //ルーム内にいるユーザーが準備完了して、ゲームが開始されたらOnTimeメソッドを実行するよう、モデルに登録
        roomHubModel.OnTime += this.OnTimer;

        //接続
        await roomHubModel.ConnectionAsync();

        //position = startposition.transform.position;

        InpuTuserId = GameObject.Find("InputFielUserId").GetComponent<InputField>();
        roomname = roomname.GetComponent<Text>();

        currentTime = timeLimit; // 初期化: 残り時間を設定

        animator = GetComponent<Animator>();

        rigidbody = GetComponent<Rigidbody>();

        virtualCamera = FindObjectOfType<CinemachineVirtualCamera>();
    }

    //入室する時に呼び出す関数
    public async void JoinRoom()
    {
        //roomname = InpuTuserId.text;
        //入室
        await roomHubModel.JoinAsync(roomname.text, int.Parse(userId.text));

        InvokeRepeating("Move", 0.1f, 0.1f);
    }

    //ユーザーが入室した時の処理
    private void OnJoinedUser(JoinedUser user)
    {

        GameObject characterObject = Instantiate(characterPrefab[user.JoinOrder], startposition[user.JoinOrder].transform.position, startposition[user.JoinOrder].transform.rotation);//Prefabを生成

        // 生成されたキャラクターをCinemachineのFollowとLook Atターゲットに設定
        if (roomHubModel.ConnectionId == user.ConnectionId)
        {
            Transform characterTransform = characterObject.transform;
            virtualCamera.Follow = characterTransform;
            virtualCamera.LookAt = characterTransform;
        }

        if (roomHubModel.ConnectionId == user.ConnectionId)
        {
            characterObject.GetComponent<Character>().isself = true;
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

    //定期的に呼び出すメソッド
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
        }
    }

    //ユーザーが準備完了を押した時のメソッド
    public async void Ready()
    {
        await roomHubModel.ReadyAsync();
    }

    //ルーム内のユーザー全員が準備完了を押したらユーザーが準備完了したときの処理
    private void OnReady(Guid connectionId, bool isReady)
    {
        characterList[roomHubModel.ConnectionId].GetComponent<Character>().isstart = true;

        StartCoroutine(StartCountdown());

        StartCoroutine("Text");
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

        GameStartText.SetActive(true); // カウントダウン終了
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

    //ゲーム内制限時間
    public async void TimeAsync(float time)
    {
        await roomHubModel.TimeAsync(time);
    }

    //定期的に呼ぶメソッド
    private void OnTimer(Guid connectionId, float time)
    {
        currentTime = time;

        StartCoroutine("CountdownTimer");
    }

    // タイマーをカウントダウンするメソッド
    public IEnumerator CountdownTimer()
    {
        while (currentTime > 0)
        {
            timerText.text = currentTime.ToString(); // UIにタイマーを表示
            currentTime -= 1f; // 1秒減らす
            yield return new WaitForSeconds(1f); // 1秒待機
        }

        if (currentTime == 0)
        {
            characterList[roomHubModel.ConnectionId].GetComponent<Character>().isstart = false;
            timerText.text = "0"; // 0秒になったら表示
            GameFinish.SetActive(true);
            Result.SetActive(true);
            //Initiate.Fade("Result",Color.black,1);
        }
    }

    public void OnResult()
    {
        Initiate.Fade("Result",Color.black,1);
    }
    // Update is called once per frame
    void Update()
    {


    }
}
