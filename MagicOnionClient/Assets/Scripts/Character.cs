using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Move : MonoBehaviour
{
    [SerializeField] float speed;

    public bool isself = false;

    [SerializeField] float rotationSpeed; // 回転速度

    private Vector3 movement; // 移動ベクトル



    // Start is called before the first frame update
    void Start()
    {
        




    }

    // Update is called once per frame
    void Update()
    {


        movement = Vector3.zero; // 毎フレーム移動ベクトルをリセット

        if (!isself) return;

        //右に移動
        if (Input.GetKey(KeyCode.D))
        {
            //transform.position += transform.right * speed * Time.deltaTime;
            //this.transform.Rotate(0, 1f, 0);
            movement += Vector3.right; // X軸の正方向
        } 
         //左に移動
        if (Input.GetKey(KeyCode.A))
        {
            //transform.position -= transform.right * speed * Time.deltaTime;
            //this.transform.Rotate(0, -1f, 0);
            movement += Vector3.left; // X軸の負方向
        }
        //前に移動    
        if (Input.GetKey(KeyCode.W))
        {
            //transform.position += transform.forward * speed * Time.deltaTime;
            movement += Vector3.forward; // Z軸の正方向

        }
        //後ろに移動   
        if (Input.GetKey(KeyCode.S))
        {
            //transform.position -= transform.forward * speed * Time.deltaTime;
            movement += Vector3.back; // Z軸の負方向
        }


        if (movement != Vector3.zero)
        {
            Quaternion targetRotation = Quaternion.LookRotation(movement);
            // スムーズに回転させる
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
        }

        // オブジェクトを移動させる
        transform.Translate(movement.normalized * speed * Time.deltaTime, Space.World);
    }
}
