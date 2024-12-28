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

    static string roomName;
    GameObject MachingIcon;
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

         MachingIcon = GameObject.Find("Spinner 1");

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
        GameObject TextObject = Instantiate(MachingPrefab, Content);

        Text MachingText = TextObject.GetComponent<Text>();

        MachingText.text = $"ID:{user.UserData.Id},���O:{user.UserData.Name}";
    }

    //�}�b�`���O�����Ƃ��ɒʒm
    public async void OnMaching(string roomName)
    {
        LobbyManager.roomName = roomName;

        await UniTask.Delay(TimeSpan.FromSeconds(1.0f));  // �񓯊���1�b�ҋ@

        MachingIcon.SetActive(false);
        Initiate.Fade("Game", Color.black, 1.0f);
    }


    //�ގ�����Ƃ��ɌĂяo���֐�
    public async void ExitRoom()
    {
        await roomHubModel.LeaveAsync();

        // �N���C�A���g����UI��I�u�W�F�N�g���N���A
        foreach (Transform child in Content)
        {
            Destroy(child.gameObject); // �v���C���[�̕\�����폜
        }

        Initiate.Fade("Title", Color.black, 1.0f);
    }

    //���[�U�[���ގ������Ƃ��̏���
    private void OnExitUser(JoinedUser user)
    {
        // �ގ��������[�U�[�ɑΉ�����I�u�W�F�N�g���폜
        if (user.ConnectionId == roomHubModel.ConnectionId)
        {
            foreach (Transform child in Content)
            {
                if (child.GetComponent<Text>().text.Contains(user.UserData.Name))
                {
                    Destroy(child.gameObject); // �v���C���[�̕\�����폜
                    break;
                }
            }
        }
    }
}
