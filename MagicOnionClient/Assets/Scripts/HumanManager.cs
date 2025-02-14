//==========================================================
//
//逃げる人の管理処理
//Author:高宮祐翔
//
//==========================================================
using Cysharp.Threading.Tasks.Triggers;
using Shared.Interfaces.StreamingHubs;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Characterスクリプトを継承した、逃げる側のキャラクターを管理しているクラス
/// </summary>
public class HumanManager : Character
{
    /// <summary>
    /// 開始処理
    /// </summary>
    public override void Start()
    {
        base.Start();
    }

    /// <summary>
    /// 毎フレーム呼ばれる関数
    /// </summary>
    public override void Update()
    {
        base.Update();
    }

    /// <summary>
    /// 鬼の攻撃アニメーションが当たったときの処理
    /// </summary>
    /// <param name="other"></param>
    public void OnTriggerEnter(Collider other)
    {
        if (Isself == true && animator.GetInteger("state") != 3 && !IsDead)
        {
           GameObject weapon = GameObject.Find("Mesh_Weapon_01");
            
            if (other.gameObject != this.gameObject)
            {
                if (other.CompareTag("weapon")||other.CompareTag("Effect"))
                {
                    IsDead = true;
                    animator.SetInteger("state", 3);

                    Isstart = false;
                    rb.velocity = Vector3.zero;

                    other.enabled = false;
                    StartCoroutine(RespawnAfterDeath());
                    
                }
            }
        }
    }
   
    /// <summary>
    /// 死亡アニメーションが終わった後のメソッド
    /// </summary>
    /// <returns></returns>
    private IEnumerator RespawnAfterDeath()
    {
        // アニメーションが開始されてから、その終了を待つ
        while (animator.GetInteger("state") == 3)
        {
            yield return null;  // 死亡アニメーションが終わるまで待機
        }
        OnAnimationDestroy();
        RespawnPlayer();
        IsDead = false;
    }

    /// <summary>
    /// 死亡アニメーション関数
    /// </summary>
    public async void OnAnimationDestroy()
    {
        if (IsDead == true)
        {
           await gameDirector.KillAsync();//非同期で鬼がキルしたときの関数を呼び出し
        }
    }

    /// <summary>
    /// 逃げる側が死んだ時にリスポーンする関数
    /// </summary>
    public void RespawnPlayer()
    {
        GameObject respawn;

        int randomIndex = Random.Range(0, 3);
        if (randomIndex == 0)
        {
            respawn = GameObject.Find("WarpPotion");
        }
        else if (randomIndex == 1)
        {
            respawn = GameObject.Find("WarpPotion (1)");
        }
        else
        {
            respawn = GameObject.Find("WarpPotion (2)");
        }
        Debug.Log(randomIndex);


        transform.position = respawn.transform.position;
        transform.rotation = respawn.transform.rotation;
        IsDead = true;

        animator.SetInteger("state", 0);
        Isstart = true;
        IsDead = false;
    }

   

    /// <summary>
    /// 宝箱を所持している
    /// </summary>
    public void PickUpTreasure()
    {
        HasTreasure = true; // 宝箱を拾った状態
    }

    /// <summary>
    /// 宝箱を置いた状態
    /// </summary>
    public void DropTreasure()
    {
        HasTreasure = false; // 宝箱を置いた状態
        currentTreasureChest = null;
    }
}

