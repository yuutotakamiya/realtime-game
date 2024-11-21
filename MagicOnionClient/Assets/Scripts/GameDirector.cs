using MagicOnionServer.Model.Entity;
using Shared.Interfaces.StreamingHubs;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameDirector : MonoBehaviour
{
    [SerializeField] GameObject characterPrefab;//キャラクターのPrefab
    [SerializeField] RoomHubModel roomHubModel;//RoomHubModelのクラスの設定
    [SerializeField] InputField InpuTuserId;//ユーザーのIdを入力
    [SerializeField] InputField roomName;//ルームの名前を入力
    [SerializeField] Text roomname;
    [SerializeField] GameObject startposition;
    Vector3 position;
    Dictionary <Guid,GameObject> characterList = new Dictionary<Guid,GameObject>();
    // Start is called before the first frame update
    public async void Start()
    {
        //ユーザーが入室時にOnJoinedUserメソッドを実行するよう、モデルに登録しておく
        roomHubModel.OnJoinedUser += this.OnJoinedUser;

        //ユーザーが退出時にOnLeaveメソッドを実行するよう、モデルに登録しておく
        roomHubModel.OnExitUser += this.OnExitUser;

        //接続
        await roomHubModel.ConnectionAsync();
        
        position = startposition.transform.position;

        InpuTuserId = GameObject.Find("InputFielUserId").GetComponent<InputField>();
        roomname = roomname. GetComponent<Text>();
    }

    //入室する時に呼び出す関数
    public async void JoinRoom()
    {
        //roomname = InpuTuserId.text;
        //入室
        await roomHubModel.JoinAsync("sampleroom", 1);
    }

    //ユーザーが入室した時の処理
    private void OnJoinedUser(JoinedUser user)
    {
        GameObject characterObject = Instantiate(characterPrefab);//インスタンス生成
        characterObject.transform.position = startposition.transform.position;
        characterList[user.ConnectionId] = characterObject;//フィールドで保持
    }

    //退室するときに呼び出す関数
    public async void ExitRoom()
    {
        await roomHubModel.LeaveAsync();

        // キャラクターオブジェクトを削除
        if (characterList.ContainsKey(roomHubModel.ConnectionId))
        {
            Destroy(characterList[roomHubModel.ConnectionId]);
            characterList.Remove(roomHubModel.ConnectionId);
        }
    }

    //ユーザーが退室したときの処理
    private void OnExitUser(JoinedUser user)
    {
        Destroy(characterList[user.ConnectionId].gameObject);
    }



    // Update is called once per frame
    void Update()
    {
        
    }


}
