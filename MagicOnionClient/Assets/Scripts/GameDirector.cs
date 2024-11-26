using DG.Tweening;
using MagicOnionServer.Model.Entity;
using Shared.Interfaces.StreamingHubs;
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
public class GameDirector : MonoBehaviour
{
    [SerializeField] GameObject characterPrefab;//キャラクターのPrefab
    [SerializeField] RoomHubModel roomHubModel;//RoomHubModelのクラスの設定
    [SerializeField] InputField InpuTuserId;//ユーザーのIdを入力
    [SerializeField] InputField roomName;//ルームの名前を入力
    [SerializeField] Text roomname;
    [SerializeField] Text userId;
    [SerializeField] GameObject startposition;
    //[SerializeField] float speed = 3.0f;
    Vector3 position;
    Dictionary <Guid,GameObject> characterList = new Dictionary<Guid,GameObject>();
    // Start is called before the first frame update
    public async void Start()
    {
        //ユーザーが入室時にOnJoinedUserメソッドを実行するよう、モデルに登録しておく
        roomHubModel.OnJoinedUser += this.OnJoinedUser;

        //ユーザーが退出時にOnLeaveメソッドを実行するよう、モデルに登録しておく
        roomHubModel.OnExitUser += this.OnExitUser;

        //ユーザーが移動したときにOnMoveCharacterメソッドを実行するよう、モデルに登録
        roomHubModel.OnMoveCharacter += this.OnMoveCharacter;

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
        await roomHubModel.JoinAsync("sampleroom",int.Parse(userId.text));

        InvokeRepeating("Move", 0.1f, 0.1f);
      
    }

    //ユーザーが入室した時の処理
    private void OnJoinedUser(JoinedUser user)
    {
        GameObject characterObject = Instantiate(characterPrefab);//インスタンス生成
        if (roomHubModel.ConnectionId == user.ConnectionId)
        {
            characterObject.GetComponent<Move>().isself = true;
        }
        characterObject.transform.position = startposition.transform.position;
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
        }

        // characterListをクリア
        characterList.Clear();

        // 自分のConnectionIdをリセット
        roomHubModel.ConnectionId = Guid.Empty;

        CancelInvoke("Move");
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
    private async void Move()
    {
        //自分自身のtransform.position、Quaternion.identityをサーバーに送信
        await roomHubModel.MoveAsync(characterList[roomHubModel.ConnectionId].gameObject.transform.position, characterList[roomHubModel.ConnectionId].gameObject.transform.rotation);
    }

    //ユーザーの移動、回転
    private void OnMoveCharacter(Guid connectionId, Vector3 pos,Quaternion rotaition)
    {

        if (characterList.ContainsKey(connectionId))
        {
            GameObject character = characterList[connectionId];

            // キャラクターの位置と回転をサーバーの値に更新
            character.transform.DOLocalMove(pos, 0.1f).SetEase(Ease.Linear);
            character.transform.DORotate(rotaition.eulerAngles, 0.1f).SetEase(Ease.Linear);
        }

    }
    // Update is called once per frame
    void Update()
    {
     
    }
    
}
