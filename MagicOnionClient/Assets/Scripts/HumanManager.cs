using Cysharp.Threading.Tasks.Triggers;
using Shared.Interfaces.StreamingHubs;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Shared.Interfaces.StreamingHubs.IRoomHubReceiver;

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


    public void OnAnimationDestroy()
    {
        //Debug.Log("KillAsync called");
        if (IsDead == true)
        {
            gameDirector.KillAsync();
        }
    }


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

