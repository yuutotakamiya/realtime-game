using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.TextCore.Text;
public enum MoveMode
{
    Idle = 1,
    Follow = 2
}
public class DefenceTarget : MonoBehaviour
{
    [SerializeField] GameDirector gameDirector;
    [SerializeField] Character character;
    public float move_speed;//�󔠂��X�s�[�h

    protected Rigidbody rb;
    public Transform followTarget;
    protected MoveMode currentMoveMode;

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
                    Quaternion move_rotation = Quaternion.LookRotation(followTarget.transform.position - transform.position, Vector3.up);
                    transform.rotation = Quaternion.Lerp(transform.rotation, move_rotation, 0.1f);
                    rb.velocity = transform.forward * move_speed;
                }

                break;
        }
    }

    public void OnCollisionEnter(Collision collision)
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
    }

    public void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Human")
        {
            followTarget = null;
            //followTarget = other.transform;
            InvokeRepeating("Chest",0.1f,0.1f);
            if (currentMoveMode == MoveMode.Follow)
            {
                character.PickUpTreasure(); // �󔠂������Ă��邱�Ƃ��v���C���[�ɓ`����
                currentMoveMode = MoveMode.Idle;
            }

            /*if (character != null && !character.HasTreasure)
            {
                followTarget = other.transform;
                currentMoveMode = MoveMode.Follow;
                
            }*/
        }
    }

    public void OnTriggerExit(Collider other)
    {
        if (other.gameObject.tag =="Human")
        {
            followTarget = other.transform;
            //followTarget = null;
            CancelInvoke("Chest");
            if (currentMoveMode == MoveMode.Idle)
            {
                currentMoveMode = MoveMode.Follow;
                character.DropTreasure(); // �󔠂𗣂ꂽ��A�����Ă��Ȃ���Ԃɖ߂�
            }

            /*followTarget = null;
            currentMoveMode = MoveMode.Idle;
            if (character != null)
            {
                
            }*/
        }
        
    }

    //����I�ɌĂԃ��\�b�h(�󔠂̈ʒu����)
    public void Chest()
    {
        gameDirector.MoveChest(this.gameObject.transform.position,this.gameObject.transform.rotation, this.gameObject.name);
    }
}
