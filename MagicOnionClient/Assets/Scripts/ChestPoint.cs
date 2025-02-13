//==========================================================
//
//�󔠂�u���|�C���g���Ǘ����鏈��
//Author:���{�S��
//
//==========================================================
using Cysharp.Threading.Tasks.Triggers;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// �󔠂��v���C���[��Ǐ]����X�N���v�g
/// </summary>
public class ChestPoint : MonoBehaviour
{
    /// <summary>
    /// �N���X�ݒ�
    /// </summary>
    [SerializeField] GameDirector gameDirector;//GameDirector�N���X�̐ݒ�
    [SerializeField] Character character;//Character�N���X�̐ݒ�
    [SerializeField] DefenceTarget defenceTarget;//DefenceTarget�N���X�̐ݒ�
    [SerializeField] RoomHubModel roomHubModel;//RoomHubModel�N���X�̐ݒ�

    private bool isChestProcessed = false;//2�����Ȃ��悤�ɂ��邽�߂̃t���O

    /// <summary>
    /// �󔠂��󔠂�u���|�C���g�Ɠ���������
    /// </summary>
    /// <param name="other"></param>
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

            Destroy(other.gameObject);//�󔠂��폜
            gameDirector.CancelInvoke("MoveChest");
            defenceTarget.CancelInvoke("Chest");
            isChestProcessed=false;
        }
    }
}
