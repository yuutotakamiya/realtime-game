using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;
using static Shared.Interfaces.StreamingHubs.IRoomHubReceiver;

public class Character : MonoBehaviour
{
    [SerializeField] float speed;
    [SerializeField] float rotateSpeed;
    //[SerializeField] Button AttckButton1;
    //[SerializeField] Button AttckButton2;
    //[SerializeField] protected Collider attackCollider;
    public bool isDead = false;//����ł���ǂ���
    protected bool isself = false;//�������g���ǂ���
    public bool isstart = false;//�����������Ă��邩�ǂ���
    public bool isAttack = false;//�U�������ǂ���
    public float AttckCoolDown;//�U���̃N�[���_�E��

    FixedJoystick joystick;
    public Rigidbody rb;
    public Animator animator;
    public RoomHubModel roomHub;
    public GameDirector gameDirector;
    public HumanManager humanManager;
    public AN_DoorScript doorScript; // �h�A�X�N���v�g�̎Q��

    public bool Isself
    {
        set { isself = value; }
    }


    public virtual void Start()
    {
        rb = GetComponent<Rigidbody>();
        joystick = GameObject.Find("Fixed Joystick").GetComponent<FixedJoystick>();
        animator = GetComponent<Animator>();
        roomHub = GameObject.Find("RoomModel").GetComponent<RoomHubModel>();
        gameDirector = GameObject.Find("GameDirector").GetComponent<GameDirector>();
        humanManager = GameObject.Find("HumanManager").GetComponent<HumanManager>();

        doorScript = GameObject.Find("Door").GetComponent<AN_DoorScript>();
    }

    public virtual async void Update()
    {
        if (isstart == true)
        {
            Vector3 move = (Camera.main.transform.forward * joystick.Vertical +
            Camera.main.transform.right * joystick.Horizontal) * speed;
            move.y = rb.velocity.y;
            rb.velocity = move;
            //�i�ޕ����Ɋ��炩�Ɍ����B
            transform.forward = Vector3.Slerp(transform.forward, move, Time.deltaTime * rotateSpeed);

            // �A�j���[�V�����̏�Ԃ𐧌䂷��
            if (rb.velocity.magnitude > 0.01)
            {
                // �L�����N�^�[�������Ă���ꍇ
                animator.SetInteger("state", 1); //Run�A�j���[�V����
            }
            else
            {
                // �L�����N�^�[���~�܂��Ă���ꍇ
                animator.SetInteger("state", 0); //Idle�A�j���[�V����
            }

            if (isAttack == true)
            {
                animator.SetInteger("state", 2);
                await roomHub.MoveAsync(this.transform.position, this.transform.rotation, CharacterState.Attack);
            }
        }
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

    //�f�t�H���g�̃A�^�b�N�{�^���̏���
    public void AttackButton()
    {
        if (gameDirector.IsEnemy == true && isstart == true && isself == true && isAttack == false)
        {
            isAttack = true;
            StartCoroutine(AttackAnimation());
        }
    }

    //�{�^�����������Ƃ��̏���
    /*private void OnTriggerEnter(Collider other)
    {
        // �v���C���[���uPlayer�v���C���[�̃I�u�W�F�N�g�ł���΁AopenButton��\��
        if (other.gameObject.layer == LayerMask.NameToLayer("killer"))
        {
            gameDirector.openButton.gameObject.SetActive(true);
        }
        else
        {
            gameDirector.openButton.gameObject.SetActive(false);
        }
    }*/
}


