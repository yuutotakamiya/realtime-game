//==========================================================
//
//マッチングの管理処理
//Author:高宮祐翔
//
//==========================================================
using MagicOnionServer.Model.Entity;
using Shared.Interfaces.StreamingHubs;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using Cysharp.Threading.Tasks;

/// <summary>
///　マッチングを管理しているスクリプト
/// </summary>
public class LobbyManager : MonoBehaviour
{
    [SerializeField] RoomHubModel roomHubModel;//roomHubModelクラスの設定
    [SerializeField] GameObject MachingText;//マッチングText;
    [SerializeField] GameObject MachingIcon;//マッチング中のIcon
    [SerializeField] GameObject[] MachingStartPositon;//マッチングスタートポジション
    [SerializeField] GameObject[] characterPrefab;//キャラクターのPrefab

    static string roomName;//ルーム名を入れるための変数

    Dictionary<Guid, GameObject> characterList = new Dictionary<Guid, GameObject>();

    //ルーム名をプロパティ化
    public static string RoomName
    {
        get { return roomName; }
    }

    /// <summary>
    /// 一番最初に呼ばれる関数
    /// </summary>
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
        await JoinRoom();//非同期でJoinRoom関数を呼び出し
    }

    private void OnDestroy()
    {
        //OnJoinedUser通知のの登録解除
        roomHubModel.OnJoinedUser -= this.OnJoinedUser;

        //OnMaching通知の登録解除
        roomHubModel.OnMatch -= this.OnMaching;

        //OnLeave通知の登録解除
        roomHubModel.OnExitUser -= this.OnExitUser;
    }

    /// <summary>
    /// 入室する時に呼び出す関数
    /// </summary>
    /// <returns></returns>
    public async UniTask JoinRoom()
    {
        //入室
        await roomHubModel.JoinLobbyAsync(UserModel.Instance.userId);
    }


    /// <summary>
    /// ユーザーが入室した時の処理
    /// </summary>
    /// <param name="user"></param>
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

    /// <summary>
    /// マッチングしたときに通知を出す処理
    /// </summary>
    /// <param name="roomName"></param>
    public async void OnMaching(string roomName)
    {
        LobbyManager.roomName = roomName;

        await UniTask.Delay(TimeSpan.FromSeconds(1.0f));  // 非同期で1秒待機

        MachingIcon.SetActive(false);
        MachingText.SetActive(false);
        Initiate.Fade("Game", Color.black, 1.0f);
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
        }

        // characterListをクリア
        characterList.Clear();

        // 自分のConnectionIdをリセット
        roomHubModel.ConnectionId = Guid.Empty;

        Initiate.Fade("Title", Color.black, 1.0f);
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
}
