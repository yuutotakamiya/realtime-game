//==========================================================
//
//�}�b�`���O�̊Ǘ�����
//Author:���{�S��
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
///�@�}�b�`���O���Ǘ����Ă���X�N���v�g
/// </summary>
public class LobbyManager : MonoBehaviour
{
    [SerializeField] RoomHubModel roomHubModel;//roomHubModel�N���X�̐ݒ�
    [SerializeField] GameObject MachingText;//�}�b�`���OText;
    [SerializeField] GameObject MachingIcon;//�}�b�`���O����Icon
    [SerializeField] GameObject[] MachingStartPositon;//�}�b�`���O�X�^�[�g�|�W�V����
    [SerializeField] GameObject[] characterPrefab;//�L�����N�^�[��Prefab

    static string roomName;//���[���������邽�߂̕ϐ�

    Dictionary<Guid, GameObject> characterList = new Dictionary<Guid, GameObject>();

    //���[�������v���p�e�B��
    public static string RoomName
    {
        get { return roomName; }
    }

    /// <summary>
    /// ��ԍŏ��ɌĂ΂��֐�
    /// </summary>
    public async void Start()
    {
        //�ڑ�
        await roomHubModel.ConnectionAsync();

        //���[�U�[����������OnJoinedUser���\�b�h�����s����悤�A���f���ɓo�^���Ă���
        roomHubModel.OnJoinedUser += this.OnJoinedUser;

        //���[�����̐l����4�l�W�܂�����AOnMaching���\�b�h�����s����悤���f���ɓo�^
        roomHubModel.OnMatch += this.OnMaching;

        //���[�U�[���ޏo����OnLeave���\�b�h�����s����悤�A���f���ɓo�^���Ă���
        roomHubModel.OnExitUser += this.OnExitUser;

        MachingIcon.SetActive(true);
        MachingText.SetActive(true);
        await JoinRoom();//�񓯊���JoinRoom�֐����Ăяo��
    }

    private void OnDestroy()
    {
        //OnJoinedUser�ʒm�̂̓o�^����
        roomHubModel.OnJoinedUser -= this.OnJoinedUser;

        //OnMaching�ʒm�̓o�^����
        roomHubModel.OnMatch -= this.OnMaching;

        //OnLeave�ʒm�̓o�^����
        roomHubModel.OnExitUser -= this.OnExitUser;
    }

    /// <summary>
    /// �������鎞�ɌĂяo���֐�
    /// </summary>
    /// <returns></returns>
    public async UniTask JoinRoom()
    {
        //����
        await roomHubModel.JoinLobbyAsync(UserModel.Instance.userId);
    }


    /// <summary>
    /// ���[�U�[�������������̏���
    /// </summary>
    /// <param name="user"></param>
    private void OnJoinedUser(JoinedUser user)
    {
        //�L�����N�^�[�̐���
        GameObject Character = Instantiate(characterPrefab[user.JoinOrder],
          MachingStartPositon[user.JoinOrder].transform.position,
          MachingStartPositon[user.JoinOrder].transform.rotation);

        //�����̐ڑ�ID��roomHubModel�̐ڑ�ID�������Ȃ�
        if (roomHubModel.ConnectionId == user.ConnectionId)
        {
            //�����̖��O��\��
            Character.GetComponent<NameManager>().Name(user.UserData.Name);
        }

        Character.transform.position = MachingStartPositon[user.JoinOrder].transform.position;
        characterList[user.ConnectionId] = Character;//�t�B�[���h�ŕێ�

    }

    /// <summary>
    /// �}�b�`���O�����Ƃ��ɒʒm���o������
    /// </summary>
    /// <param name="roomName"></param>
    public async void OnMaching(string roomName)
    {
        LobbyManager.roomName = roomName;

        await UniTask.Delay(TimeSpan.FromSeconds(1.0f));  // �񓯊���1�b�ҋ@

        MachingIcon.SetActive(false);
        MachingText.SetActive(false);
        Initiate.Fade("Game", Color.black, 1.0f);
    }


    /// <summary>
    /// �ގ�����Ƃ��ɌĂяo���֐�
    /// </summary>
    public async void ExitRoom()
    {
        await roomHubModel.LeaveAsync();
        // �S�ẴL�����N�^�[�I�u�W�F�N�g���폜
        foreach (var entry in characterList)
        {
            Destroy(entry.Value);  // �L�����N�^�[�I�u�W�F�N�g��j��
        }

        // characterList���N���A
        characterList.Clear();

        // ������ConnectionId�����Z�b�g
        roomHubModel.ConnectionId = Guid.Empty;

        Initiate.Fade("Title", Color.black, 1.0f);
    }

    /// <summary>
    /// ���[�U�[���ގ������Ƃ��̏���
    /// </summary>
    /// <param name="user"></param>
    private void OnExitUser(JoinedUser user)
    {
        // �ގ��������[�U�[�̃L�����N�^�[�I�u�W�F�N�g���폜
        if (characterList.ContainsKey(user.ConnectionId))
        {
            Destroy(characterList[user.ConnectionId]);  // �I�u�W�F�N�g��j��
            characterList.Remove(user.ConnectionId);    // ���X�g����폜
        }
    }
}
