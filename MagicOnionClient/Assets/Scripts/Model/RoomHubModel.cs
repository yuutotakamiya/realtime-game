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

    //�ڑ�ID
    public Guid ConnectionId;

    //���[�U�[�ڑ��ʒm
    public Action<JoinedUser> OnJoinedUser { get; set; }//Model���g���N���X�ɂ�Action���g���ăT�[�o�[����͂����f�[�^��n��
    //���[�U�[�ގ��ʒm
    public Action<JoinedUser> OnExitUser { get; set; }
    //���[�U�[�̈ړ��A��]�A�A�j���[�V�����̒ʒm
    public Action<Guid, Vector3, Quaternion, CharacterState> OnMoveCharacter { get; set; }
    //���[�U�[���������ʒm
    public Action<Guid, bool> OnReadyUser { get; set; }
    //�������Ԃ̒ʒm
    public Action<JoinedUser, float> OnTime { get; set; }
    //�L���ʒm
    public Action<Guid, int, string> OnKillNum { get; set; }
    //�}�b�`���O�ʒm
    public Action<string> OnMatch { get; set; }
    //�󔠂̈ʒu�ʒm
    public Action<Vector3, Quaternion, string> OnChest { get; set; }
    //�󔠂̎擾���ʒm
    public Action<int, Dictionary<string, int>> OnChestN { get; set; }
    //�Q�[���I���ʒm
    public Action<bool,List<ResultData>> OnEndG { get; set; }


    /// <summary>
    /// MagicOnion�ڑ�����
    /// </summary>
    /// <returns></returns>
    public async UniTask ConnectionAsync()
    {
        var handler = new YetAnotherHttpHandler() { Http2Only = true };
        channel = GrpcChannel.ForAddress(ServerURL, new GrpcChannelOptions() { HttpHandler = handler });
        roomHub = await StreamingHubClient.ConnectAsync<IRoomHub, IRoomHubReceiver>(channel, this);
    }

    /// <summary>
    /// MagicOnion�ؒf����
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
    /// �j������
    /// </summary>
    public async void OnDestroy()
    {
        DisconnectAsync();
    }

    /// <summary>
    /// ����
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
    /// �����ʒm(IRoomHubReceiver�C���^�[�t�F�C�X�̎���)
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
    /// �ގ�
    /// </summary>
    /// <returns></returns>
    public async UniTask LeaveAsync()
    {
        await roomHub.LeaveAsync();
    }

    /// <summary>
    /// �ގ��ʒm(IRoomHubReceiver�C���^�[�t�F�C�X�̎���)
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
    /// �ړ��A��]�ʒm
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
    /// �ړ��A��]
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
    /// ���[�U�[�̏���
    /// </summary>
    /// <returns></returns>
    public async UniTask ReadyAsync()
    {
        await roomHub.ReadyAsync();
    }

    /// <summary>
    /// ���������ʒm
    /// </summary>
    /// <param name="connectionId"></param>
    /// <param name="isReady"></param>
    public void OnReady(Guid connectionId, bool isReady)
    {
        OnReadyUser(connectionId, isReady);
    }

    /// <summary>
    /// �Q�[������������
    /// </summary>
    /// <param name="time"></param>
    /// <returns></returns>
    public async UniTask TimeAsync(float time)
    {
        await roomHub.TimeAsync(time);
    }

    /// <summary>
    /// �������Ԓʒm
    /// </summary>
    /// <param name="user"></param>
    /// <param name="time"></param>
    public void OnTimer(JoinedUser user, float time)
    {
        OnTime(user, time);
    }

    /// <summary>
    /// �S���N���L���������̏���
    /// </summary>
    /// <returns></returns>
    public async UniTask KillAsync()
    {
        await roomHub.KillAsync();
    }

    /// <summary>
    /// �S��������L����������ʒm
    /// </summary>
    /// <param name="connectionId"></param>
    /// <param name="totalKillNum"></param>
    /// <param name="userName"></param>
    public void OnKill(Guid connectionId, int totalKillNum, string userName)
    {
        OnKillNum(connectionId, totalKillNum, userName);
    }

    /// <summary>
    /// �}�b�`���O�̓���
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
    /// �}�b�`���O�ʒm
    /// </summary>
    /// <param name="roomName"></param>
    public void OnMatching(string roomName)
    {
        OnMatch(roomName);
    }

    /// <summary>
    /// �󔠂̈ʒu����
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
    /// �󔠂̈ʒu�̒ʒm
    /// </summary>
    /// <param name="pos"></param>
    /// <param name="rotaition"></param>
    /// <param name="Namechest"></param>
    public void OnMoveChest(Vector3 pos, Quaternion rotaition, string Namechest)
    {
        OnChest(pos, rotaition, Namechest);
    }

   /// <summary>
   /// �󔠂̎擾������
   /// </summary>
   /// <returns></returns>
    public async UniTask GainChest()
    {
        await roomHub.GainChest();
    }

    /// <summary>
    /// �󔠂̎擾�����v�̒ʒm
    /// </summary>
    /// <param name="TotalChestNum"></param>
    /// <param name="keyValuePairs"></param>
    public void OnChestNum(int TotalChestNum, Dictionary<string, int> keyValuePairs)
    {
        OnChestN(TotalChestNum, keyValuePairs);
    }

    /// <summary>
    /// �Q�[���I������
    /// </summary>
    /// <returns></returns>
    public async UniTask EndGameAsync()
    {
        await roomHub.EndGameAsync();
    }

    /// <summary>
    /// �Q�[���I���ʒm
    /// </summary>
    /// <param name="isHumanEndGame"></param>
    public void OnEndGame(bool isHumanEndGame,List<ResultData> resultData)
    {
        OnEndG(isHumanEndGame,resultData);
    }

}
