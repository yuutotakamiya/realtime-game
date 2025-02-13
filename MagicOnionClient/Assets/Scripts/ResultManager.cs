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

    /*// �v���C���[�̖��O��\������Text�R���|�[�l���g
    [SerializeField] Text NameText;

    // �v���C���[���擾�����󔠂̐���\������Text�R���|�[�l���g
    [SerializeField] Text ChestNumText;

    // �v���C���[�̃L������\������Text�R���|�[�l���g
    [SerializeField] Text KillCountText;

    // �v���C���[�̃|�C���g��\������Text�R���|�[�l���g
    [SerializeField] Text PointText;*/

    static List<ResultData> resultData;

    /// <summary>
    /// �J�n����
    /// </summary>
    void Start()
    {
        // resultData���ݒ肳��Ă��Ȃ��ꍇ�̓G���[�n���h�����O
        if (resultData != null && resultData.Count > 0)
        {
            for (int i = 0; i < resultData.Count; i++)
            {
                // �v���n�u�𐶐����āA���ʃp�l���iresultPanel�j�ɒǉ�
                GameObject resultUI = Instantiate(playerResultPrefab, resultPanel);

                // �e�v���C���[�̃��U���g�f�[�^��ݒ�
                resultUI.transform.Find("NameText").GetComponent<Text>().text = resultData[i].Name;
                resultUI.transform.Find("ChestNumText").GetComponent<Text>().text = "Chests:  " + resultData[i].ChestNum.ToString();
                resultUI.transform.Find("KillCountText").GetComponent<Text>().text = "Kills:  " + resultData[i].KillCount.ToString();
                resultUI.transform.Find("PointText").GetComponent<Text>().text = "Points:  " + resultData[i].Point.ToString();
            }
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

