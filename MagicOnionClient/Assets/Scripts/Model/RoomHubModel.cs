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

public class RoomHubModel : BaseModel, IRoomHubReceiver
{

    private GrpcChannel channel;
    private IRoomHub roomHub;

    //�ڑ�ID
    public Guid ConnectionId;

    //���[�U�[�ڑ��ʒm
    public Action<JoinedUser> OnJoinedUser {  get; set; }//Model���g���N���X�ɂ�Action���g���ăT�[�o�[����͂����f�[�^��n��

    public Action<JoinedUser> OnExitUser { get; set; }

    public Action<Guid,Vector3> OnMoveCharacter {  get; set; }


    //MagicOnion�ڑ�����
    public async UniTask ConnectionAsync()
    {
        var handler = new YetAnotherHttpHandler() { Http2Only = true };
        channel = GrpcChannel.ForAddress(ServerURL, new GrpcChannelOptions() { HttpHandler = handler });
        roomHub = await StreamingHubClient.ConnectAsync<IRoomHub, IRoomHubReceiver>(channel, this);
    }

    //MagicOnion�ؒf����
    public async UniTask DisconnectAsync()
    {
        if (roomHub != null) await roomHub.DisposeAsync();
        if (channel != null) await channel.ShutdownAsync();
        roomHub = null;
        channel = null;
    }

    //�j������
   public async void OnDestroy()
   {
        DisconnectAsync();
   }

    //����
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

    //�����ʒm(IRoomHubReceiver�C���^�[�t�F�C�X�̎���)
    public void OnJoin(JoinedUser user)
    {
        OnJoinedUser(user);
    }

    //�ގ�
    public async UniTask LeaveAsync()
    {
      await roomHub.LeaveAsync();
    }

    //�ގ��ʒm(IRoomHubReceiver�C���^�[�t�F�C�X�̎���)
    public void OnLeave(JoinedUser user)
    {
        OnExitUser(user);
    }

    //�ړ��ʒm
    public void OnMove(Guid connectionId,Vector3 pos)
    {
        OnMoveCharacter(connectionId,pos);
    }


    //�ړ�
    public async UniTask MoveAsync(Vector3 pos)
    {
        await roomHub.MoveAsync(pos);
    }
}