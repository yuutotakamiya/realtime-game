using Shared.Interfaces.StreamingHubs;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameDirector : MonoBehaviour
{
    [SerializeField] GameObject characterPrefab;
    [SerializeField] RoomHubModel roomHubModel;
    Dictionary<Guid,GameObject> characterList = new Dictionary<Guid,GameObject>();
    // Start is called before the first frame update
  public async void Start()
  {
        //���[�U�[����������OnJoinedUser���\�b�h�����s����悤�A���f���ɓo�^���Ă���
        roomHubModel.OnJoinedUser += this.OnJoinedUser;
        //�ڑ�
        await roomHubModel.ConnectionAsync();
  }

    public async void JoinRoom()
    {
        //����
        await roomHubModel.JoinAsync("sampleroom", 1);
    }

    //���[�U�[�������������̏���
    private void OnJoinedUser(JoinedUser user)
    {
        GameObject characterObject = Instantiate(characterPrefab);//�C���X�^���X����
        characterObject.transform.position = new Vector3(0, 0, 0);
        characterList[user.ConnectionId] = characterObject;//�t�B�[���h�ŕێ�
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
