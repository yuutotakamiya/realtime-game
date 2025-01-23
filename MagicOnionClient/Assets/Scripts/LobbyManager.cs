using MagicOnionServer.Model.Entity;
using Shared.Interfaces.StreamingHubs;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using Cysharp.Threading.Tasks;

public class LobbyManager : MonoBehaviour
{
    //[SerializeField] GameObject MachingPrefab;//
    [SerializeField] RoomHubModel roomHubModel;//roomHubModelクラスの設定
    [SerializeField] GameObject MachingText;//マッチングText;
    [SerializeField] GameObject MachingIcon;//マッチング中のIcon
    [SerializeField] GameObject[] MachingStartPositon;//マッチングスタートポジション
    [SerializeField] GameObject[] characterPrefab;//キャラクターのPrefab

    static string roomName;//ルーム名を入れるための変数

    Dictionary<Guid, GameObject> characterList = new Dictionary<Guid, GameObject>();

    // 最大のJoinOrder（最大プレイヤー数4人）
    private int maxPlayers = 4;
    //ルーム名をプロパティ化
    public static string RoomName
    {
        get { return roomName; }
    }

    // Start is called before the first frame update
    public async void Start()
    {
        //接続
        await roomHubModel.ConnectionAsync();

        //ユーザーが入室時にOnJoinedUserメソッドを実行するよう、モデルに登録しておく
        roomHubModel.OnJoinedUser += this.OnJoinedUser;

        //ルーム内の人数が4人集まったら、OnMachingメソッドを実行するようモデルに登録
        roomHubModel.OnMatch += this.OnMaching;

        //ユーザーが退出時にOnLeaveメソッドを実行するよう、モデルに登録しておく
        roomHubModel.OnExitUser += this.OnExitUser;

        MachingIcon.SetActive(true);
        MachingText.SetActive(true);
        await JoinRoom();
    }

    // Update is called once per frame
    void Update()
    {

    }

    //入室する時に呼び出す関数
    public async UniTask JoinRoom()
    {
        //入室
        await roomHubModel.JoinLobbyAsync(UserModel.Instance.userId);
    }


    //ユーザーが入室した時の処理
    private void OnJoinedUser(JoinedUser user)
    {
        //キャラクターの生成
        GameObject Character = Instantiate(characterPrefab[user.JoinOrder],
          MachingStartPositon[user.JoinOrder].transform.position,
          MachingStartPositon[user.JoinOrder].transform.rotation);

        //自分の接続IDとroomHubModelの接続IDが同じなら
        if (roomHubModel.ConnectionId == user.ConnectionId)
        {
            //自分の名前を表示
            Character.GetComponent<NameManager>().Name(user.UserData.Name);
        }

        Character.transform.position = MachingStartPositon[user.JoinOrder].transform.position;
        characterList[user.ConnectionId] = Character;//フィールドで保持

    }

    //マッチングしたときに通知を出す処理
    public async void OnMaching(string roomName)
    {
        LobbyManager.roomName = roomName;

        await UniTask.Delay(TimeSpan.FromSeconds(1.0f));  // 非同期で1秒待機

        MachingIcon.SetActive(false);
        MachingText.SetActive(false);
        Initiate.Fade("Game", Color.black, 1.0f);
    }


    //退室するときに呼び出す関数
    public async void ExitRoom()
    {
        await roomHubModel.LeaveAsync();
        // 全てのキャラクターオブジェクトを削除
        foreach (var entry in characterList)
        {
            Destroy(entry.Value);  // キャラクターオブジェクトを破棄
        }

        // characterListをクリア
        characterList.Clear();

        // 自分のConnectionIdをリセット
        roomHubModel.ConnectionId = Guid.Empty;

        Initiate.Fade("Title", Color.black, 1.0f);
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
}
