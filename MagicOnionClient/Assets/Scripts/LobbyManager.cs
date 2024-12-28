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

    static string roomName;
    GameObject MachingIcon;
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

         MachingIcon = GameObject.Find("Spinner 1");

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
        GameObject TextObject = Instantiate(MachingPrefab, Content);

        Text MachingText = TextObject.GetComponent<Text>();

        MachingText.text = $"ID:{user.UserData.Id},名前:{user.UserData.Name}";
    }

    //マッチングしたときに通知
    public async void OnMaching(string roomName)
    {
        LobbyManager.roomName = roomName;

        await UniTask.Delay(TimeSpan.FromSeconds(1.0f));  // 非同期で1秒待機

        MachingIcon.SetActive(false);
        Initiate.Fade("Game", Color.black, 1.0f);
    }


    //退室するときに呼び出す関数
    public async void ExitRoom()
    {
        await roomHubModel.LeaveAsync();

        // クライアント側のUIやオブジェクトをクリア
        foreach (Transform child in Content)
        {
            Destroy(child.gameObject); // プレイヤーの表示を削除
        }

        Initiate.Fade("Title", Color.black, 1.0f);
    }

    //ユーザーが退室したときの処理
    private void OnExitUser(JoinedUser user)
    {
        // 退室したユーザーに対応するオブジェクトを削除
        if (user.ConnectionId == roomHubModel.ConnectionId)
        {
            foreach (Transform child in Content)
            {
                if (child.GetComponent<Text>().text.Contains(user.UserData.Name))
                {
                    Destroy(child.gameObject); // プレイヤーの表示を削除
                    break;
                }
            }
        }
    }
}
