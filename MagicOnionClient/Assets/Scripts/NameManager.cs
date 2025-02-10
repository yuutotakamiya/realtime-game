//==========================================================
//
//マッチングで表示する名前を管理
//Author:高宮祐翔
//
//==========================================================
using Cysharp.Threading.Tasks.Triggers;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// マッチングで名前を表示するスクリプト
/// </summary>
public class NameManager : MonoBehaviour
{
    [SerializeField] Text NameText;
    /// <summary>
    /// 最初関数
    /// </summary>
    void Start()
    {
        if(NameText == null)
        {
            return;
        }
    }

    /// <summary>
    /// 名前を表示する関数
    /// </summary>
    /// <param name="Name"></param>
    public void Name(string Name)
    {
        NameText.text = Name;
    }
}
