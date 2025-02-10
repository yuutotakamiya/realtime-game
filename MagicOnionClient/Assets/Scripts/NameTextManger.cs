//==========================================================
//
//名前表示を常にカメラの方向に向ける処理
//Author:高宮祐翔
//
//==========================================================
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 名前を表示しているTextを常にカメラの方向に向けるスクリプト
/// </summary>
public class NameTextManger : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
      
    }

    // Update is called once per frame
    void Update()
    {
        transform.LookAt(Camera.main.transform);

        // カメラの上下反転を防ぐため、Y軸の回転のみを適用
        Vector3 eulerRotation = transform.rotation.eulerAngles;
        transform.rotation = Quaternion.Euler(0f, eulerRotation.y, 0f);
    }
}
