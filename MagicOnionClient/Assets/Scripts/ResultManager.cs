using Cysharp.Threading.Tasks;
using Shared.Interfaces.StreamingHubs;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;
using static UnityEngine.Rendering.DebugUI;

public class ResultManager : MonoBehaviour
{
    [SerializeField]  GameObject playerResultPrefab;// �v���C���[���U���g�\���p��Prefab
    [SerializeField]  Transform resultPanel;// ���U���g��\������p�l��

    static List<ResultData> resultData;

    /// <summary>
    /// �J�n����
    /// </summary>
    void Start()
    {
        foreach (var result in resultData)
        {
            // �v���n�u�𐶐����āA���ʃp�l���iresultPanel�j�ɒǉ�
            GameObject resultUI = Instantiate(playerResultPrefab, resultPanel);

            // �v���n�u����Text�R���|�[�l���g���擾���A�f�[�^��ݒ�
            resultUI.gameObject.transform.Find("NameText").GetComponent<Text>().text = result.Name;
            resultUI.gameObject.transform.Find("KillCountText").GetComponent<Text>().text = "Kills: " + result.KillNum.ToString();
            resultUI.gameObject.transform.Find("ChestNumText").GetComponent<Text>().text = "Chests: " + result.ChestNum.ToString();
            resultUI.gameObject.transform.Find("PointText").GetComponent<Text>().text = "Points: " + result.Point.ToString();

        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    /// <summary>
    /// �^�C�g����ʂɑJ��
    /// </summary>
    public void Title()
    {
        Initiate.Fade("Title", Color.black, 1);
    }

    /// <summary>
    /// ���U���g��ʂɕ\�����邽�߂̃f�[�^
    /// </summary>
    /// <param name="Data"></param>
    public static void SetResult(List<ResultData> Data)
    {
        resultData = Data;
    }

}

