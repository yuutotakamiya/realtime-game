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

            //gameDirecter�ŌĂяo�������ʂ�Character�N���X�̕ϐ��ɑ��
            Character foundCharacter = gameDirector.GetCharacter();

            //�����������Ă���󔠂ƕ󔠂�u���ꏊ�ɒu�����󔠂��ꏏ��������
            if (foundCharacter.currentTreasureChest == other.gameObject)
            {
                await gameDirector.GainChest();//�󔠎擾����
            }

            Destroy(other.gameObject);

            defenceTarget.CancelInvoke("Chest");

            //HumanManager.DropTreasure();
        }
    }
}
