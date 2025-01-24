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
    [SerializeField] RoomHubModel roomHubModel;
    [SerializeField] HumanManager humanManager;
    public float move_speed;//宝箱をスピード

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

    //逃げる人がトリガーに触れたら
    public void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Human")
        {
            followTarget = null;
            //followTarget = other.transform;
            InvokeRepeating("Chest",0.1f,0.1f);
            if (currentMoveMode == MoveMode.Follow)
            {
                //humanManager.PickUpTreasure(); // 宝箱を持っていることをプレイヤーに伝える
                gameDirector.characterList[roomHubModel.ConnectionId].GetComponent<HumanManager>().PickUpTreasure();
                currentMoveMode = MoveMode.Idle;
            }
        }
    }

    //逃げる人が宝箱から離れたら
    public void OnTriggerExit(Collider other)
    {
        if (other.gameObject.tag =="Human")
        {
            followTarget = other.transform;
            //followTarget = null;
            CancelInvoke("Chest");
            if (currentMoveMode == MoveMode.Idle)
            {
                gameDirector.characterList[roomHubModel.ConnectionId].GetComponent<HumanManager>().DropTreasure();
                currentMoveMode = MoveMode.Follow;
                //humanManager. DropTreasure(); // 宝箱を離れたら、持っていない状態に戻す
            }
        }
        
    }

    //定期的に呼ぶメソッド(宝箱の位置同期)
    public void Chest()
    {
        gameDirector.MoveChest(this.gameObject.transform.position,this.gameObject.transform.rotation, this.gameObject.name);
    }
}
