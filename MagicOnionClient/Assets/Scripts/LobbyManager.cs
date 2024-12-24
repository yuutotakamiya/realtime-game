using MagicOnionServer.Model.Entity;
using Shared.Interfaces.StreamingHubs;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

public class LobbyManager : MonoBehaviour
{
    [SerializeField] GameObject MachingPrefab;
    [SerializeField] RoomHubModel roomHubModel;
    [SerializeField] InputField userIdText;
    [SerializeField] Transform Content;

    static string roomName;

    //���[�������v���p�e�B
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

        JoinRoom();
   }

    // Update is called once per frame
    void Update()
    {
        
    }

    //�������鎞�ɌĂяo���֐�
    public async void JoinRoom()
    {
        //����
        await roomHubModel.JoinLobbyAsync(UserModel.Instance.userId);

    }

    //���[�U�[�������������̏���
    private void OnJoinedUser(JoinedUser user)
    {
        GameObject TextObject = Instantiate(MachingPrefab, Content);

        Text MachingText = TextObject.GetComponent<Text>();

        MachingText.text = $"ID:{user.UserData.Id},���O:{user.UserData.Name}";
    }

    //�}�b�`���O�����Ƃ��ɒʒm
    public void OnMaching(string roomName)
    {
        LobbyManager.roomName = roomName;

        Initiate.Fade("Game", Color.black, 1.0f);
    }


    //�ގ�����Ƃ��ɌĂяo���֐�
    public async void ExitRoom()
    {
        await roomHubModel.LeaveAsync();

        Initiate.Fade("Title", Color.black, 1.0f);
    }

    //���[�U�[���ގ������Ƃ��̏���
    private void OnExitUser(JoinedUser user)
    {
        // �ގ��������[�U�[�̃L�����N�^�[�I�u�W�F�N�g���폜
        /*if (GameDirector.Instance.characterList.ContainsKey(user.ConnectionId))
        {
            Destroy(GameDirector.Instance.characterList[user.ConnectionId]);  // �I�u�W�F�N�g��j��
            GameDirector.Instance.characterList.Remove(user.ConnectionId);    // ���X�g����폜
        }*/
    }
}
