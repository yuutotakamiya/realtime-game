//==========================================================
//
//宝箱を置くポイントを管理する処理
//Author:高宮祐翔
//
//==========================================================
using Cysharp.Threading.Tasks.Triggers;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 宝箱がプレイヤーを追従するスクリプト
/// </summary>
public class ChestPoint : MonoBehaviour
{
    /// <summary>
    /// クラス設定
    /// </summary>
    [SerializeField] GameDirector gameDirector;//GameDirectorクラスの設定
    [SerializeField] Character character;//Characterクラスの設定
    [SerializeField] DefenceTarget defenceTarget;//DefenceTargetクラスの設定
    [SerializeField] RoomHubModel roomHubModel;//RoomHubModelクラスの設定

    private bool isChestProcessed = false;//2回入らないようにするためのフラグ

    /// <summary>
    /// 宝箱が宝箱を置くポイントと当たったら
    /// </summary>
    /// <param name="other"></param>
    public async void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Chest")&&!isChestProcessed)
        {
            isChestProcessed = true;

            //gameDirecterで呼び出した結果をCharacterクラスの変数に代入
            Character foundCharacter = gameDirector.GetCharacter();

            //自分が持っている宝箱と宝箱を置く場所に置いた宝箱が一緒だったら
            if (foundCharacter.currentTreasureChest == other.gameObject)
            {
                await gameDirector.GainChest();//宝箱取得同期
            }

            Destroy(other.gameObject);//宝箱を削除
            gameDirector.CancelInvoke("MoveChest");
            defenceTarget.CancelInvoke("Chest");
            isChestProcessed=false;
        }
    }
}
