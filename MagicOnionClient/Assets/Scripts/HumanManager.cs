using Cysharp.Threading.Tasks.Triggers;
using Shared.Interfaces.StreamingHubs;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Shared.Interfaces.StreamingHubs.IRoomHubReceiver;

public class HumanManager : Character
{
    //[SerializeField] GameDirector gameDirector;
    //public bool isAttack = false;
    // Start is called before the first frame update
    //Character character;
    [SerializeField] GameObject HumanGameOverText;
    [SerializeField] Character character;
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
            // 衝突したオブジェクトが自分ではない場合
            if (other.gameObject != this.gameObject)
            {
                GameObject weapon = GameObject.Find("Mesh_Weapon_01");

                if (weapon != null && weapon.CompareTag("weapon"))
                {
                    animator.SetInteger("state", 3);

                    isstart = false;
                    Destroy(this.gameObject);
                }
              
            }
        }
    }

    public void DestroyObject()
    {
        // オブジェクトを破壊
        //Destroy(this.gameObject);
    }
}

