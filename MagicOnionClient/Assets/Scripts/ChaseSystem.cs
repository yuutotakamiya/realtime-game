using UnityEngine;

public class SimpleChaseAndEscape : MonoBehaviour
{
    public Transform runner;  // 逃げるキャラクターのTransform
    public float chaseSpeed = 5.0f;  // 鬼の追跡速度
    public float escapeSpeed = 4.0f;  // 逃げるキャラクターの逃走速度
    public float attackDistance = 1.5f;  // 攻撃できる距離
    public Camera mainCamera;  // メインカメラ

    private Animator animator;

    void Start()
    {
        animator = GetComponent<Animator>();

        // 初期回転を180度設定（Z軸周りに回転）
        transform.rotation = Quaternion.Euler(0, 180, 0);  // 鬼は最初に180度回転している状態
    }

    void Update()
    {
        // 鬼と逃げるキャラクターの距離を計算
        float distanceToRunner = Vector3.Distance(transform.position, runner.position);

        // 鬼が逃げるキャラクターの背中を追いかける
        ChaseRunner();

        // 逃げるキャラクターが鬼から逃げる
        EscapeFromChaser();

        // 攻撃範囲内であれば攻撃
        if (distanceToRunner <= attackDistance)
        {
            animator.SetInteger("State", 1);  // 攻撃アニメーション
        }
        else
        {
            animator.SetInteger("State", 0);  // 停止アニメーション
        }

        // カメラの外に出ないように位置を制限
        RestrictPositionToCameraView();
    }

    // 鬼が逃げるキャラクターの背中を追いかける
    void ChaseRunner()
    {
        // 鬼が進む方向を計算（逃げるキャラクターの位置に向かって進む）
        Vector3 direction = runner.position - transform.position;  // 逃げるキャラクターに向かって進む方向
        direction.y = 0;  // 高さを無視して、X-Z平面で追いかける

        // 追いかける方向に移動
        transform.position = Vector3.MoveTowards(transform.position, runner.position, chaseSpeed * Time.deltaTime);

        // 逃げるキャラクターの背中の方向に向かせる
        if (direction.magnitude > 0.1f)  // 移動する方向が有効な場合
        {
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(direction), 0.1f);  // 鬼が逃げるキャラクターに向かう
        }
    }

    // 逃げるキャラクターが鬼から逃げる
    void EscapeFromChaser()
    {
        // 鬼の反対方向に逃げる
        Vector3 direction = (runner.position - transform.position).normalized;  // 鬼の反対方向
        runner.position += direction * escapeSpeed * Time.deltaTime;

        // 逃げるキャラクターが反対方向に向く
        runner.forward = -direction;  // 逃げるキャラクターは鬼の反対方向を向く
    }

    // カメラの視界内にキャラクターがいるかをチェックし、範囲外に出ないように位置を制限
    void RestrictPositionToCameraView()
    {
        // カメラの範囲を計算（スクリーン座標に変換）
        Vector3 screenPos = mainCamera.WorldToScreenPoint(transform.position);
        float screenWidth = Screen.width;
        float screenHeight = Screen.height;

        // 画面の範囲外に出ないように制限
        if (screenPos.x < 0)
        {
            screenPos.x = 0;
        }
        if (screenPos.x > screenWidth)
        {
            screenPos.x = screenWidth;
        }
        if (screenPos.y < 0)
        {
            screenPos.y = 0;
        }
        if (screenPos.y > screenHeight)
        {
            screenPos.y = screenHeight;
        }

        // 画面の範囲内に戻す
        transform.position = mainCamera.ScreenToWorldPoint(new Vector3(screenPos.x, screenPos.y, mainCamera.WorldToScreenPoint(transform.position).z));
    }
}
