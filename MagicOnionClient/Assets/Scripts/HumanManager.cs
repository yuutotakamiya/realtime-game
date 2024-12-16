using Cysharp.Threading.Tasks.Triggers;
using Shared.Interfaces.StreamingHubs;
//using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Shared.Interfaces.StreamingHubs.IRoomHubReceiver;

public class HumanManager : Character
{
    //[SerializeField] GameObject Wizard;
    //GameObject weapon;
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
        if (isself == true && animator.GetInteger("state") != 3 && !isDead)
        {
           GameObject weapon = GameObject.Find("Mesh_Weapon_01");
            
            if (other.gameObject != this.gameObject)
            {
                if (other.CompareTag("weapon"))
                {
                    isDead = true;
                    animator.SetInteger("state", 3);

                    isstart = false;
                    rb.velocity = Vector3.zero;

                    other.enabled = false;
                    StartCoroutine(RespawnAfterDeath());
                    
                }
            }
        }
    }
   
    private IEnumerator RespawnAfterDeath()
    {
        // �A�j���[�V�������J�n����Ă���A���̏I����҂�
        while (animator.GetInteger("state") == 3)
        {
            yield return null;  // ���S�A�j���[�V�������I���܂őҋ@
        }
        OnAnimationDestroy();
        RespawnPlayer();
        isDead = false;
    }


    public void OnAnimationDestroy()
    {
        //Debug.Log("KillAsync called");
        if (isDead == true)
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
        isDead = true;

        animator.SetInteger("state", 0);
        isstart = true;
        isDead = false;
    }

}

