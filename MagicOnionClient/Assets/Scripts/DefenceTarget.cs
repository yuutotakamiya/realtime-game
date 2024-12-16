using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public enum MoveMode
{
    Idle = 1,
    Follow = 2
}
public class DefenceTarget : MonoBehaviour
{
    [SerializeField] GameDirector gameDirector;
    public float move_speed = 3f;
    private bool isHolding;//持っているかどうか
    public float followDistance = 5f;  // 追従する最大距離
    private float hideDistance = 6f;   // ボタンを非表示にする閾値距離

    protected Rigidbody rb;
    protected Transform followTarget;
    protected MoveMode currentMoveMode;

    void Start()
    {
        rb = this.GetComponent<Rigidbody>();
        currentMoveMode = MoveMode.Idle;
        isHolding = false;

        // 初期状態でボタンを非表示
        gameDirector.holdButton.SetActive(false);
        gameDirector.notholdButton.SetActive(false);
    }

    void Update()
    {
        if (isHolding && followTarget != null)
        {
            Vector3 direction = (followTarget.position - transform.position).normalized;
            rb.MovePosition(transform.position + direction * move_speed * Time.deltaTime);
        }
        CheckDistanceAndUpdateButtons();
    }

    // プレイヤーが近くにいるかどうかをチェックしてボタンを表示/非表示にする
    private void CheckDistanceAndUpdateButtons()
    {
        if (followTarget != null)
        {
            float distance = Vector3.Distance(followTarget.position, transform.position);

            if (distance > hideDistance)
            {
                gameDirector.holdButton.SetActive(false);
                gameDirector.notholdButton.SetActive(false);
            }
            else
            {
                UpdateButtonState();
            }
        }
    }

    private void UpdateButtonState()
    {
        if (!isHolding)
        {
            // まだ引きずっていない状態なら「引きずる」ボタンを表示
            gameDirector.holdButton.SetActive(true);
            gameDirector.notholdButton.SetActive(false);
        }
        else
        {
            // 引きずっている状態なら「やめる」ボタンを表示
            gameDirector.holdButton.SetActive(false);
            gameDirector.notholdButton.SetActive(true);
        }
    }


    // プレイヤーが接触した時
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Human")&&gameDirector.IsEnemy==false)
        {
            followTarget = other.transform;  // 追従対象を設定

            // 初期状態では「引きずる」ボタンを表示し、「やめる」ボタンを非表示にする
            gameDirector.holdButton.SetActive(true);
            gameDirector.notholdButton.SetActive(false);

            // 距離に基づいてボタンを更新
            CheckDistanceAndUpdateButtons();
        }
    }

    // 「引きずる」ボタンが押された時
    public void OnHoldButtonPressed()
    {
        isHolding = true;  // 追従を開始
        UpdateButtonState();
    }

    // 「やめる」ボタンが押された時
    public void OnNotHoldButtonPressed()
    {
        isHolding = false;  // 追従を停止
        UpdateButtonState();
    }
}
