using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Character : MonoBehaviour
{
    [SerializeField] float speed;
    public bool isself = false;
    public bool isstart = false;
    FixedJoystick joystick;
    Rigidbody rb;
    [SerializeField] float rotateSpeed = 10f;
    /*[SerializeField] Camera characterCamera;
    [SerializeField] GameObject character;*/
    Animator animator;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        joystick = GameObject.Find("Fixed Joystick").GetComponent<FixedJoystick>();
        /*Collider characterCollider = GetComponent<Collider>();
        characterCollider.material = (PhysicMaterial)Resources.Load("CharacterMaterial");*/
        animator = GetComponent<Animator>();
        //animator.GetInteger("state");

    }

    // Update is called once per frame
    void Update()
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

            if (Input.GetKeyDown(KeyCode.Space))
            {
                animator.SetInteger("state", 2);
            }

        }

        
    }

    public void Animation()
    {
        
    }
}
