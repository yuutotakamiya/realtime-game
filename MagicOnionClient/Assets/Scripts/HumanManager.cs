//==========================================================
//
//������l�̊Ǘ�����
//Author:���{�S��
//
//==========================================================
using Cysharp.Threading.Tasks.Triggers;
using Shared.Interfaces.StreamingHubs;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Character�X�N���v�g���p�������A�����鑤�̃L�����N�^�[���Ǘ����Ă���N���X
/// </summary>
public class HumanManager : Character
{
    /// <summary>
    /// �J�n����
    /// </summary>
    public override void Start()
    {
        base.Start();
    }

    /// <summary>
    /// ���t���[���Ă΂��֐�
    /// </summary>
    public override void Update()
    {
        base.Update();
    }

    /// <summary>
    /// �S�̍U���A�j���[�V���������������Ƃ��̏���
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
    /// ���S�A�j���[�V�������I�������̃��\�b�h
    /// </summary>
    /// <returns></returns>
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

    /// <summary>
    /// ���S�A�j���[�V�����֐�
    /// </summary>
    public async void OnAnimationDestroy()
    {
        if (IsDead == true)
        {
           await gameDirector.KillAsync();//�񓯊��ŋS���L�������Ƃ��̊֐����Ăяo��
        }
    }

    /// <summary>
    /// �����鑤�����񂾎��Ƀ��X�|�[������֐�
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
    /// �󔠂��������Ă���
    /// </summary>
    public void PickUpTreasure()
    {
        HasTreasure = true; // �󔠂��E�������
    }

    /// <summary>
    /// �󔠂�u�������
    /// </summary>
    public void DropTreasure()
    {
        HasTreasure = false; // �󔠂�u�������
        currentTreasureChest = null;
    }
}

