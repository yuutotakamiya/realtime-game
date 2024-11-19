using Shared.Interfaces.StreamingHubs;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameDirector : MonoBehaviour
{
    [SerializeField] GameObject characterPrefab;
    [SerializeField] RoomHubModel roomHubModel;
    Dictionary<Guid,GameObject> characterList = new Dictionary<Guid,GameObject>();
    // Start is called before the first frame update
  public async void Start()
  {
        //ユーザーが入室時にOnJoinedUserメソッドを実行するよう、モデルに登録しておく
        roomHubModel.OnJoinedUser += this.OnJoinedUser;
        //接続
        await roomHubModel.ConnectionAsync();
  }

    public async void JoinRoom()
    {
        //入室
        await roomHubModel.JoinAsync("sampleroom", 1);
    }

    //ユーザーが入室した時の処理
    private void OnJoinedUser(JoinedUser user)
    {
        GameObject characterObject = Instantiate(characterPrefab);//インスタンス生成
        characterObject.transform.position = new Vector3(0, 0, 0);
        characterList[user.ConnectionId] = characterObject;//フィールドで保持
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
