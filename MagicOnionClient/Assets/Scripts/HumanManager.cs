using Cysharp.Threading.Tasks.Triggers;
using Shared.Interfaces.StreamingHubs;
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
            // 衝突したオブジェクトが自分ではない場合
            if (other.gameObject != this.gameObject)
            {
                GameObject weapon = GameObject.Find("Mesh_Weapon_01");

                if (weapon != null && weapon.CompareTag("weapon"))
                {
                    animator.SetInteger("state", 3);

                    isstart = false;
                    rb.velocity = Vector3.zero;

                    StartCoroutine(RespawnAfterDeath());
                }
              
            }
        }
    }

    // アニメーションが終わった後にリスポーンを呼び出す
    private IEnumerator RespawnAfterDeath()
    {
        // アニメーションの再生時間を待つ
        AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(0);
        float animationTime = stateInfo.length;

        yield return new WaitForSeconds(animationTime);

        // オブジェクトを削除し、リスポーン処理を呼ぶ
        OnAnimationDestroy();
        RespawnPlayer();
    }

    //アニメーションイベント用のメソッド
    public void OnAnimationDestroy()
    {
        //Destroy(this.gameObject);
        gameDirector. CancelInvoke("Move");
        
    }

    public void RespawnPlayer()
    {

        GameObject respawn;
        // ランダムにインデックスを選ぶ
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

        // 選ばれた場所にプレイヤーをワープ
        transform.position = respawn.transform.position;
        transform.rotation = respawn.transform .rotation;

        animator.SetInteger("state", 0);
        isstart = true;       
    }
    
}

