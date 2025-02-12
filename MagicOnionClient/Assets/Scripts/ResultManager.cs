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
    [SerializeField]  GameObject playerResultPrefab;// プレイヤーリザルト表示用のPrefab
    [SerializeField]  Transform resultPanel;// リザルトを表示するパネル

    static List<ResultData> resultData;

    /// <summary>
    /// 開始処理
    /// </summary>
    void Start()
    {
        foreach (var result in resultData)
        {
            // プレハブを生成して、結果パネル（resultPanel）に追加
            GameObject resultUI = Instantiate(playerResultPrefab, resultPanel);

            // プレハブ内のTextコンポーネントを取得し、データを設定
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
    /// タイトル画面に遷移
    /// </summary>
    public void Title()
    {
        Initiate.Fade("Title", Color.black, 1);
    }

    /// <summary>
    /// リザルト画面に表示するためのデータ
    /// </summary>
    /// <param name="Data"></param>
    public static void SetResult(List<ResultData> Data)
    {
        resultData = Data;
    }

}

