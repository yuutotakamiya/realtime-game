//==========================================================
//
//�}�b�`���O�ŕ\�����閼�O���Ǘ�
//Author:���{�S��
//
//==========================================================
using Cysharp.Threading.Tasks.Triggers;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// �}�b�`���O�Ŗ��O��\������X�N���v�g
/// </summary>
public class NameManager : MonoBehaviour
{
    [SerializeField] Text NameText;
    /// <summary>
    /// �ŏ��֐�
    /// </summary>
    void Start()
    {
        if(NameText == null)
        {
            return;
        }
    }

    /// <summary>
    /// ���O��\������֐�
    /// </summary>
    /// <param name="Name"></param>
    public void Name(string Name)
    {
        NameText.text = Name;
    }
}
