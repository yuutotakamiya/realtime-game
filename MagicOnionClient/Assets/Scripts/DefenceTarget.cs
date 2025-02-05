using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.TextCore.Text;
using static UnityEditor.PlayerSettings;
public enum MoveMode
{
    Idle = 1,
    Follow = 2
}
public class DefenceTarget : MonoBehaviour
{
    [SerializeField] GameDirector gameDirector;
    [SerializeField] Character character;
    [SerializeField] RoomHubModel roomHubModel;
    [SerializeField] HumanManager humanManager;
    public float move_speed;//�󔠂��X�s�[�h

    protected Rigidbody rb;
    public Transform followTarget;
    public MoveMode currentMoveMode;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        currentMoveMode = MoveMode.Idle;
    }

    void Update()
    {
        DoAutoMovement();
    }

    protected void DoAutoMovement()
    {
        switch (currentMoveMode)
        {
            /*case MoveMode.Wait:
                break;*/
            case MoveMode.Follow:
                if (followTarget != null)
                {
                    //character.transform.DOLocalMove(pos, 0.1f).SetEase(Ease.Linear);
                    //character.transform.DORotate(rotaition.eulerAngles, 0.1f).SetEase(Ease.Linear);

                    //transform.DORotateQuaternion(move_rotation, 0.1f).SetEase(Ease.Linear);

                    /* �^�[�Q�b�g�ƃv���C���[�̋������擾 */
                    float dis = Vector3.Distance(followTarget.transform.position, this.transform.position);

                    //transform.DOComplete();
                    
                    if (dis > 3f)
                    {
                        Quaternion move_rotation = Quaternion.LookRotation(followTarget.transform.position - transform.position, Vector3.up);
                        transform.rotation = Quaternion.Lerp(transform.rotation, move_rotation, 0.1f);
                        rb.velocity = transform.forward * move_speed;

                        // transform.DOMove(followTarget.transform.position, 0.1f).SetEase(Ease.Linear);
                    }
                    else
                    {
                        rb.velocity = Vector3.zero;
                    }
                }

                break;
        }
    }

    /*public void OnCollisionEnter(Collision collision)
    {
        if(collision.gameObject.tag == "Human")
        {
            followTarget = null;

            if (currentMoveMode == MoveMode.Follow)
            {
                currentMoveMode = MoveMode.Idle;
            }
        }
    }

    public void OnCollisionExit(Collision collision)
    {
        if(collision.gameObject.tag == "Human")
        {
            followTarget = collision.transform;

            if (currentMoveMode == MoveMode.Idle)
            {
                currentMoveMode = MoveMode.Follow;
            }
        }
    }*/

    //������l���g���K�[�ɐG�ꂽ��
    public void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Human")
        {
            followTarget = null;
            InvokeRepeating("Chest", 0.1f, 0.1f);
            if (currentMoveMode == MoveMode.Follow)
            {
               character.SetTreasureChest(this.gameObject);
                //humanManager.PickUpTreasure(); // �󔠂������Ă��邱�Ƃ��v���C���[�ɓ`����
                gameDirector.characterList[roomHubModel.ConnectionId].GetComponent<HumanManager>().PickUpTreasure();
                currentMoveMode = MoveMode.Idle;
            }
        }
    }

    //������l���󔠂��痣�ꂽ��
    public void OnTriggerExit(Collider other)
    {
        if (other.gameObject.tag == "Human")
        {
            followTarget = other.transform;
            CancelInvoke("Chest");
            if (currentMoveMode == MoveMode.Idle)
            {
                gameDirector.characterList[roomHubModel.ConnectionId].GetComponent<HumanManager>().DropTreasure();
                currentMoveMode = MoveMode.Follow;
                //humanManager. DropTreasure(); // �󔠂𗣂ꂽ��A�����Ă��Ȃ���Ԃɖ߂�
            }
        }
    }

    //����I�ɌĂԃ��\�b�h(�󔠂̈ʒu����)
    public void Chest()
    {
        gameDirector.MoveChest(this.gameObject.transform.position, this.gameObject.transform.rotation, this.gameObject.name);
    }
}
