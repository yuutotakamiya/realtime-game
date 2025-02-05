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
    [SerializeField] Collider collider;//攻撃の当たり判定
    protected bool isDead = false;//死んでいるどうか
    protected bool isself = false;//自分自身かどうか
    protected bool isstart = false;//準備完了しているかどうか
    protected bool isAttack = false;//攻撃中かどうか
    public bool isInDropArea = false;//宝箱の置くエリアにいるかどうか
    public float AttckCoolDown;//攻撃のクールダウン
    protected bool hasTreasure = false;//宝箱を持っているかどうかのフラグ

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

    public GameObject currentTreasureChest;//現在引きずっている宝箱

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

    //宝箱を既に持っているかどうかのフラグのプロパティ
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

        //defenceTargetがnullだったとき何もしない
        if (defenceTarget == null)
        {
            return;
        }
        //joystickがnullなら何もしない
        if (joystick == null)
        {
            return;
        }
        //コライダーがnullなら何もしない
        if (collider == null)
        {
            return;
        }
        //rigidbodyがnullだったら何もしない
        if (rb == null)
        {
            return;
        }
        //gameDirecterがnullだったら何もしない
        if (gameDirector == null)
        {
            return;
        }
        //defenceTargetがnullだったら何もしない
        if (defenceTarget == null)
        {
            return;
        }
        //Rendererがnullだったら何もしない
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

            //進む方向に滑らかに向く。
            transform.forward = Vector3.Slerp(transform.forward, move, Time.deltaTime * rotateSpeed);

            rb.AddForce(Physics.gravity * 3f, ForceMode.Acceleration); // 3倍の重力を適用

           
            // アニメーションの状態を制御する
            if (rb.velocity.magnitude > 0.01)
            {
                //宝箱を引きずっている時だったら
                if (HasTreasure == true)
                {
                    animator.SetInteger("state", 4);
                }
                else
                {
                    // キャラクターが動いている場合
                    animator.SetInteger("state", 1); //Runアニメーション
                }
            }
            else
            {
                // キャラクターが止まっている場合
                animator.SetInteger("state", 0); //Idleアニメーション
            }

            //攻撃中だったら
            if (IsAttack == true)
            {
                animator.SetInteger("state", 2);//攻撃アニメーション
                await roomHub.MoveAsync(this.transform.position, this.transform.rotation, CharacterState.Attack);//攻撃アニメーションの同期
            }
        }
    }
    // 宝箱をセットするメソッド
    public void SetTreasureChest(GameObject chest)
    {
        currentTreasureChest = chest;
    }

    //アニメーションイベントを使って特定の場所だけColliderをtrueにする
    public void StartAttack()
    {
        IsAttack = true;
        audioSource.PlayOneShot(AttackSE);//攻撃SE
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
        IsAttack = true;
        collider.enabled = true;
        //objectRenderer.material.color = newColor;
    }

    //雷攻撃のコライダーの判定をfalseにする
    public void StopAttackAnimation()
    {
        IsAttack = false;
        Destroy(EffectPrefab, 4);
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
}


