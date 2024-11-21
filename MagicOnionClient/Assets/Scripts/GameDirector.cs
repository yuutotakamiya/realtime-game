using MagicOnionServer.Model.Entity;
using Shared.Interfaces.StreamingHubs;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameDirector : MonoBehaviour
{
    [SerializeField] GameObject characterPrefab;//�L�����N�^�[��Prefab
    [SerializeField] RoomHubModel roomHubModel;//RoomHubModel�̃N���X�̐ݒ�
    [SerializeField] InputField InpuTuserId;//���[�U�[��Id�����
    [SerializeField] InputField roomName;//���[���̖��O�����
    [SerializeField] Text roomname;
    [SerializeField] GameObject startposition;
    Vector3 position;
    Dictionary <Guid,GameObject> characterList = new Dictionary<Guid,GameObject>();
    // Start is called before the first frame update
    public async void Start()
    {
        //���[�U�[����������OnJoinedUser���\�b�h�����s����悤�A���f���ɓo�^���Ă���
        roomHubModel.OnJoinedUser += this.OnJoinedUser;

        //���[�U�[���ޏo����OnLeave���\�b�h�����s����悤�A���f���ɓo�^���Ă���
        roomHubModel.OnExitUser += this.OnExitUser;

        //�ڑ�
        await roomHubModel.ConnectionAsync();
        
        position = startposition.transform.position;

        InpuTuserId = GameObject.Find("InputFielUserId").GetComponent<InputField>();
        roomname = roomname. GetComponent<Text>();
    }

    //�������鎞�ɌĂяo���֐�
    public async void JoinRoom()
    {
        //roomname = InpuTuserId.text;
        //����
        await roomHubModel.JoinAsync("sampleroom", 1);
    }

    //���[�U�[�������������̏���
    private void OnJoinedUser(JoinedUser user)
    {
        GameObject characterObject = Instantiate(characterPrefab);//�C���X�^���X����
        characterObject.transform.position = startposition.transform.position;
        characterList[user.ConnectionId] = characterObject;//�t�B�[���h�ŕێ�
    }

    //�ގ�����Ƃ��ɌĂяo���֐�
    public async void ExitRoom()
    {
        await roomHubModel.LeaveAsync();

        // �L�����N�^�[�I�u�W�F�N�g���폜
        if (characterList.ContainsKey(roomHubModel.ConnectionId))
        {
            Destroy(characterList[roomHubModel.ConnectionId]);
            characterList.Remove(roomHubModel.ConnectionId);
        }
    }

    //���[�U�[���ގ������Ƃ��̏���
    private void OnExitUser(JoinedUser user)
    {
        Destroy(characterList[user.ConnectionId].gameObject);
    }



    // Update is called once per frame
    void Update()
    {
        
    }


}
