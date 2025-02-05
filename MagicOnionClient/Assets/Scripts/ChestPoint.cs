using Cysharp.Threading.Tasks.Triggers;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ChestPoint : MonoBehaviour
{
    [SerializeField] GameDirector gameDirector;
    [SerializeField] Character character;
    [SerializeField] DefenceTarget defenceTarget;
    [SerializeField] RoomHubModel roomHubModel;

    private bool isChestProcessed = false;

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

            Destroy(other.gameObject);

            defenceTarget.CancelInvoke("Chest");

            //HumanManager.DropTreasure();
        }
    }
}
