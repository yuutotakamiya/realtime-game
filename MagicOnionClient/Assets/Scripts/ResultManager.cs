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

    /*// プレイヤーの名前を表示するTextコンポーネント
    [SerializeField] Text NameText;

    // プレイヤーが取得した宝箱の数を表示するTextコンポーネント
    [SerializeField] Text ChestNumText;

    // プレイヤーのキル数を表示するTextコンポーネント
    [SerializeField] Text KillCountText;

    // プレイヤーのポイントを表示するTextコンポーネント
    [SerializeField] Text PointText;*/

    static List<ResultData> resultData;

    /// <summary>
    /// 開始処理
    /// </summary>
    void Start()
    {
        // resultDataが設定されていない場合はエラーハンドリング
        if (resultData != null && resultData.Count > 0)
        {
            for (int i = 0; i < resultData.Count; i++)
            {
                // プレハブを生成して、結果パネル（resultPanel）に追加
                GameObject resultUI = Instantiate(playerResultPrefab, resultPanel);

                // 各プレイヤーのリザルトデータを設定
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

