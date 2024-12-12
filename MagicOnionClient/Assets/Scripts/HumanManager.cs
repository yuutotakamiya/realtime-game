using Cysharp.Threading.Tasks.Triggers;
using Shared.Interfaces.StreamingHubs;
//using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Shared.Interfaces.StreamingHubs.IRoomHubReceiver;

public class HumanManager : Character
{
    [SerializeField] GameObject HumanGameOverText;
    [SerializeField] Character character;

    //[SerializeField] GameObject[] WarpPotion;
    //RoomHubModel roomHubModel;
    public override void Start()
    {
        base.Start();
        
    }

    // Update is called once per frame
    public override void Update()
    {
        base.Update();
    }

    public  void OnTriggerEnter(Collider other)
    {
        if (isself == true)
        {
            // �Փ˂����I�u�W�F�N�g�������ł͂Ȃ��ꍇ
            if (other.gameObject != this.gameObject)
            {
                GameObject weapon = GameObject.Find("Mesh_Weapon_01");

                if (isAttack&&other.CompareTag("killer"))
                {
                    animator.SetInteger("state", 3);

                    isstart = false;
                    rb.velocity = Vector3.zero;

                    StartCoroutine(RespawnAfterDeath());
                }
              
            }
        }
    }

    // �A�j���[�V�������I�������Ƀ��X�|�[�����Ăяo��
    private IEnumerator RespawnAfterDeath()
    {
        // �A�j���[�V�����̍Đ����Ԃ�҂�
        /*AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(0);
        float animationTime = stateInfo.length;

        yield return new WaitForSeconds(animationTime);*/

        // ���S�A�j���[�V�������Đ�����Ă���Ԃ͏����𒆒f
        yield return new WaitForSeconds(animator.GetCurrentAnimatorStateInfo(0).length);

        // �I�u�W�F�N�g���폜���A���X�|�[���������Ă�
        OnAnimationDestroy();
        RespawnPlayer();
    }

    //�A�j���[�V�����C�x���g�p�̃��\�b�h
    public void OnAnimationDestroy()
    {
        //gameDirector. CancelInvoke("Move");
        gameDirector.KillAsync();
    }

    //�v���C���[�����񂾂烊�X�|�[�����郁�\�b�h
    public void RespawnPlayer()
    {

        GameObject respawn;
        // �����_���ȏꏊ�Ƀ��X�|�[��
        int randomIndex = Random.Range(0, 3);
        if (randomIndex == 0)
        {
           respawn= GameObject.Find("WarpPotion");
        }
        else if (randomIndex==1)
        {
            respawn = GameObject.Find("WarpPotion (1)");
        }
        else
        {
            respawn = GameObject.Find("WarpPotion (2)");
        }
        Debug.Log(randomIndex);

        // �I�΂ꂽ�ꏊ�Ƀv���C���[�����[�v
        transform.position = respawn.transform.position;
        transform.rotation = respawn.transform .rotation;

        animator.SetInteger("state", 0);
        isstart = true;       
    }

}

