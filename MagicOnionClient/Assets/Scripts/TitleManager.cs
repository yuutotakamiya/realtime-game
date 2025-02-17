//==========================================================
//
//タイトル管理処理
//Author:高宮祐翔
//
//==========================================================
using Cysharp.Threading.Tasks;
using DG.Tweening;
using MessagePack.Formatters.MagicOnionServer.Model.Entity;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using static UnityEngine.Rendering.DebugUI;

/// <summary>
/// タイトルを管理しているスクリプト
/// </summary>
public class TitleManager : BaseModel
{
    [SerializeField] InputField inputFieldName;//名前を入力するフィールド
    [SerializeField] InputField InputFieldUserID;//Debug用ユーザーのID入力フィールド

    [SerializeField] Text InputText;//名前入力されたテキスト
    [SerializeField] Text ErrorText;//エラーテキスト

    [SerializeField] UserModel userModel;//ユーザーモデルクラス定義
    [SerializeField] GameObject StartButton;//スタートボタン
    [SerializeField] GameObject SubmitButton;
 
    [SerializeField] CanvasGroup buttonCanvasGroup; // ボタンに追加したCanvasGroup
    [SerializeField] float fadeDuration = 0.5f; // フェードの時間
    [SerializeField] float fadeDelay = 0.5f; // 次のフェードまでの遅延

    [SerializeField]AudioClip startbuttonSE;
    AudioSource audioSource;

    /// <summary>
    /// 開始処理
    /// </summary>
    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        // ボタンのフェードインとフェードアウトを繰り返す
        buttonCanvasGroup.DOFade(0, fadeDuration) // フェードアウト
            .SetLoops(-1, LoopType.Yoyo) // ループ設定。Yoyoは行ったり来たり。
            .SetEase(Ease.InOutQuad) // イージング効果
            .SetDelay(fadeDelay); // フェード間の遅延
    }

    void Update()
    {

    }

    /// <summary>
    /// スタートボタンが押されたら呼び出す
    /// </summary>
    public void OnStart()
    {
        if (!string.IsNullOrEmpty(InputFieldUserID.text))
        {
            UserModel.Instance.UserID = int.Parse(InputFieldUserID.text);
            audioSource.PlayOneShot(startbuttonSE);
            Initiate.Fade("MachingScene", Color.black, 1.0f);
            return;
        }

        //ユーザーのデータを読み込む
        bool isSuccess = UserModel.Instance.LoadUserData();

        if (!isSuccess)
        {
            inputFieldName.gameObject.SetActive(true);
            StartButton.SetActive(false);
            SubmitButton.SetActive(true);
        }
        else
        {
            audioSource.PlayOneShot(startbuttonSE);
            Initiate.Fade("MachingScene", Color.black, 1.0f);
        }
    }

    /// <summary>
    /// ユーザー登録API呼び出し
    /// </summary>
    /// <param name="name"></param>
    /// <returns></returns>
    public async void RegistUserAsync(string name)
    {
        name = inputFieldName.text;
        if (!string.IsNullOrEmpty(name))
        {
            await UserModel.Instance.RegistUserAsync(name);
            audioSource.PlayOneShot(startbuttonSE);
            Initiate.Fade("MachingScene", Color.black, 1.0f);
        }
        else
        {
            ErrorText.gameObject.SetActive(true);
            ErrorText.text = "※名前をもう一度入力してください";
        }

    }
}
