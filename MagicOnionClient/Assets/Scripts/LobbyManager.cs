using MagicOnionServer.Model.Entity;
using Shared.Interfaces.StreamingHubs;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

public class LobbyManager : MonoBehaviour
{
    [SerializeField] GameObject MachingPrefab;
    [SerializeField] RoomHubModel roomHubModel;
    [SerializeField] InputField userIdText;
    [SerializeField] Transform Content;

    static string roomName;

    //ルーム名をプロパティ
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

        JoinRoom();
   }

    // Update is called once per frame
    void Update()
    {
        
    }

    //入室する時に呼び出す関数
    public async void JoinRoom()
    {
        //入室
        await roomHubModel.JoinLobbyAsync(UserModel.Instance.userId);

    }

    //ユーザーが入室した時の処理
    private void OnJoinedUser(JoinedUser user)
    {
        GameObject TextObject = Instantiate(MachingPrefab, Content);

        Text MachingText = TextObject.GetComponent<Text>();

        MachingText.text = $"ID:{user.UserData.Id},名前:{user.UserData.Name}";
    }

    //マッチングしたときに通知
    public void OnMaching(string roomName)
    {
        LobbyManager.roomName = roomName;

        Initiate.Fade("Game", Color.black, 1.0f);
    }


    //退室するときに呼び出す関数
    public async void ExitRoom()
    {
        await roomHubModel.LeaveAsync();

        Initiate.Fade("Title", Color.black, 1.0f);
    }

    //ユーザーが退室したときの処理
    private void OnExitUser(JoinedUser user)
    {
        // 退室したユーザーのキャラクターオブジェクトを削除
        /*if (GameDirector.Instance.characterList.ContainsKey(user.ConnectionId))
        {
            Destroy(GameDirector.Instance.characterList[user.ConnectionId]);  // オブジェクトを破棄
            GameDirector.Instance.characterList.Remove(user.ConnectionId);    // リストから削除
        }*/
    }
}
