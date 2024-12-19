using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public enum MoveMode
{
    Idle = 1,
    Follow = 2
}
public class DefenceTarget : MonoBehaviour
{
    [SerializeField] GameDirector gameDirector;
    public float move_speed;

    protected Rigidbody rb;
    protected Transform followTarget;
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
                currentMoveMode = MoveMode.Idle;
            }
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
            }
        }
        
    }

    //定期的に呼ぶメソッド
    public void Chest()
    {
        gameDirector.MoveChest(this.gameObject.transform.position,this.transform.rotation, this.gameObject.name);
    }
}
