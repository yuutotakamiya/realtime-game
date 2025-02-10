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
    public Action<JoinedUser> OnJoinedUser { get; set; }//Modelを使うクラスにはActionを使ってサーバーから届いたデータを渡す
    //ユーザー退室通知
    public Action<JoinedUser> OnExitUser { get; set; }
    //ユーザーの移動、回転、アニメーションの通知
    public Action<Guid, Vector3, Quaternion, CharacterState> OnMoveCharacter { get; set; }
    //ユーザー準備完了通知
    public Action<Guid, bool> OnReadyUser { get; set; }
    //制限時間の通知
    public Action<JoinedUser, float> OnTime { get; set; }
    //キル通知
    public Action<Guid, int, string> OnKillNum { get; set; }
    //マッチング通知
    public Action<string> OnMatch { get; set; }
    //宝箱の位置通知
    public Action<Vector3, Quaternion, string> OnChest { get; set; }
    //宝箱の取得数通知
    public Action<int, Dictionary<string, int>> OnChestN { get; set; }
    //ゲーム終了通知
    public Action<bool,List<ResultData>> OnEndG { get; set; }


    /// <summary>
    /// MagicOnion接続処理
    /// </summary>
    /// <returns></returns>
    public async UniTask ConnectionAsync()
    {
        var handler = new YetAnotherHttpHandler() { Http2Only = true };
        channel = GrpcChannel.ForAddress(ServerURL, new GrpcChannelOptions() { HttpHandler = handler });
        roomHub = await StreamingHubClient.ConnectAsync<IRoomHub, IRoomHubReceiver>(channel, this);
    }

    /// <summary>
    /// MagicOnion切断処理
    /// </summary>
    /// <returns></returns>
    public async UniTask DisconnectAsync()
    {
        if (roomHub != null) await roomHub.DisposeAsync();
        if (channel != null) await channel.ShutdownAsync();
        roomHub = null;
        channel = null;
    }

    /// <summary>
    /// 破棄処理
    /// </summary>
    public async void OnDestroy()
    {
        DisconnectAsync();
    }

    /// <summary>
    /// 入室
    /// </summary>
    /// <param name="roomName"></param>
    /// <param name="userId"></param>
    /// <returns></returns>
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

    /// <summary>
    /// 入室通知(IRoomHubReceiverインターフェイスの実装)
    /// </summary>
    /// <param name="user"></param>
    public void OnJoin(JoinedUser user)
    {
        if (OnJoinedUser != null)
        {
            OnJoinedUser(user);
        }
    }

    /// <summary>
    /// 退室
    /// </summary>
    /// <returns></returns>
    public async UniTask LeaveAsync()
    {
        await roomHub.LeaveAsync();
    }

    /// <summary>
    /// 退室通知(IRoomHubReceiverインターフェイスの実装)
    /// </summary>
    /// <param name="user"></param>
    public void OnLeave(JoinedUser user)
    {
        if (OnExitUser != null)
        {
            OnExitUser(user);
        }
    }

    /// <summary>
    /// 移動、回転通知
    /// </summary>
    /// <param name="connectionId"></param>
    /// <param name="pos"></param>
    /// <param name="rotaition"></param>
    /// <param name="characterState"></param>
    public void OnMove(Guid connectionId, Vector3 pos, Quaternion rotaition, CharacterState characterState)
    {
        if (OnMoveCharacter!=null)
        {
            OnMoveCharacter(connectionId, pos, rotaition, characterState);
        }
    }

    /// <summary>
    /// 移動、回転
    /// </summary>
    /// <param name="pos"></param>
    /// <param name="rotaition"></param>
    /// <param name="characterState"></param>
    /// <returns></returns>
    public async UniTask MoveAsync(Vector3 pos, Quaternion rotaition, CharacterState characterState)
    {
        await roomHub.MoveAsync(pos, rotaition, characterState);
    }

    /// <summary>
    /// ユーザーの準備
    /// </summary>
    /// <returns></returns>
    public async UniTask ReadyAsync()
    {
        await roomHub.ReadyAsync();
    }

    /// <summary>
    /// 準備完了通知
    /// </summary>
    /// <param name="connectionId"></param>
    /// <param name="isReady"></param>
    public void OnReady(Guid connectionId, bool isReady)
    {
        OnReadyUser(connectionId, isReady);
    }

    /// <summary>
    /// ゲーム内制限時間
    /// </summary>
    /// <param name="time"></param>
    /// <returns></returns>
    public async UniTask TimeAsync(float time)
    {
        await roomHub.TimeAsync(time);
    }

    /// <summary>
    /// 制限時間通知
    /// </summary>
    /// <param name="user"></param>
    /// <param name="time"></param>
    public void OnTimer(JoinedUser user, float time)
    {
        OnTime(user, time);
    }

    /// <summary>
    /// 鬼が誰をキルしたかの処理
    /// </summary>
    /// <returns></returns>
    public async UniTask KillAsync()
    {
        await roomHub.KillAsync();
    }

    /// <summary>
    /// 鬼がだれをキルしたかを通知
    /// </summary>
    /// <param name="connectionId"></param>
    /// <param name="totalKillNum"></param>
    /// <param name="userName"></param>
    public void OnKill(Guid connectionId, int totalKillNum, string userName)
    {
        OnKillNum(connectionId, totalKillNum, userName);
    }

    /// <summary>
    /// マッチングの同期
    /// </summary>
    /// <param name="userId"></param>
    /// <returns></returns>
    public async UniTask JoinLobbyAsync(int userId)
    {
        JoinedUser[] users = await roomHub.JoinLobbyAsync(userId);
        foreach (var user in users)
        {
            if (user.UserData.Id == userId)
            {
                this.ConnectionId = user.ConnectionId;
            }
            OnJoinedUser(user);
        }
    }

    /// <summary>
    /// マッチング通知
    /// </summary>
    /// <param name="roomName"></param>
    public void OnMatching(string roomName)
    {
        OnMatch(roomName);
    }

    /// <summary>
    /// 宝箱の位置同期
    /// </summary>
    /// <param name="pos"></param>
    /// <param name="rotaition"></param>
    /// <param name="Namechest"></param>
    /// <returns></returns>
    public async UniTask MoveChest(Vector3 pos, Quaternion rotaition, string Namechest)
    {
        await roomHub.MoveChest(pos, rotaition, Namechest);
    }

    /// <summary>
    /// 宝箱の位置の通知
    /// </summary>
    /// <param name="pos"></param>
    /// <param name="rotaition"></param>
    /// <param name="Namechest"></param>
    public void OnMoveChest(Vector3 pos, Quaternion rotaition, string Namechest)
    {
        OnChest(pos, rotaition, Namechest);
    }

   /// <summary>
   /// 宝箱の取得数同期
   /// </summary>
   /// <returns></returns>
    public async UniTask GainChest()
    {
        await roomHub.GainChest();
    }

    /// <summary>
    /// 宝箱の取得数合計の通知
    /// </summary>
    /// <param name="TotalChestNum"></param>
    /// <param name="keyValuePairs"></param>
    public void OnChestNum(int TotalChestNum, Dictionary<string, int> keyValuePairs)
    {
        OnChestN(TotalChestNum, keyValuePairs);
    }

    /// <summary>
    /// ゲーム終了同期
    /// </summary>
    /// <returns></returns>
    public async UniTask EndGameAsync()
    {
        await roomHub.EndGameAsync();
    }

    /// <summary>
    /// ゲーム終了通知
    /// </summary>
    /// <param name="isHumanEndGame"></param>
    public void OnEndGame(bool isHumanEndGame,List<ResultData> resultData)
    {
        OnEndG(isHumanEndGame,resultData);
    }

}
