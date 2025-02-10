//==========================================================
//
//�󔠂̒Ǐ]���Ǘ�����
//Author:���{�S��
//
//==========================================================
using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.TextCore.Text;

/// <summary>
/// �󔠂̏�Ԃ̊Ǘ�
/// </summary>
public enum MoveMode
{
    Idle = 1,
    Follow = 2
}

/// <summary>
/// �󔠂��v���C���[��Ǐ]����X�N���v�g
/// </summary>
public class DefenceTarget : MonoBehaviour
{
    [SerializeField] GameDirector gameDirector;//GameDirector�N���X�̐ݒ�
    [SerializeField] Character character;//Character�N���X�̐ݒ�
    [SerializeField] RoomHubModel roomHubModel;//RoomHubModel�N���X�̐ݒ�
    [SerializeField] HumanManager humanManager;//HumanManager�N���X�̐ݒ�
    public float move_speed;//�󔠂��X�s�[�h

    protected Rigidbody rb;
    public Transform followTarget;
    public MoveMode currentMoveMode;

    /// <summary>
    /// �J�n����
    /// </summary>
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        currentMoveMode = MoveMode.Idle;
    }

    /// <summary>
    /// ���b�Ă΂��֐�
    /// </summary>
    void Update()
    {
        DoAutoMovement();
    }

    /// <summary>
    /// �󔠂̈ړ�
    /// </summary>
    protected void DoAutoMovement()
    {
        switch (currentMoveMode)
        {
            case MoveMode.Follow:
                if (followTarget != null)
                {
                    /* �^�[�Q�b�g�ƃv���C���[�̋������擾 */
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
    /// ������l���g���K�[�ɐG�ꂽ��
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
               Character character�@= other.gameObject.GetComponent<Character>();
                character.SetTreasureChest(this.gameObject);
                humanManager.PickUpTreasure();// �󔠂������Ă��邱�Ƃ��v���C���[�ɓ`����
                currentMoveMode = MoveMode.Idle;
            }
        }
    }

    /// <summary>
    /// ������l���󔠂��痣�ꂽ��
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
                humanManager.DropTreasure(); // �󔠂𗣂ꂽ��A�����Ă��Ȃ���Ԃɖ߂�
                currentMoveMode = MoveMode.Follow;
            }
        }
    }

    /// <summary>
    /// ����I�ɌĂԃ��\�b�h(�󔠂̈ʒu����)
    /// </summary>
    public void Chest()
    {
        gameDirector.MoveChest(this.gameObject.transform.position, this.gameObject.transform.rotation, this.gameObject.name);
    }
}
