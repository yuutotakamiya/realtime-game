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
    [SerializeField] GameObject characterPrefab;//�L�����N�^�[��Prefab
    [SerializeField] RoomHubModel roomHubModel;//RoomHubModel�̃N���X�̐ݒ�
    [SerializeField] InputField InpuTuserId;//���[�U�[��Id�����
    [SerializeField] InputField roomName;//���[���̖��O�����
    [SerializeField] Text roomname;
    [SerializeField] Text userId;
    [SerializeField] GameObject startposition;
    //[SerializeField] float speed = 3.0f;
    Vector3 position;
    Dictionary <Guid,GameObject> characterList = new Dictionary<Guid,GameObject>();
    // Start is called before the first frame update
    public async void Start()
    {
        //���[�U�[����������OnJoinedUser���\�b�h�����s����悤�A���f���ɓo�^���Ă���
        roomHubModel.OnJoinedUser += this.OnJoinedUser;

        //���[�U�[���ޏo����OnLeave���\�b�h�����s����悤�A���f���ɓo�^���Ă���
        roomHubModel.OnExitUser += this.OnExitUser;

        //���[�U�[���ړ������Ƃ���OnMoveCharacter���\�b�h�����s����悤�A���f���ɓo�^
        roomHubModel.OnMoveCharacter += this.OnMoveCharacter;

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
        await roomHubModel.JoinAsync("sampleroom",int.Parse(userId.text));

        InvokeRepeating("Move", 0.1f, 0.1f);
      
    }

    //���[�U�[�������������̏���
    private void OnJoinedUser(JoinedUser user)
    {
        GameObject characterObject = Instantiate(characterPrefab);//�C���X�^���X����
        if (roomHubModel.ConnectionId == user.ConnectionId)
        {
            characterObject.GetComponent<Move>().isself = true;
        }
        characterObject.transform.position = startposition.transform.position;
        characterList[user.ConnectionId] = characterObject;//�t�B�[���h�ŕێ�
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

        CancelInvoke("Move");
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

    //����I�ɌĂяo�����\�b�h
    private async void Move()
    {
        //�������g��transform.position�AQuaternion.identity���T�[�o�[�ɑ��M
        await roomHubModel.MoveAsync(characterList[roomHubModel.ConnectionId].gameObject.transform.position, characterList[roomHubModel.ConnectionId].gameObject.transform.rotation);
    }

    //���[�U�[�̈ړ��A��]
    private void OnMoveCharacter(Guid connectionId, Vector3 pos,Quaternion rotaition)
    {

        if (characterList.ContainsKey(connectionId))
        {
            GameObject character = characterList[connectionId];

            // �L�����N�^�[�̈ʒu�Ɖ�]���T�[�o�[�̒l�ɍX�V
            character.transform.DOLocalMove(pos, 0.1f).SetEase(Ease.Linear);
            character.transform.DORotate(rotaition.eulerAngles, 0.1f).SetEase(Ease.Linear);
        }

    }
    // Update is called once per frame
    void Update()
    {
     
    }
    
}
