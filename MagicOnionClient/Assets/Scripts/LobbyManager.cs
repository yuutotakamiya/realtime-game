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
    [SerializeField] GameObject MachingPrefab;
    [SerializeField] RoomHubModel roomHubModel;
    [SerializeField] InputField userIdText;
    [SerializeField] Transform Content;
    [SerializeField] GameObject MachingText;
    [SerializeField] GameObject MachingIcon;
    [SerializeField] GameObject[] MachingStartPositon;
    [SerializeField] GameObject[] characterPrefab;

    static string roomName;

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

        GameObject Character = Instantiate(characterPrefab[user.JoinOrder],
          MachingStartPositon[user.JoinOrder].transform.position,
          MachingStartPositon[user.JoinOrder].transform.rotation);

        Character.GetComponent<Character>().Name(user.UserData.Name);
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

        Initiate.Fade("Title", Color.black, 1.0f);
    }

    //���[�U�[���ގ������Ƃ��̏���
    private void OnExitUser(JoinedUser user)
    {
        // �ގ��������[�U�[�̃L�����N�^�[�I�u�W�F�N�g���폜
        if (user.ConnectionId==roomHubModel.ConnectionId)
        {
            Destroy(this.gameObject);  // �I�u�W�F�N�g��j��
        }
    }
}
