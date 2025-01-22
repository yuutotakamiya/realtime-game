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

    //�S�̍U���A�j���[�V���������������Ƃ��̏���
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
   
    //���S�A�j���[�V�������I�������̃��\�b�h
    private IEnumerator RespawnAfterDeath()
    {
        // �A�j���[�V�������J�n����Ă���A���̏I����҂�
        while (animator.GetInteger("state") == 3)
        {
            yield return null;  // ���S�A�j���[�V�������I���܂őҋ@
        }
        OnAnimationDestroy();
        RespawnPlayer();
        IsDead = false;
    }

    //���S�A�j���[�V�����֐�
    public void OnAnimationDestroy()
    {
        if (IsDead == true)
        {
            gameDirector.KillAsync();
        }
    }

    //�����鑤�����񂾎��Ƀ��X�|�[������֐�
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

