using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{

    public Transform target; // カメラが追従するプレイヤーのTransform
    public float distance = 5.0f; // プレイヤーからの距離
    public float height = 2.0f; // プレイヤーの上にあるカメラの高さ
    public float rotationSpeed = 5.0f; // カメラの回転速度

    private float horizontalInput; // 水平方向のマウス入力
    private float verticalInput; // 垂直方向のマウス入力

    // Start is called before the first frame update
    void Start()
    {
        //最初のプレイヤーの位置の取得
        //pastPos = player.transform.position;
    }

    // Update is called once per frame
    void LateUpdate()
    {
        // マウスの入力を取得
        horizontalInput += Input.GetAxis("Mouse X") * rotationSpeed;
        verticalInput -= Input.GetAxis("Mouse Y") * rotationSpeed;

        // 垂直方向のカメラ角度を制限（上下の動きを制御）
        verticalInput = Mathf.Clamp(verticalInput, -30f, 60f);

        // カメラの回転計算
        Quaternion rotation = Quaternion.Euler(verticalInput, horizontalInput, 0);

        // カメラの位置をプレイヤーの背後に設定
        Vector3 position = target.position - (rotation * Vector3.forward * distance) + Vector3.up * height;

        // カメラの位置と回転を設定
        transform.position = position;
        transform.LookAt(target.position + Vector3.up * height);
    }
}
