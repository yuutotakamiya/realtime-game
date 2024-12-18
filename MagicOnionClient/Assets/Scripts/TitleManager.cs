using MessagePack.Formatters.MagicOnionServer.Model.Entity;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static UnityEngine.Rendering.DebugUI;

public class TitleManager : MonoBehaviour
{
    [SerializeField] InputField inputField;

    [SerializeField] Text InputText;

    [SerializeField] UserModel userModel;

    // Start is called before the first frame update
    void Start()
    {
        inputField = GameObject.Find("InputField"). GetComponent<InputField>();
        InputText = InputText.GetComponent<Text>();
       
    }
    // Update is called once per frame
    void Update()
    {

    }

    //ユーザー登録API呼び出し
    public async void InputOnButton(string name)
    {
       
        name = inputField.text;
        await userModel.RegistUserAsync(name);
        Initiate.Fade("Game",Color.black,1.0f);
    }
}
