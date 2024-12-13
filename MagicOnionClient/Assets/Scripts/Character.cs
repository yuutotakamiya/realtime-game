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
    public bool isDead = false;//死んでいるどうか
    protected bool isself = false;//自分自身かどうか
    public bool isstart = false;//準備完了しているかどうか
    public bool isAttack = false;//攻撃中かどうか
    public float AttckCoolDown;//攻撃のクールダウン

    FixedJoystick joystick;
    public Rigidbody rb;
    public Animator animator;
    public RoomHubModel roomHub;
    public GameDirector gameDirector;
    public HumanManager humanManager;
    public AN_DoorScript doorScript; // ドアスクリプトの参照

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

            if (isAttack == true)
            {
                animator.SetInteger("state", 2);
                await roomHub.MoveAsync(this.transform.position, this.transform.rotation, CharacterState.Attack);
            }
        }
    }

    //攻撃アニメーションが終わるまで
    private IEnumerator AttackAnimation()
    {
        // アニメーションの終了を待つ代わりに、クールダウン時間を待つ
        yield return new WaitForSeconds(AttckCoolDown);

        // クールダウン終了後に攻撃フラグを解除
        isAttack = false;
        animator.SetInteger("state", 0); // Idleアニメーションに戻す
    }

    //デフォルトのアタックボタンの処理
    public void AttackButton()
    {
        if (gameDirector.IsEnemy == true && isstart == true && isself == true && isAttack == false)
        {
            isAttack = true;
            StartCoroutine(AttackAnimation());
        }
    }

    //ボタンを押したときの処理
    /*private void OnTriggerEnter(Collider other)
    {
        // プレイヤーが「Player」レイヤーのオブジェクトであれば、openButtonを表示
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


