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
public class GameDirector : MonoBehaviour
{
    //�L�����N�^�[�֌W
    [SerializeField] GameObject[] characterPrefab;//�L�����N�^�[��Prefab
    [SerializeField] GameObject[] startposition;//�ŏ��̃X�^�[�g�|�W�V����

    //�N���X�̐ݒ�
    [SerializeField] RoomHubModel roomHubModel;//RoomHubModel�̃N���X�̐ݒ�
    [SerializeField] DefenceTarget defenceTarget;

    //�J�E���g�_�E��
    [SerializeField] public float timeLimit;//�������Ԃ�ݒ�
    [SerializeField] float currentTime;//���݂̃^�C��
    [SerializeField] int countdownTime;//�Q�[�����n�܂�O�̃J�E���g�_�E���ݒ�

    //UI
    [SerializeField] Text timerText;//�^�C�}�[Text
    [SerializeField] Text countdownText;//�J�E���g�_�E��Text
    [SerializeField] Text Crrenttext;//���݂̃L����
    [SerializeField] Text KillNum;//�L����
    [SerializeField] Text KillLog;//�L���ʒm
    [SerializeField] Image skullIamge;//���W���̉摜
    [SerializeField] Text killerKakeru;//�~Text;
    [SerializeField] Text humanKakeru;//�~Text;
    [SerializeField] public GameObject GameFinish;//�Q�[���I��Text
    [SerializeField] GameObject GameStartText;//�Q�[���X�^�[�gText
    [SerializeField] GameObject Result;//���U���g��ʂɍs�����߂̃{�^��
    [SerializeField] GameObject AttackButton1;//�f�t�H���g�̍U���{�^��
    [SerializeField] GameObject AttackButton2;//���U��
    [SerializeField] Image MiniMap;//�~�j�}�b�v
    [SerializeField] Text ChestNumText;//�󔠂̎擾������������Text
    [SerializeField] GameObject WinText;
    [SerializeField] GameObject WinText2;
    [SerializeField] Image ChestImage;

    /*[SerializeField] public GameObject openButton;
    [SerializeField] public GameObject closeButton;
    /*[SerializeField] public GameObject holdButton;
    [SerializeField] public GameObject notholdButton;
    [SerializeField] public GameObject placeButton;*/

    private CinemachineVirtualCamera virtualCamera; // Cinemachine Virtual Camera

    private bool isEnemy = false;//�������G���ǂ���
    private bool ishave = false;//�󔠂������Ă��邩�ǂ���

    Vector3 position;
    Animator animator;
    Rigidbody rigidbody;
    Character character;

    private Dictionary<Guid, GameObject> characterList = new Dictionary<Guid, GameObject>();

    public static Dictionary<string,int> keyValuePairs;//���O�ƕ󔠂̖��O���t�B�[���h�ɕۑ�

    private JoinedUser MyName; //�������g�̖��O���t�B�[���h�ɕۑ�

    //�������g���G���ǂ����̃v���p�e�B
    public bool IsEnemy
    {
        get { return isEnemy; }
        set { isEnemy = value; }
    }
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

        animator = GetComponent<Animator>();
        rigidbody = GetComponent<Rigidbody>();
        virtualCamera = FindObjectOfType<CinemachineVirtualCamera>();

        KillNum.text = "0";

        await JoinRoom();
        await UniTask.Delay(TimeSpan.FromSeconds(4.0f));  // �񓯊���4�b�ҋ@
        await Ready();
    }

    //�������鎞�ɌĂяo���֐�
    public async UniTask JoinRoom()
    {
        //����
        await roomHubModel.JoinAsync(LobbyManager.RoomName, UserModel.Instance.userId);

        InvokeRepeating("Move", 0.1f, 0.1f);
    }

    //���[�U�[�������������̏���
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
                //AttackButton2.SetActive(true);
                KillNum.gameObject.SetActive(true);
                //Crrenttext.gameObject.SetActive(true);
                skullIamge.gameObject.SetActive(true);
                MiniMap.gameObject.SetActive(true);
                killerKakeru.gameObject.SetActive(true);
                ChestNumText.gameObject.SetActive(false);
                ChestImage.gameObject.SetActive(false);
                humanKakeru.gameObject.SetActive (false);

            }
            else
            {
                AttackButton1.SetActive(false);
                AttackButton2.SetActive(false);
                KillNum.gameObject.SetActive(false);
                Crrenttext.gameObject.SetActive(false);
                skullIamge.gameObject .SetActive(false);
                MiniMap.gameObject .SetActive(false);
                killerKakeru.gameObject .SetActive(false);
                ChestNumText.gameObject .SetActive(true);
                ChestImage.gameObject .SetActive(true);
                humanKakeru.gameObject.SetActive (true);
            }

            // �������ꂽ�L�����N�^�[��Cinemachine��Follow��Look At�^�[�Q�b�g�ɐݒ�
            Transform characterTransform = characterObject.transform;
            virtualCamera.Follow = characterTransform;//�L�����N�^�[�ɃJ�������t�H���[
            virtualCamera.LookAt = characterTransform;//�L�����N�^�[�ɃJ���������b�N
        }

        if (roomHubModel.ConnectionId == user.ConnectionId)
        {
            characterObject.GetComponent<Character>().Isself = true;
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

    //�L�����N�^�[�̈ʒu�����I�ɌĂяo�����\�b�h
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

            //Debug.Log(characterState);
        }
    }

    //���[�U�[���������������������̃��\�b�h
    public async UniTask Ready()
    {
        await roomHubModel.ReadyAsync();
    }

    //���[�����̃��[�U�[�S���������������Ă�����̏���
    private void OnReady(Guid connectionId, bool isReady)
    {
        isReady = true;
        StartCoroutine(StartCountdown());
        StartCoroutine("Text");
    }

    //�Q�[������������
    public async void Time(float time)
    {
        await roomHubModel.TimeAsync(time);
    }

    //����I�ɌĂԃ��\�b�h(�Q�[������������)
    private async void OnTimer(JoinedUser user, float time)
    {
        currentTime = time;
        await Ready();
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

    //�󔠂̈ʒu����
    public async void MoveChest(Vector3 pos, Quaternion rotaition, string Namechest)
    {
        await roomHubModel.MoveChest(pos, rotaition, Namechest);
    }

    //�󔠂̈ʒu�����I�ɒʒm���郁�\�b�h
    public void OnMoveChest(Vector3 pos, Quaternion rotaition, string Namechest)
    {
        GameObject chest = GameObject.Find(Namechest);

        chest.transform.rotation = rotaition;
        chest.transform.position = pos;
    }

    //�󔠂̎擾������
    public async UniTask GainChest()
    {
        await roomHubModel.GainChest();
    }

    //�󔠂̎擾���ʒm
    public async void OnChestNum(int TotalChestNum,Dictionary<string,int> keyValuePairs)
    {
        if (keyValuePairs.ContainsKey(MyName.UserData.Name))
        {
            ChestNumText.text = keyValuePairs[MyName.UserData.Name].ToString();
        }

        //�󔠂����v2�擾������
        if (TotalChestNum == 2)
        {
           await EndGameAsync();
        }
    }

    //�Q�[���I������
    public async UniTask EndGameAsync()
    {
        await roomHubModel.EndGameAsync();
    }

    //�Q�[���I���ʒm
    public async void OnEndGame(bool isHumanEndGame)
    {
        if (isHumanEndGame==true)
        {
            characterList[roomHubModel.ConnectionId].GetComponent<Character>().Isstart = false;
            WinText.SetActive(true);
            WinText2.SetActive(true);
            StopCoroutine("CountdownTimer");
            Invoke("LoadResult", 3);
        }
        else
        {
            characterList[roomHubModel.ConnectionId].GetComponent<Character>().Isstart = false;
            timerText.text = "0"; // 0�b�ɂȂ�����\��
            GameFinish.SetActive(true);
            //await UniTask.Delay(TimeSpan.FromSeconds(4.0f));  // �񓯊���4�b�ҋ@
            Invoke("LoadResult", 3);
        }
    }

    public void LoadResult()
    {
        Initiate.Fade("Result", Color.black, 1);
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
        characterList[roomHubModel.ConnectionId].GetComponent<Character>().Isstart = true;
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
    public  IEnumerator CountdownTimer()
    {
        while (currentTime > 0)
        {
            timerText.text = currentTime.ToString(); // UI�Ƀ^�C�}�[��\��
            currentTime -= 1f; // 1�b���炷
            yield return new WaitForSeconds(1f); // 1�b�ҋ@
        }

        if (currentTime == 0)
        {
            EndGameAsync();
        }
    }

    //���U���g�{�^���������ꂽ���̏���
    public void OnResult()
    {
        Initiate.Fade("Result", Color.black, 1);
    }

    //�f�t�H���g�U���{�^���������ꂽ���̏���
    public void AttackButton()
    {
        characterList[roomHubModel.ConnectionId].GetComponent<Character>().AttackButton();
    }

    //�������g�̐ڑ�ID���擾
    public Character GetCharacter()
    {
        Character foundCharacter = characterList[roomHubModel.ConnectionId].GetComponent<Character>();

        return foundCharacter;
    }

    public void LightningAttak()
    {
        characterList[roomHubModel.ConnectionId].GetComponent<Character>().LightningAttack();
    }

    // Update is called once per frame
    void Update()
    {

    }
}

