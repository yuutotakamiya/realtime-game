using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;
using static Shared.Interfaces.StreamingHubs.IRoomHubReceiver;

public class Character : MonoBehaviour
{
    [SerializeField] float speed;
    [SerializeField] float rotateSpeed;
    [SerializeField] public Text NameText;
    [SerializeField] GameObject EffectPrefab;
    [SerializeField] AudioClip AttackSE;
    [SerializeField] Collider collider;//�U���̓����蔻��
    protected bool isDead = false;//����ł���ǂ���
    protected bool isself = false;//�������g���ǂ���
    protected bool isstart = false;//�����������Ă��邩�ǂ���
    protected bool isAttack = false;//�U�������ǂ���
    public bool isInDropArea = false;//�󔠂̒u���G���A�ɂ��邩�ǂ���
    public float AttckCoolDown;//�U���̃N�[���_�E��
    protected bool hasTreasure = false;//�󔠂������Ă��邩�ǂ����̃t���O

    protected FixedJoystick joystick;
    protected Rigidbody rb;
    protected Animator animator;
    protected RoomHubModel roomHub;
    protected GameDirector gameDirector;
    protected DefenceTarget defenceTarget;
    protected CinemachineVirtualCamera virtualCamera;
    public Renderer objectRenderer;
    //public Color newColor = Color.yellow;
    AudioSource audioSource;

    public static Character instance;

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

    //�󔠂�u���G���A�ɂ��邩�ǂ����̃t���O�̃v���p�e�B
    public bool IsInDropArea
    {
        get { return isInDropArea; }
        set { isInDropArea = value; }
    }

    //�󔠂����Ɏ����Ă��邩�ǂ����̃t���O�̃v���p�e�B
    public bool HasTreasure
    {
        get { return hasTreasure; }
        set { hasTreasure = value; }
    }

    public virtual void Start()
    {
        audioSource = GetComponent<AudioSource>();
        rb = GetComponent<Rigidbody>();
        joystick = GameObject.Find("Fixed Joystick").GetComponent<FixedJoystick>();
        animator = GetComponent<Animator>();
        roomHub = GameObject.Find("RoomModel").GetComponent<RoomHubModel>();
        gameDirector = GameObject.Find("GameDirector").GetComponent<GameDirector>();
        defenceTarget = GameObject.Find("DefenceTarget").GetComponent<DefenceTarget>();
        objectRenderer = GetComponent<Renderer>();

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
        //Renderer��null�������牽�����Ȃ�
        if (objectRenderer == null)
        {
            return;
        }
        collider.enabled = false;
    }

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
                await roomHub.MoveAsync(this.transform.position, this.transform.rotation, CharacterState.Attack);//�U���A�j���[�V�����̓���
            }
        }
    }
    // �󔠂��Z�b�g���郁�\�b�h
    public void SetTreasureChest(GameObject chest)
    {
        currentTreasureChest = chest;
    }

    //�A�j���[�V�����C�x���g���g���ē���̏ꏊ����Collider��true�ɂ���
    public void StartAttack()
    {
        IsAttack = true;
        audioSource.PlayOneShot(AttackSE);//�U��SE
        collider.enabled = true;
    }

    //�R���C�_�[�̔����false�ɂ���
    public void StopAttack()
    {
        IsAttack = false;
        collider.enabled = false;
        animator.SetInteger("state", 0);
    }

    //���U���̃R���C�_�[�̔����true�ɂ���
    public void StartAttackAnimation()
    {
        IsAttack = true;
        collider.enabled = true;
        //objectRenderer.material.color = newColor;
    }

    //���U���̃R���C�_�[�̔����false�ɂ���
    public void StopAttackAnimation()
    {
        IsAttack = false;
        Destroy(EffectPrefab, 4);
        collider.enabled = false;
    }

    //�U���A�j���[�V�������I���܂�
    private IEnumerator AttackAnimation()
    {
        // �A�j���[�V�����̏I����҂���ɁA�N�[���_�E�����Ԃ�҂�
        yield return new WaitForSeconds(AttckCoolDown);

        // �N�[���_�E���I����ɍU���t���O������
        isAttack = false;
        animator.SetInteger("state", 0); // Idle�A�j���[�V�����ɖ߂�
    }

    //���O��\��
    public void Name(string Name)
    {
        if (NameText != null)
        {
            NameText.text = Name;
        }
    }

    //�f�t�H���g�̃A�^�b�N�{�^���̏���
    public void AttackButton()
    {
        if (gameDirector.IsEnemy == true && Isstart == true && Isself == true && IsAttack == false)
        {
            IsAttack = true;
            StartCoroutine(AttackAnimation());
        }
    }

    //���U���A�j���[�V��������
    public void LightningAttack()
    {
        if (gameDirector.IsEnemy == true && Isstart == true && Isself == true && IsAttack == false)
        {
            IsAttack = true;
            Instantiate(EffectPrefab, transform.position, Quaternion.identity);
            StartCoroutine(AttackAnimation());
        }
    }
}


