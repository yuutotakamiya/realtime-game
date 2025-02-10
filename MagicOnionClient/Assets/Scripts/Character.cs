//==========================================================
//
//�L�����N�^�[���Ǘ�����
//Author:���{�S��
//
//==========================================================
using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;
using static Shared.Interfaces.StreamingHubs.IRoomHubReceiver;

/// <summary>
/// �L�����N�^�[���Ǘ����Ă���X�N���v�g
/// </summary>
public class Character : MonoBehaviour
{
    [SerializeField] float speed;//�ړ��̃X�s�[�h�̐ݒ�
    [SerializeField] float rotateSpeed;//��]�̐ݒ�
    [SerializeField] float AttckCoolDown;//�U���̃N�[���_�E��
    [SerializeField] public Text NameText;//���OText
    [SerializeField] AudioClip AttackSE;//�U��SE
    [SerializeField] Collider collider;//�U���̓����蔻��

    /// <summary>
    /// �t���O�֌W
    /// </summary>
    protected bool isDead = false;//����ł���ǂ���
    protected bool isself = false;//�������g���ǂ���
    protected bool isstart = false;//�����������Ă��邩�ǂ���
    protected bool isAttack = false;//�U�������ǂ���
    protected bool hasTreasure = false;//�󔠂������Ă��邩�ǂ����̃t���O


    /// <summary>
    /// �N���X�̐錾
    /// </summary>
    protected FixedJoystick joystick;
    protected Rigidbody rb;
    protected Animator animator;
    protected RoomHubModel roomHubModel;
    protected GameDirector gameDirector;
    protected DefenceTarget defenceTarget;
    protected CinemachineVirtualCamera virtualCamera;
    private AudioSource audioSource;
    public GameObject currentTreasureChest;//���݈��������Ă����

    //�������g���ǂ����̃t���O�̃v���p�e�B
    public bool Isself
    {
        set { isself = value; }
        get { return isself; }
    }

    //����ł��邩�ǂ����̃t���O�̃v���p�e�B
    public bool IsDead
    {
        get { return isDead; }
        set { isDead = value; }
    }

    //�U�����Ă��邩�ǂ����̃t���O�̃v���p�e�B
    public bool IsAttack
    {
        get { return isAttack; }
        set { isAttack = value; }
    }

    //�����������Ă��邩�ǂ����̃t���O�̃v���p�e�B
    public bool Isstart
    {
        get { return isstart; }
        set { isstart = value; }
    }
    //�󔠂����Ɏ����Ă��邩�ǂ����̃t���O�̃v���p�e�B
    public bool HasTreasure
    {
        get { return hasTreasure; }
        set { hasTreasure = value; }
    }

    /// <summary>
    /// ��ԍŏ��ɌĂ΂��֐�
    /// </summary>
    public virtual void Start()
    {
        audioSource = GetComponent<AudioSource>();
        rb = GetComponent<Rigidbody>();
        joystick = GameObject.Find("Fixed Joystick").GetComponent<FixedJoystick>();
        animator = GetComponent<Animator>();
        roomHubModel = GameObject.Find("RoomModel").GetComponent<RoomHubModel>();
        gameDirector = GameObject.Find("GameDirector").GetComponent<GameDirector>();
        defenceTarget = GameObject.Find("DefenceTarget").GetComponent<DefenceTarget>();

        //defenceTarget��null�������Ƃ��������Ȃ�
        if (defenceTarget == null)
        {
            return;
        }
        //joystick��null�Ȃ牽�����Ȃ�
        if (joystick == null)
        {
            return;
        }
        //�R���C�_�[��null�Ȃ牽�����Ȃ�
        if (collider == null)
        {
            return;
        }
        //rigidbody��null�������牽�����Ȃ�
        if (rb == null)
        {
            return;
        }
        //gameDirecter��null�������牽�����Ȃ�
        if (gameDirector == null)
        {
            return;
        }
        //defenceTarget��null�������牽�����Ȃ�
        if (defenceTarget == null)
        {
            return;
        }
       
        collider.enabled = false;
    }

    /// <summary>
    /// ���t���[���Ă΂��֐�
    /// </summary>
    public virtual async void Update()
    {
        if (Isstart == true)
        {
            Vector3 move = (Camera.main.transform.forward * joystick.Vertical +
            Camera.main.transform.right * joystick.Horizontal) * speed;
            move.y = rb.velocity.y;
            rb.velocity = move;
            move.y = 0;

            //�i�ޕ����Ɋ��炩�Ɍ����B
            transform.forward = Vector3.Slerp(transform.forward, move, Time.deltaTime * rotateSpeed);

            rb.AddForce(Physics.gravity * 3f, ForceMode.Acceleration); // 3�{�̏d�͂�K�p

           
            // �A�j���[�V�����̏�Ԃ𐧌䂷��
            if (rb.velocity.magnitude > 0.01)
            {
                //�󔠂����������Ă��鎞��������
                if (HasTreasure == true)
                {
                    animator.SetInteger("state", 4);
                }
                else
                {
                    // �L�����N�^�[�������Ă���ꍇ
                    animator.SetInteger("state", 1); //Run�A�j���[�V����
                }
            }
            else
            {
                // �L�����N�^�[���~�܂��Ă���ꍇ
                animator.SetInteger("state", 0); //Idle�A�j���[�V����
            }

            //�U������������
            if (IsAttack == true)
            {
                animator.SetInteger("state", 2);//�U���A�j���[�V����
                await roomHubModel.MoveAsync(this.transform.position, this.transform.rotation, CharacterState.Attack);//�U���A�j���[�V�����̓���
            }
        }
    }

    /// <summary>
    /// �󔠂��Z�b�g���郁�\�b�h
    /// </summary>
    /// <param name="chest"></param>
    public void SetTreasureChest(GameObject chest)
    {
        currentTreasureChest = chest;
    }

    /// <summary>
    /// �A�j���[�V�����C�x���g���g���ē���̏ꏊ����Collider��true�ɂ���
    /// </summary>
    public void StartAttack()
    {
        IsAttack = true;
        audioSource.PlayOneShot(AttackSE);//�U��SE
        collider.enabled = true;
    }

    /// <summary>
    /// �R���C�_�[�̔����false�ɂ���
    /// </summary>
    public void StopAttack()
    {
        IsAttack = false;
        collider.enabled = false;
        animator.SetInteger("state", 0);
    }

    /// <summary>
    /// �U���A�j���[�V�������I���܂�
    /// </summary>
    /// <returns></returns>
    private IEnumerator AttackAnimation()
    {
        // �A�j���[�V�����̏I����҂���ɁA�N�[���_�E�����Ԃ�҂�
        yield return new WaitForSeconds(AttckCoolDown);

        // �N�[���_�E���I����ɍU���t���O������
        isAttack = false;
        animator.SetInteger("state", 0); // Idle�A�j���[�V�����ɖ߂�
    }

    /// <summary>
    /// ���O��\��
    /// </summary>
    /// <param name="Name"></param>
    public void Name(string Name)
    {
        if (NameText != null)
        {
            NameText.text = Name;
        }
    }

    /// <summary>
    /// �f�t�H���g�̃A�^�b�N�{�^���̏���
    /// </summary>
    public void AttackButton()
    {
        if (gameDirector.IsEnemy == true && Isstart == true && Isself == true && IsAttack == false)
        {
            IsAttack = true;
            StartCoroutine(AttackAnimation());
        }
    }
}


