//==========================================================
//
//�C���Q�[�����Ǘ�����X�N���v�g
//Author:���{�S��
//
//==========================================================
using Cinemachine;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using JetBrains.Annotations;
using MagicOnionServer.Model.Entity;
using Shared.Interfaces.StreamingHubs;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.TextCore.Text;
using UnityEngine.UI;
using static Shared.Interfaces.StreamingHubs.IRoomHubReceiver;

/// <summary>
/// �Q�[���S�̂��Ǘ����Ă���X�N���v�g
/// </summary>
public class GameDirector : MonoBehaviour
{
    //�L�����N�^�[�֌W
    [SerializeField] GameObject[] characterPrefab;//�L�����N�^�[��Prefab
    [SerializeField] GameObject[] startposition;//�ŏ��̃X�^�[�g�|�W�V����

    //�N���X�̐ݒ�
    [SerializeField] RoomHubModel roomHubModel;//RoomHubModel�̃N���X�̐ݒ�
    [SerializeField] DefenceTarget defenceTarget;//DefenceTarget�N���X�̐ݒ�

    //�J�E���g�_�E��
    [SerializeField] float timeLimit;//�������Ԃ�ݒ�
    [SerializeField] float currentTime;//���݂̃^�C��
    [SerializeField] int countdownTime;//�Q�[�����n�܂�O�̃J�E���g�_�E���ݒ�

    //UI
    [SerializeField] Text timerText;//�^�C�}�[Text
    [SerializeField] Text countdownText;//�J�E���g�_�E��Text
    [SerializeField] Text Crrenttext;//���݂̃L����
    [SerializeField] Text KillNum;//�L����
    [SerializeField] Text KillLog;//�L���ʒm
    [SerializeField] Text killerKakeru;//�~Text;
    [SerializeField] Text humanKakeru;//�~Text;
    [SerializeField] Text ChestNumText;//�󔠂̎擾������������Text
    [SerializeField] GameObject GameFinish;//�Q�[���I��Text
    [SerializeField] GameObject GameStartText;//�Q�[���X�^�[�gText
    [SerializeField] GameObject Result;//���U���g��ʂɍs�����߂̃{�^��
    [SerializeField] GameObject AttackButton1;//�f�t�H���g�̍U���{�^��
    [SerializeField] GameObject WinText;//
    [SerializeField] GameObject WinText2;//
    [SerializeField] Image skullIamge;//���W���̉摜
    [SerializeField] Image MiniMap;//�~�j�}�b�v
    [SerializeField] Image ChestImage;//�󔠂̉摜
    

    //virtualCamera�J�����̐錾
    private CinemachineVirtualCamera virtualCamera; // Cinemachine Virtual Camera

    //�t���O�֌W
    private bool isEnemy = false;//�������G���ǂ���
    private bool ishave = false;//�󔠂������Ă��邩�ǂ���

    
    Vector3 position;

    //�R���|�[�l���g�̐錾
    Animator animator;
    Rigidbody rigidbody;

    //�L�����N�^�[�̏���ۑ����邽�߂̃t�B�[���h
    private Dictionary<Guid, GameObject> characterList = new Dictionary<Guid, GameObject>();

    //���O�ƕ󔠂̖��O���t�B�[���h�ɕۑ�
    public static Dictionary<string,int> keyValuePairs;

    //�������g�̖��O���t�B�[���h�ɕۑ�
    private JoinedUser MyName; 

    //�������g���G���ǂ����𔻒f����t���O���v���p�e�B��
    public bool IsEnemy
    {
        get { return isEnemy; }
        set { isEnemy = value; }
    }

    /// <summary>
    /// �J�n����
    /// </summary>
    public async void Start()
    {
        //�ڑ�
        await roomHubModel.ConnectionAsync();

        //���[�U�[����������OnJoinedUser���\�b�h�����s����悤�A���f���ɓo�^
        roomHubModel.OnJoinedUser += this.OnJoinedUser;

        //���[�U�[���ޏo����OnLeave���\�b�h�����s����悤�A���f���ɓo�^
        roomHubModel.OnExitUser += this.OnExitUser;

        //���[�U�[���ړ������Ƃ���OnMoveCharacter���\�b�h�����s����悤�A���f���ɓo�^
        roomHubModel.OnMoveCharacter += this.OnMoveCharacter;

        //���[�����ɂ��郆�[�U�[���S����������������OnReady���\�b�h�����s����悤�A���f���ɓo�^
        roomHubModel.OnReadyUser += this.OnReady;

        //���[�����ɂ��郆�[�U�[�������������āA�Q�[�����J�n���ꂽ��OnTime���\�b�h�����s����悤�A���f���ɓo�^
        roomHubModel.OnTime += this.OnTimer;

        //���[�����ɂ��郆�[�U�[���S�ɃL�����ꂽ�Ƃ���OnKill���\�b�h�����s����悤�A���f���ɓo�^
        roomHubModel.OnKillNum += this.OnKill;

        //�󔠂��ړ������Ƃ���OnMoveChest���\�b�h�����s����悤�A���f���ɓo�^
        roomHubModel.OnChest += this.OnMoveChest;

        //�󔠂�S�Ď擾�����Ƃ���OnGainChest���\�b�h�����s����悤�A���f���ɓo�^
        roomHubModel.OnChestN += this.OnChestNum;

        //�Q�[�����I�������Ƃ���OnEndGame���\�b�h�����s����悤�A���f���ɓo�^
        roomHubModel.OnEndG += this.OnEndGame;

        currentTime = timeLimit; // ������: �c�莞�Ԃ�ݒ�

        animator = GetComponent<Animator>();//Animator�R���|�[�l���g�̎擾
        rigidbody = GetComponent<Rigidbody>();//Rigidbody�R���|�[�l���g�̎擾
        virtualCamera = FindObjectOfType<CinemachineVirtualCamera>();//CinemachineVirtualCamera�I�u�W�F�N�g�̎擾

        KillNum.text = "0";

        await JoinRoom();
        await UniTask.Delay(TimeSpan.FromSeconds(4.0f));  // �񓯊���4�b�ҋ@
        await Ready();
    }

    /// <summary>
    /// ���̃V�[���ɍs�����Ƃ��ɓo�^�������̂���������֐�
    /// </summary>
    private void OnDestroy()
    {
        //OnJoinedUser�ʒm�̓o�^����
        roomHubModel.OnJoinedUser -= this.OnJoinedUser;

        //OnLeave�ʒm�̓o�^����
        roomHubModel.OnExitUser -= this.OnExitUser;

        //OnMoveCharacter�ʒm�̓o�^����
        roomHubModel.OnMoveCharacter -= this.OnMoveCharacter;

        //OnReady�ʒm�̓o�^����
        roomHubModel.OnReadyUser -= this.OnReady;

        //OnTime�ʒm��o�^����
        roomHubModel.OnTime -= this.OnTimer;

        //OnKill�ʒm�̓o�^����
        roomHubModel.OnKillNum -= this.OnKill;

        //OnMoveChest�̒ʒm�o�^����
        roomHubModel.OnChest -= this.OnMoveChest;

        //OnGainChest�ʒm�̓o�^����
        roomHubModel.OnChestN -= this.OnChestNum;

        //OnEndGame�ʒm�̓o�^����
        roomHubModel.OnEndG -= this.OnEndGame;
    }

    /// <summary>
    /// �������鎞�ɌĂяo���֐�
    /// </summary>
    /// <returns></returns>
    public async UniTask JoinRoom()
    {
        //����
        await roomHubModel.JoinAsync(LobbyManager.RoomName, UserModel.Instance.UserID);

        InvokeRepeating("Move", 0.1f, 0.1f);
    }

    /// <summary>
    /// ���[�U�[�������������̏���
    /// </summary>
    /// <param name="user"></param>
    private void OnJoinedUser(JoinedUser user)
    {
        //�L�����N�^�[�𐶐�
        GameObject characterObject = Instantiate(characterPrefab[user.JoinOrder],
            startposition[user.JoinOrder].transform.position,
            startposition[user.JoinOrder].transform.rotation);

        //�������g�̐ڑ�ID��������������
        if (roomHubModel.ConnectionId == user.ConnectionId)
        {
            MyName=user;
            characterObject.GetComponent<Character>().Name(user.UserData.Name);
        }

       //roomHubModel�̐ڑ�ID�Ǝ������g�̐ڑ�ID��������������
        if (roomHubModel.ConnectionId == user.ConnectionId)
        {
            //JoinOrder��0��������
            if (user.JoinOrder == 0)
            {
                isEnemy = true;
                AttackButton1.SetActive(true);
                KillNum.gameObject.SetActive(true);
                skullIamge.gameObject.SetActive(true);
                MiniMap.gameObject.SetActive(true);
                killerKakeru.gameObject.SetActive(true);
            }
            else
            {
                ChestNumText.gameObject .SetActive(true);
                ChestImage.gameObject .SetActive(true);
                humanKakeru.gameObject.SetActive (true);
            }

            // �������ꂽ�L�����N�^�[��Cinemachine��Follow��Look At�^�[�Q�b�g�ɐݒ�
            Transform characterTransform = characterObject.transform;
            virtualCamera.Follow = characterTransform;//�L�����N�^�[�ɃJ�������t�H���[
            virtualCamera.LookAt = characterTransform;//�L�����N�^�[�ɃJ���������b�N
        }

        //�������g�̐ڑ�ID��roomHubModel�̐ڑ����ꏏ��������
        if (roomHubModel.ConnectionId == user.ConnectionId)
        {
            characterObject.GetComponent<Character>().Isself = true;
        }

        characterObject.transform.position = startposition[user.JoinOrder].transform.position;
        characterList[user.ConnectionId] = characterObject;//�t�B�[���h�ŕێ�
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
            CancelInvoke("Move");
        }

        // characterList���N���A
        characterList.Clear();

        // ������ConnectionId�����Z�b�g
        roomHubModel.ConnectionId = Guid.Empty;
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

    /// <summary>
    /// �L�����N�^�[�̈ʒu�����I�ɌĂяo�����\�b�h
    /// </summary>
    public async void Move()
    {
        //�������g��transform.position�AQuaternion.identity,�A�j���[�V�������T�[�o�[�ɑ��M
        await roomHubModel.MoveAsync(characterList[roomHubModel.ConnectionId].gameObject.transform.position,
            characterList[roomHubModel.ConnectionId].gameObject.transform.rotation,
           (CharacterState)characterList[roomHubModel.ConnectionId].GetComponent<Animator>().GetInteger("state"));
    }

    /// <summary>
    /// ���[�U�[�̈ړ��A��]�A�A�j���[�V����
    /// </summary>
    /// <param name="connectionId"></param>
    /// <param name="pos"></param>
    /// <param name="rotaition"></param>
    /// <param name="characterState"></param>
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
        }
    }

    /// <summary>
    /// ���[�U�[���������������������̃��\�b�h
    /// </summary>
    /// <returns></returns>
    public async UniTask Ready()
    {
        await roomHubModel.ReadyAsync();
    }

    /// <summary>
    /// ���[�����̃��[�U�[�S���������������Ă�����̏���
    /// </summary>
    /// <param name="connectionId"></param>
    /// <param name="isReady"></param>
    private void OnReady(Guid connectionId, bool isReady)
    {
        isReady = true;
        StartCoroutine(StartCountdown());
        StartCoroutine("Text");
    }

    /// <summary>
    /// �Q�[������������
    /// </summary>
    /// <param name="time"></param>
    public async void Time(float time)
    {
        await roomHubModel.TimeAsync(time);
    }

    /// <summary>
    /// ����I�ɌĂԃ��\�b�h(�Q�[������������)
    /// </summary>
    /// <param name="user"></param>
    /// <param name="time"></param>
    private async void OnTimer(JoinedUser user, float time)
    {
        currentTime = time;
        await Ready();
    }

    /// <summary>
    /// �L�������Ƃ��̃��\�b�h
    /// </summary>
    public async UniTask KillAsync()
    {
        await roomHubModel.KillAsync();
    }

    /// <summary>
    /// �L�������Ƃ��̒ʒm
    /// </summary>
    /// <param name="connectionId"></param>
    /// <param name="TotalKillNum"></param>
    /// <param name="userName"></param>
    public void OnKill(Guid connectionId, int TotalKillNum,string userName)
    {
        KillNum.text = TotalKillNum.ToString();//�L��������

        AnimateKillLog(userName);//�L�����O
    }

    /// <summary>
    /// �󔠂̈ʒu����
    /// </summary>
    /// <param name="pos"></param>
    /// <param name="rotaition"></param>
    /// <param name="Namechest"></param>
    public async UniTask MoveChest(Vector3 pos, Quaternion rotaition, string Namechest)
    {
        await roomHubModel.MoveChest(pos, rotaition, Namechest);
    }

    /// <summary>
    /// �󔠂̈ʒu�����I�ɒʒm���郁�\�b�h
    /// </summary>
    /// <param name="pos"></param>
    /// <param name="rotaition"></param>
    /// <param name="Namechest"></param>
    public void OnMoveChest(Vector3 pos, Quaternion rotaition, string Namechest)
    {
        GameObject chest = GameObject.Find(Namechest);//�󔠂̖��O������

        chest.transform.rotation = rotaition;//�󔠂̉�]����
        chest.transform.position = pos;//�󔠂̈ʒu����
    }

    /// <summary>
    /// �󔠂̎擾������
    /// </summary>
    /// <returns></returns>
    public async UniTask GainChest()
    {
        await roomHubModel.GainChest();//�󔠎擾����񓯊��ŌĂяo��
    }

    /// <summary>
    /// �󔠂̎擾���ʒm
    /// </summary>
    /// <param name="TotalChestNum"></param>
    /// <param name="keyValuePairs"></param>
    public async void OnChestNum(int TotalChestNum,Dictionary<string,int> keyValuePairs)
    {
        //�������g�̏ꍇ
        if (keyValuePairs.ContainsKey(MyName.UserData.Name))
        {
            ChestNumText.text = keyValuePairs[MyName.UserData.Name].ToString();//�󔠂̐���Text�ɑ��
        }

        //�󔠂����v2�擾������
        if (TotalChestNum == 2)
        {
           await EndGameAsync(true);//�Q�[���I��������񓯊��ŌĂяo��
        }
    }

    /// <summary>
    /// �Q�[���I������
    /// </summary>
    /// <returns></returns>
    public async UniTask EndGameAsync(bool isEndGame)
    {
        await roomHubModel.EndGameAsync(isEndGame);
    }

    /// <summary>
    /// �Q�[���I���ʒm
    /// </summary>
    /// <param name="isEndGame"></param>
    public async  void OnEndGame(bool isEndGame, List<ResultData> resultData)
    {
        ResultManager.SetResult(resultData);

        //�l�ԑ������������ꍇ
        if (isEndGame == true)
        {
            characterList[roomHubModel.ConnectionId].GetComponent<Character>().Isstart = false;
            WinText.SetActive(true);
            WinText2.SetActive(true);
            StopCoroutine(CountdownTimer());
            Invoke("LoadResult", 3);
        }
        //�ǂ������鑤�����������ꍇ
        else
        {
            characterList[roomHubModel.ConnectionId].GetComponent<Character>().Isstart = false;
            timerText.text = "0"; // 0�b�ɂȂ�����\��
            GameFinish.SetActive(true);
            Invoke("LoadResult", 3);
        }
    }

    /// <summary>
    /// ���U���g�V�[���֑J�ڂ���֐�
    /// </summary>
    public void LoadResult()
    {
        Initiate.Fade("Result", Color.black, 1);//Result�V�[���֑J��
    }

    /// <summary>
    /// DoTween���g�����L�����O�A�j���[�V����
    /// </summary>
    /// <param name="userName"></param>
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


    /// <summary>
    /// �J�E���g�_�E�����s�����\�b�h
    /// </summary>
    /// <returns></returns>
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
        characterList[roomHubModel.ConnectionId].GetComponent<Character>().Isstart = true;
        countdownText.gameObject.SetActive(false);
        StartCoroutine(HideGameStartText());

        // �Q�[���J�n
        StartCoroutine(CountdownTimer());
    }

    /// <summary>
    /// �Q�[���X�^�[�g�I�u�W�F�N�g����b��Ƀe�L�X�g���\���ɂ���֐�
    /// </summary>
    /// <returns></returns>
    private IEnumerator HideGameStartText()
    {
        yield return new WaitForSeconds(1.0f); // 1�b�ҋ@
        GameStartText.SetActive(false); // �Q�[���J�n���b�Z�[�W���\��
    }

    /// <summary>
    /// ��b��Ƀe�L�X�g������
    /// </summary>
    /// <returns></returns>
    private IEnumerator Text()
    {
        yield return new WaitForSeconds(1.0f);
        countdownText.text = "";
    }

    /// <summary>
    /// �^�C�}�[���J�E���g�_�E�����郁�\�b�h
    /// </summary>
    /// <returns></returns>
    public  IEnumerator CountdownTimer()
    {
        while (currentTime > 0)
        {
            timerText.text = currentTime.ToString(); // UI�Ƀ^�C�}�[��\��
            currentTime -= 1f; // 1�b���炷
            yield return new WaitForSeconds(1f); // 1�b�ҋ@
        }
    }

    /// <summary>
    /// �������Ԃ�0�ɂȂ�����
    /// </summary>
    private async void Timer()
    {
        if (currentTime == 0)
        {
          await  EndGameAsync(false);
        }
    }

    /// <summary>
    /// ���U���g�{�^���������ꂽ���̏���
    /// </summary>
    public void OnResult()
    {
        Initiate.Fade("Result", Color.black, 1);
    }

    /// <summary>
    /// �f�t�H���g�U���{�^���������ꂽ���̏���
    /// </summary>
    public void AttackButton()
    {
        characterList[roomHubModel.ConnectionId].GetComponent<Character>().AttackButton();
    }

    /// <summary>
    /// �������g�̐ڑ�ID���擾����֐�
    /// </summary>
    /// <returns></returns>
    public Character GetCharacter()
    {
        //�����̐ڑ�ID���擾
        Character foundCharacter = characterList[roomHubModel.ConnectionId].GetComponent<Character>();

        return foundCharacter;
    }
}

