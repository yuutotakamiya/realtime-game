using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Shared.Interfaces.StreamingHubs.IRoomHubReceiver;

public class Character : MonoBehaviour
{
    [SerializeField] float speed;
    [SerializeField] float rotateSpeed;
    //[SerializeField] protected Collider attackCollider;
    public bool isDead = false;//死んでいるどうか
    public bool isself = false;//自分自身かどうか
    public bool isstart = false;//準備完了しているかどうか

    FixedJoystick joystick;
    public Rigidbody rb;
    public Animator animator;
    public RoomHubModel roomHub;
    public GameDirector gameDirector;

    // Start is called before the first frame update
   public virtual void Start()
   {
        rb = GetComponent<Rigidbody>();
        joystick = GameObject.Find("Fixed Joystick").GetComponent<FixedJoystick>();
        animator = GetComponent<Animator>();
        roomHub = GameObject.Find("RoomModel").GetComponent<RoomHubModel>();
        gameDirector = GameObject.Find("GameDirector").GetComponent<GameDirector>();
        
    }

    // Update is called once per frame
   public virtual async void Update()
   {
        if (isstart == true)
        {
            Vector3 move = (Camera.main.transform.forward * joystick.Vertical +
            Camera.main.transform.right * joystick.Horizontal) * speed;
            move.y = rb.velocity.y;
            rb.velocity = move;
            //進む方向に滑らかに向く。
            transform.forward = Vector3.Slerp(transform.forward, move, Time.deltaTime * rotateSpeed);

            // アニメーションの状態を制御する
            if (rb.velocity.magnitude > 0.01)
            {
                // キャラクターが動いている場合
                animator.SetInteger("state", 1); //Runアニメーション
            }
            else
            {
                // キャラクターが止まっている場合
                animator.SetInteger("state", 0); //Idleアニメーション
            }

            if (Input.GetKeyDown(KeyCode.Space))
            {
                animator.SetInteger("state", 2);
                await roomHub.MoveAsync(this.transform.position, this.transform.rotation, CharacterState.Attack);
                //attackCollider.enabled = true;
            }



        }
   }
}

