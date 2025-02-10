//==========================================================
//
//宝箱の追従を管理処理
//Author:高宮祐翔
//
//==========================================================
using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.TextCore.Text;

/// <summary>
/// 宝箱の状態の管理
/// </summary>
public enum MoveMode
{
    Idle = 1,
    Follow = 2
}

/// <summary>
/// 宝箱がプレイヤーを追従するスクリプト
/// </summary>
public class DefenceTarget : MonoBehaviour
{
    [SerializeField] GameDirector gameDirector;//GameDirectorクラスの設定
    [SerializeField] Character character;//Characterクラスの設定
    [SerializeField] RoomHubModel roomHubModel;//RoomHubModelクラスの設定
    [SerializeField] HumanManager humanManager;//HumanManagerクラスの設定
    public float move_speed;//宝箱をスピード

    protected Rigidbody rb;
    public Transform followTarget;
    public MoveMode currentMoveMode;

    /// <summary>
    /// 開始処理
    /// </summary>
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        currentMoveMode = MoveMode.Idle;
    }

    /// <summary>
    /// 毎秒呼ばれる関数
    /// </summary>
    void Update()
    {
        DoAutoMovement();
    }

    /// <summary>
    /// 宝箱の移動
    /// </summary>
    protected void DoAutoMovement()
    {
        switch (currentMoveMode)
        {
            case MoveMode.Follow:
                if (followTarget != null)
                {
                    /* ターゲットとプレイヤーの距離を取得 */
                    float dis = Vector3.Distance(followTarget.transform.position, this.transform.position);
                    
                    if (dis > 3f)
                    {
                        Quaternion move_rotation = Quaternion.LookRotation(followTarget.transform.position - transform.position, Vector3.up);
                        transform.rotation = Quaternion.Lerp(transform.rotation, move_rotation, 0.1f);
                        rb.velocity = transform.forward * move_speed;
                    }
                    else
                    {
                        rb.velocity = Vector3.zero;
                    }
                }

                break;
        }
    }

    /// <summary>
    /// 逃げる人がトリガーに触れたら
    /// </summary>
    /// <param name="other"></param>
    public void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Human")
        {
            followTarget = null;
            InvokeRepeating("Chest", 0.1f, 0.1f);
            if (currentMoveMode == MoveMode.Follow)
            {
               Character character　= other.gameObject.GetComponent<Character>();
                character.SetTreasureChest(this.gameObject);
                humanManager.PickUpTreasure();// 宝箱を持っていることをプレイヤーに伝える
                currentMoveMode = MoveMode.Idle;
            }
        }
    }

    /// <summary>
    /// 逃げる人が宝箱から離れたら
    /// </summary>
    /// <param name="other"></param>
    public void OnTriggerExit(Collider other)
    {
        if (other.gameObject.tag == "Human")
        {
            followTarget = other.transform;
            CancelInvoke("Chest");
            if (currentMoveMode == MoveMode.Idle)
            {
                humanManager.DropTreasure(); // 宝箱を離れたら、持っていない状態に戻す
                currentMoveMode = MoveMode.Follow;
            }
        }
    }

    /// <summary>
    /// 定期的に呼ぶメソッド(宝箱の位置同期)
    /// </summary>
    public void Chest()
    {
        gameDirector.MoveChest(this.gameObject.transform.position, this.gameObject.transform.rotation, this.gameObject.name);
    }
}
