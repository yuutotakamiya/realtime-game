using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    private GameObject player;   //ユニティちゃんのオブジェクト情報を格納
    private Vector3 offset;      //カメラとの相対距離を格納
    void Start()
    {
        //ユニティちゃんのオブジェクト情報を格納
        this.player = GameObject.Find("wizard(Clone)");
        //メインカメラ(自身のオブジェクト)とユニティちゃんとトランスフォームの相対距離を算出
        offset = transform.position - player.transform.position;
    }

    void Update()
    {
        //メインカメラに相対距離を反映させた新しいトランスフォームの値をセットする
        transform.position = this.player.transform.position + offset;
    }
}
