using Cysharp.Threading.Tasks.Triggers;
using Shared.Interfaces.StreamingHubs;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HumanManager : Character
{
    public override void Start()
    {
        base.Start();
    }

    // Update is called once per frame
    public override void Update()
    {
        base.Update();
    }

    //鬼の攻撃アニメーションが当たったときの処理
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
   
    //死亡アニメーションが終わった後のメソッド
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

    //死亡アニメーション関数
    public void OnAnimationDestroy()
    {
        if (IsDead == true)
        {
            gameDirector.KillAsync();
        }
    }

    //逃げる側が死んだ時にリスポーンする関数
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
}

