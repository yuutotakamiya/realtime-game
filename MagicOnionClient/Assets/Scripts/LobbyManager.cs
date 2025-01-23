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
    //[SerializeField] GameObject MachingPrefab;//
    [SerializeField] RoomHubModel roomHubModel;//roomHubModel�N���X�̐ݒ�
    [SerializeField] GameObject MachingText;//�}�b�`���OText;
    [SerializeField] GameObject MachingIcon;//�}�b�`���O����Icon
    [SerializeField] GameObject[] MachingStartPositon;//�}�b�`���O�X�^�[�g�|�W�V����
    [SerializeField] GameObject[] characterPrefab;//�L�����N�^�[��Prefab

    static string roomName;//���[���������邽�߂̕ϐ�

    Dictionary<Guid, GameObject> characterList = new Dictionary<Guid, GameObject>();

    // �ő��JoinOrder�i�ő�v���C���[��4�l�j
    private int maxPlayers = 4;
    //���[�������v���p�e�B��
    public static string RoomName
    {
        get { return roomName; }
    }

    // Start is called before the first frame update
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
        await JoinRoom();
    }

    // Update is called once per frame
    void Update()
    {

    }

    //�������鎞�ɌĂяo���֐�
    public async UniTask JoinRoom()
    {
        //����
        await roomHubModel.JoinLobbyAsync(UserModel.Instance.userId);
    }


    //���[�U�[�������������̏���
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

    //�}�b�`���O�����Ƃ��ɒʒm���o������
    public async void OnMaching(string roomName)
    {
        LobbyManager.roomName = roomName;

        await UniTask.Delay(TimeSpan.FromSeconds(1.0f));  // �񓯊���1�b�ҋ@

        MachingIcon.SetActive(false);
        MachingText.SetActive(false);
        Initiate.Fade("Game", Color.black, 1.0f);
    }


    //�ގ�����Ƃ��ɌĂяo���֐�
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

    //���[�U�[���ގ������Ƃ��̏���
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
