using Cinemachine;
using DG.Tweening;
using MagicOnionServer.Model.Entity;
using Shared.Interfaces.StreamingHubs;
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.TextCore.Text;
using UnityEngine.UI;
using static Shared.Interfaces.StreamingHubs.IRoomHubReceiver;
public class GameDirector : MonoBehaviour
{
    [SerializeField] GameObject[] characterPrefab;//�L�����N�^�[��Prefab
    [SerializeField] RoomHubModel roomHubModel;//RoomHubModel�̃N���X�̐ݒ�
    [SerializeField] HumanManager humanManager;//HumanManager�̃N���X�̐ݒ�
    [SerializeField] InputField InpuTuserId;//���[�U�[��Id�����
    [SerializeField] InputField roomName;//���[���̖��O�����
    [SerializeField] Text roomname;
    [SerializeField] Text userId;
    [SerializeField] GameObject[] startposition;
    [SerializeField] Text timerText;
    [SerializeField] public float timeLimit;
    [SerializeField] float currentTime;
    [SerializeField] int countdownTime;
    [SerializeField] Text countdownText;
    [SerializeField] public GameObject GameFinish;
    [SerializeField] GameObject GameStartText;
    [SerializeField] GameObject Result;
    [SerializeField] Text Crrenttext;//���݂̃L����
    [SerializeField] Text KillNum;//�L����
    [SerializeField] Text KillLog;//�L���ʒm
    [SerializeField] GameObject AttackButton1;
    [SerializeField] GameObject AttackButton2;
    private CinemachineVirtualCamera virtualCamera; // Cinemachine Virtual Camera

    private bool isEnemy = false;//�������G���ǂ���

    Vector3 position;
    Animator animator;
    Rigidbody rigidbody;
    Character character;
    Dictionary<Guid, GameObject> characterList = new Dictionary<Guid, GameObject>();
    public bool IsEnemy
    {
        get { return isEnemy; }
        set { isEnemy = value; }
    }
    public async void Start()
    {
        //���[�U�[����������OnJoinedUser���\�b�h�����s����悤�A���f���ɓo�^���Ă���
        roomHubModel.OnJoinedUser += this.OnJoinedUser;

        //���[�U�[���ޏo����OnLeave���\�b�h�����s����悤�A���f���ɓo�^���Ă���
        roomHubModel.OnExitUser += this.OnExitUser;

        //���[�U�[���ړ������Ƃ���OnMoveCharacter���\�b�h�����s����悤�A���f���ɓo�^
        roomHubModel.OnMoveCharacter += this.OnMoveCharacter;

        //���[�����ɂ��郆�[�U�[���S����������������OnReady���\�b�h�����s����悤�A���f���ɓo�^
        roomHubModel.OnReadyUser += this.OnReady;

        //���[�����ɂ��郆�[�U�[�������������āA�Q�[�����J�n���ꂽ��OnTime���\�b�h�����s����悤�A���f���ɓo�^
        roomHubModel.OnTime += this.OnTimer;

        //���[�����ɂ��郆�[�U�[���S�ɃL�����ꂽ�Ƃ���OnKill���\�b�h�����s����悤�A���f���ɓo�^���Ă���
        roomHubModel.OnKillNum += this.OnKill;

        //�}�b�`���O�����Ƃ��AOnMaching���\�b�h�����s����悤�A���f���ɓo�^
        roomHubModel.OnMatchi += this.OnMaching;

        //�ڑ�
        await roomHubModel.ConnectionAsync();

        //position = startposition.transform.position;

        InpuTuserId = GameObject.Find("InputFielUserId").GetComponent<InputField>();
        roomname = roomname.GetComponent<Text>();

        currentTime = timeLimit; // ������: �c�莞�Ԃ�ݒ�

        animator = GetComponent<Animator>();

        rigidbody = GetComponent<Rigidbody>();

        virtualCamera = FindObjectOfType<CinemachineVirtualCamera>();

        KillNum.text = "0";
    }

    //�������鎞�ɌĂяo���֐�
    public async void JoinRoom()
    {
        //roomname = InpuTuserId.text;
        //����
        await roomHubModel.JoinAsync(roomname.text, int.Parse(userId.text));

        InvokeRepeating("Move", 0.1f, 0.1f);
    }

    //���[�U�[�������������̏���
    private void OnJoinedUser(JoinedUser user)
    {

        GameObject characterObject = Instantiate(characterPrefab[user.JoinOrder], startposition[user.JoinOrder].transform.position, startposition[user.JoinOrder].transform.rotation);//Prefab�𐶐�

        // �������ꂽ�L�����N�^�[��Cinemachine��Follow��Look At�^�[�Q�b�g�ɐݒ�
        if (roomHubModel.ConnectionId == user.ConnectionId)
        {
            //JoinOrder��0��������
            if (user.JoinOrder == 0)
            {
                isEnemy = true;
                AttackButton1.SetActive(true);
                AttackButton2.SetActive(true);
                KillNum.gameObject.SetActive(true);
                Crrenttext.gameObject.SetActive(true);
                

            }
            else
            {
                AttackButton1.SetActive(false);
                AttackButton2.SetActive(false);
                KillNum.gameObject.SetActive(false);
                Crrenttext.gameObject.SetActive(false);
            }
            Transform characterTransform = characterObject.transform;
            virtualCamera.Follow = characterTransform;
            virtualCamera.LookAt = characterTransform;
        }

        if (roomHubModel.ConnectionId == user.ConnectionId)
        {
            characterObject.GetComponent<Character>().isself = true;
        }

        characterObject.transform.position = startposition[user.JoinOrder].transform.position;
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
            CancelInvoke("Move");
        }

        // characterList���N���A
        characterList.Clear();

        // ������ConnectionId�����Z�b�g
        roomHubModel.ConnectionId = Guid.Empty;
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
    public async void Move()
    {
        //�������g��transform.position�AQuaternion.identity,�A�j���[�V�������T�[�o�[�ɑ��M
        await roomHubModel.MoveAsync(characterList[roomHubModel.ConnectionId].gameObject.transform.position,
            characterList[roomHubModel.ConnectionId].gameObject.transform.rotation,
           (CharacterState)characterList[roomHubModel.ConnectionId].GetComponent<Animator>().GetInteger("state"));
    }

    //���[�U�[�̈ړ��A��]�A�A�j���[�V����
    private void OnMoveCharacter(Guid connectionId, Vector3 pos, Quaternion rotaition, CharacterState characterState)
    {
        if (characterList.ContainsKey(connectionId))
        {
            GameObject character = characterList[connectionId];

            // �L�����N�^�[�̈ʒu�Ɖ�]���T�[�o�[�ɍX�V
            character.transform.DOLocalMove(pos, 0.1f).SetEase(Ease.Linear);
            character.transform.DORotate(rotaition.eulerAngles, 0.1f).SetEase(Ease.Linear);

            //�L�����N�^�[�̃A�j���[�V����
            Animator animator = character.GetComponent<Animator>();

            animator.SetInteger("state", (int)characterState);

            Debug.Log(characterState);
        }
    }

    //���[�U�[���������������������̃��\�b�h
    public async void Ready()
    {
        await roomHubModel.ReadyAsync();
    }

    //���[�����̃��[�U�[�S���������������������烆�[�U�[���������������Ƃ��̏���
    private void OnReady(Guid connectionId, bool isReady)
    {
        characterList[roomHubModel.ConnectionId].GetComponent<Character>().isstart = true;

        StartCoroutine(StartCountdown());

        StartCoroutine("Text");
    }

    //�Q�[������������
    public async void TimeAsync(float time)
    {
        await roomHubModel.TimeAsync(time);
    }

    //����I�ɌĂԃ��\�b�h
    private void OnTimer(Guid connectionId, float time)
    {
        currentTime = time;

        StartCoroutine("CountdownTimer");
    }

    //�L�������Ƃ��̃��\�b�h
    public async void KillAsync()
    {
        await roomHubModel.KillAsync();
    }

    //�L�������Ƃ��̒ʒm
    public void OnKill(Guid connectionId, int TotalKillNum,string userName)
    {
        KillNum.text = TotalKillNum.ToString();

        AnimateKillLog(userName);

    }

    //�}�b�`���O��������
    public async void JoinLobbyAsync(int userId)
    {
        await roomHubModel.JoinLobbyAsync(userId);
    }

    //�}�b�`���O�����Ƃ��ɒʒm
    public void OnMaching(string roomName)
    {

    }

    //DoTween���g�����L�����O�A�j���[�V����
    private void AnimateKillLog(string userName)
    {
        // KillLog �e�L�X�g�̏����ʒu��ۑ�
        Vector3 initialPosition = KillLog.transform.localPosition;

        // ���O��F�t���ŕ\���i��: ���O��ԐF�Łj
        string KillMessage = "<color=red>" + userName + "</color>" + "���E����܂���";

        // KillLog�Ƀ��b�Z�[�W��ǉ�
        KillLog.text += KillMessage + "\n";

        // KillLog �S�̂̃t�F�[�h�C���A�j���[�V����
        KillLog.DOFade(1f, 0.5f)  // �t�F�[�h�C��
            .SetEase(Ease.OutQuad)  // �C�[�W���O��ݒ�
            .OnComplete(() =>
            {
                // �t�F�[�h�C����A�����ҋ@���Ă��� KillLog �S�̂��t�F�[�h�A�E�g
                KillLog.DOFade(0f, 1f)  // �t�F�[�h�A�E�g
                    .SetDelay(2f)  // 2�b��Ƀt�F�[�h�A�E�g�J�n
                    .SetEase(Ease.InQuad)
                    .OnComplete(() =>
                    {
                        // �t�F�[�h�A�E�g���KillLog�����Z�b�g
                        KillLog.text = ""; // KillLog�̃e�L�X�g������
                    });
            });

        // KillLog �e�L�X�g����ɃX���C�h������A�j���[�V����
        KillLog.transform.DOLocalMoveY(initialPosition.y + 50f, 0.5f)
            .SetEase(Ease.OutQuad)
            .OnComplete(() =>
            {
                // �X���C�h�A�j���[�V������A���̈ʒu�ɖ߂�
                KillLog.transform.DOLocalMoveY(initialPosition.y, 0.5f)
                    .SetEase(Ease.InQuad);
            });
    }


    // �J�E���g�_�E�����s�����\�b�h
    private IEnumerator StartCountdown()
    {
        countdownTime = 3; //3�b�̃J�E���g�_�E��
        while (countdownTime > 0)
        {
            countdownText.text = countdownTime.ToString(); // �J�E���g�_�E����\��

            // ���o: �������g�債�āA�\������
            countdownText.transform.DOScale(1.5f, 0.5f).SetEase(Ease.OutBounce); // �g��i0.5�b�Łj
            countdownText.color = Color.red; // ������ԂɕύX

            yield return new WaitForSeconds(0.5f); // 0.5�b�ԑҋ@�i�g��\������鎞�ԁj

            // ���̏�Ԃɖ߂�
            countdownText.transform.DOScale(1f, 0.3f).SetEase(Ease.OutBounce); // �k���i0.3�b�Łj
            countdownText.color = Color.white; // �F�𔒂ɖ߂�

            yield return new WaitForSeconds(0.5f); // �������\������鎞��

            countdownTime--; // ���̐����ɐi��  
        }

        GameStartText.SetActive(true); 
        countdownText.gameObject.SetActive(false);
        StartCoroutine(HideGameStartText());

        // �Q�[���J�n
        StartCoroutine(CountdownTimer());
    }

    //�Q�[���X�^�[�g�I�u�W�F�N�g����b��Ƀe�L�X�g���\���ɂ���֐�
    private IEnumerator HideGameStartText()
    {
        yield return new WaitForSeconds(1.0f); // 1�b�ҋ@
        GameStartText.SetActive(false); // �Q�[���J�n���b�Z�[�W���\��
    }
    //��b��Ƀe�L�X�g������
    private IEnumerator Text()
    {
        yield return new WaitForSeconds(1.0f);

        countdownText.text = "";
    }

    // �^�C�}�[���J�E���g�_�E�����郁�\�b�h
    public IEnumerator CountdownTimer()
    {
        while (currentTime > 0)
        {
            timerText.text = currentTime.ToString(); // UI�Ƀ^�C�}�[��\��
            currentTime -= 1f; // 1�b���炷
            yield return new WaitForSeconds(1f); // 1�b�ҋ@
        }

        if (currentTime == 0)
        {
            characterList[roomHubModel.ConnectionId].GetComponent<Character>().isstart = false;
            timerText.text = "0"; // 0�b�ɂȂ�����\��
            GameFinish.SetActive(true);
            Result.SetActive(true);
            //Initiate.Fade("Result",Color.black,1);
        }
    }

    //���U���g�{�^���������ꂽ���̏���
    public void OnResult()
    {
        Initiate.Fade("Result",Color.black,1);
    }

    //�{�^���������ꂽ���̏���
    public void AttackButton()
    {
        characterList[roomHubModel.ConnectionId].GetComponent<Character>().AttackButton();
    }

   
    // Update is called once per frame
    void Update()
    {


    }
}
