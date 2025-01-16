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
    [SerializeField] GameObject MachingPrefab;
    [SerializeField] RoomHubModel roomHubModel;
    [SerializeField] InputField userIdText;
    [SerializeField] Transform Content;
    [SerializeField] GameObject MachingText;
    [SerializeField] GameObject MachingIcon;
    [SerializeField] GameObject[] MachingStartPositon;
    [SerializeField] GameObject[] characterPrefab;

    static string roomName;

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

        GameObject Character = Instantiate(characterPrefab[user.JoinOrder],
          MachingStartPositon[user.JoinOrder].transform.position,
          MachingStartPositon[user.JoinOrder].transform.rotation);

        Character.GetComponent<Character>().Name(user.UserData.Name);
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

        Initiate.Fade("Title", Color.black, 1.0f);
    }

    //ユーザーが退室したときの処理
    private void OnExitUser(JoinedUser user)
    {
        // 退室したユーザーのキャラクターオブジェクトを削除
        if (user.ConnectionId==roomHubModel.ConnectionId)
        {
            Destroy(this.gameObject);  // オブジェクトを破棄
        }
    }
}
