using Cinemachine;
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
    [SerializeField] public Text NameText;
    [SerializeField] GameObject EffectPrefab;
    protected bool isDead = false;//死んでいるどうか
    protected bool isself = false;//自分自身かどうか
    protected bool isstart = false;//準備完了しているかどうか
    protected bool isAttack = false;//攻撃中かどうか
    public bool isInDropArea=false;//宝箱の置くエリアにいるかどうか
    public float AttckCoolDown;//攻撃のクールダウン

    protected FixedJoystick joystick;
    protected Rigidbody rb;
    protected Animator animator;
    protected RoomHubModel roomHub;
    protected GameDirector gameDirector;
    [SerializeField] Collider collider;
    //protected AN_DoorScript doorScript; // ドアスクリプトの参照
    protected DefenceTarget defenceTarget;
    protected CinemachineVirtualCamera virtualCamera;
    public Renderer objectRenderer;
    public Color newColor = Color.yellow;

    public static Character instance;

    //自分自身かどうかのフラグのプロパティ
    public bool Isself
    {
        set { isself = value; }
        get { return isself; }
    }

    //死んでいるかどうかのフラグのプロパティ
    public bool IsDead
    {
        get { return isDead; }
        set { isDead = value; }
    }

    //攻撃しているかどうかのフラグのプロパティ
    public bool IsAttack
    {
        get { return isAttack; }
        set { isAttack = value; }
    }

    //準備完了しているかどうかのフラグのプロパティ
    public bool Isstart
    {
        get { return isstart; }
        set { isstart = value; }
    }

    //宝箱を置くエリアにいるかどうかのフラグのプロパティ
    public bool IsInDropArea
    {
        get { return isInDropArea; }
        set { isInDropArea = value; }
    }

    public virtual void Start()
    {
        rb = GetComponent<Rigidbody>();
        joystick = GameObject.Find("Fixed Joystick").GetComponent<FixedJoystick>();
        animator = GetComponent<Animator>();
        roomHub = GameObject.Find("RoomModel").GetComponent<RoomHubModel>();
        gameDirector = GameObject.Find("GameDirector").GetComponent<GameDirector>();
        //doorScript = GameObject.Find("Door").GetComponent<AN_DoorScript>();
        defenceTarget = GameObject.Find("DefenceTarget").GetComponent<DefenceTarget>();
        objectRenderer = GetComponent<Renderer>();

        //defenceTargetがnullだったとき何もしない
        if (defenceTarget == null) 
        {
            return;
        }

        //コライダーがnullなら何もしない
        if (collider == null)
        {
            return;
        }

        //joystickがnullなら何もしない
        if (joystick == null)
        {
            return;
        }
        collider.enabled = false;
        //rb.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;
    }

    public virtual async void Update()
    {
        if (Isstart == true)
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
                rb.velocity = Vector3.zero; 
            }

            if (IsAttack == true)
            {
                animator.SetInteger("state", 2);
                await roomHub.MoveAsync(this.transform.position, this.transform.rotation, CharacterState.Attack);
            }
        }
    }

    //アニメーションイベントを使って特定の場所だけColliderをtrueにする
    public void StartAttack()
    {
        IsAttack  =true;
        collider.enabled = true;
    }
   
    //コライダーの判定をfalseにする
    public void StopAttack()
    {
        IsAttack = false;
        collider.enabled = false;
        animator.SetInteger("state", 0);
    }

    //雷攻撃のコライダーの判定をtrueにする
    public void StartAttackAnimation()
    {
        IsAttack =true;
        collider.enabled = true;
        objectRenderer.material.color = newColor;
    }

    //雷攻撃のコライダーの判定をfalseにする
    public void StopAttackAnimation()
    {
        IsAttack = false;
        Destroy(EffectPrefab,4);
        collider.enabled = false;
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
   
    //名前を表示
    public void Name(string Name)
    {
        if (NameText != null)
        {
            NameText.text = Name;
        }
    }

    //デフォルトのアタックボタンの処理
    public void AttackButton()
    {
        if (gameDirector.IsEnemy == true && Isstart == true && Isself == true && IsAttack == false)
        {
            IsAttack = true;
            StartCoroutine(AttackAnimation());
        }
    }

    //雷攻撃アニメーション処理
    public void LightningAttack()
    {
        if (gameDirector.IsEnemy == true && Isstart == true && Isself == true && IsAttack == false)
        {
            IsAttack = true;
            Instantiate(EffectPrefab, transform.position, Quaternion.identity);
            StartCoroutine(AttackAnimation());
        }
    }


    //ドアボタンを押したときの処理
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


