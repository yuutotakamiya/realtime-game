using Cysharp.Net.Http;
using Cysharp.Threading.Tasks;
using Grpc.Net.Client;
using MagicOnion.Client;
using MagicOnionServer.Model.Entity;
using Shared.Interfaces.StreamingHubs;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Unity.VisualScripting;
using UnityEngine;
using static Shared.Interfaces.StreamingHubs.IRoomHubReceiver;

public class RoomHubModel : BaseModel, IRoomHubReceiver
{

    private GrpcChannel channel;
    private IRoomHub roomHub;

    //接続ID
    public Guid ConnectionId;

    //ユーザー接続通知
    public Action<JoinedUser> OnJoinedUser {  get; set; }//Modelを使うクラスにはActionを使ってサーバーから届いたデータを渡す

    public Action<JoinedUser> OnExitUser { get; set; }

    public Action<Guid,Vector3,Quaternion, CharacterState> OnMoveCharacter {  get; set; }

    public Action<Guid,bool> OnReadyUser {  get; set; }

    public Action<Guid, float> OnTime { get; set; } 


    //MagicOnion接続処理
    public async UniTask ConnectionAsync()
    {
        var handler = new YetAnotherHttpHandler() { Http2Only = true };
        channel = GrpcChannel.ForAddress(ServerURL, new GrpcChannelOptions() { HttpHandler = handler });
        roomHub = await StreamingHubClient.ConnectAsync<IRoomHub, IRoomHubReceiver>(channel, this);
    }

    //MagicOnion切断処理
    public async UniTask DisconnectAsync()
    {
        if (roomHub != null) await roomHub.DisposeAsync();
        if (channel != null) await channel.ShutdownAsync();
        roomHub = null;
        channel = null;
    }

    //破棄処理
   public async void OnDestroy()
   {
        DisconnectAsync();
   }

    //入室
    public async UniTask JoinAsync(string roomName, int userId)
    {
        JoinedUser[] users = await roomHub.JoinAsync(roomName, userId);
        foreach (var user in users) 
        {
            if (user.UserData.Id == userId)
            {
                this.ConnectionId = user.ConnectionId;
            }
            OnJoinedUser(user);
        }
    }

    //入室通知(IRoomHubReceiverインターフェイスの実装)
    public void OnJoin(JoinedUser user)
    {
        OnJoinedUser(user);
    }

    //退室
    public async UniTask LeaveAsync()
    {
      await roomHub.LeaveAsync();
    }

    //退室通知(IRoomHubReceiverインターフェイスの実装)
    public void OnLeave(JoinedUser user)
    {
        OnExitUser(user);
    }

    //移動、回転通知
    public void OnMove(Guid connectionId,Vector3 pos, Quaternion rotaition, CharacterState characterState)
    {
        OnMoveCharacter(connectionId,pos,rotaition,characterState);
    }


    //移動、回転
    public async UniTask MoveAsync(Vector3 pos, Quaternion rotaition, CharacterState characterState)
    {
        await roomHub.MoveAsync(pos, rotaition,characterState);
    }

    //ユーザーの準備
    public async UniTask ReadyAsync()
    {
        await roomHub.ReadyAsync();
    }

    //ルーム内にいる全ユーザーが準備完了したかどうかの通知
    public void OnReady(Guid connectionId,bool isReady)
    {
        OnReadyUser(connectionId,isReady);
    }

    //ゲーム内制限時間
    public async UniTask TimeAsync(float time)
    {
        await roomHub.TimeAsync(time);
    }

    //ルーム内全員に制限時間を通知
    public void OnTimer(Guid connectionId,float time)
    {
        OnTime(connectionId,time);
    }
}
