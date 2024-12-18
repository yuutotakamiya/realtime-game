using MessagePack.Formatters.MagicOnionServer.Model.Entity;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using static UnityEngine.Rendering.DebugUI;

public class TitleManager : BaseModel
{
    [SerializeField] InputField inputField;

    [SerializeField] Text InputText;

    [SerializeField] UserModel userModel;

    [SerializeField] GameObject StartButton;

    [SerializeField] GameObject OKButton;

    [SerializeField] Text ErrorText;

    void Start()
    {
        //inputField = GameObject.Find("InputField"). GetComponent<InputField>();
        //InputText = InputText.GetComponent<Text>();
    }

    void Update()
    {

    }

    //�X�^�[�g�{�^���������ꂽ��Ăяo��
    public void OnStart()
    {
        bool isSuccess = UserModel.Instance.LoadUserData();

        if (isSuccess == true)
        {
            Initiate.Fade("Game", Color.black, 1.0f);
        }
        else
        {
            inputField.gameObject.SetActive(true);
            StartButton.SetActive(false);
            OKButton.SetActive(true);
        }
    }

    //���[�U�[�o�^API�Ăяo��
    public async void OnSubmitButton(string name)
    {
        name = inputField.text;
        if (!string.IsNullOrEmpty(name))
        {
            await userModel.RegistUserAsync(name);
            Initiate.Fade("Game", Color.black, 1.0f);
        }
        else
        {
            ErrorText.gameObject.SetActive(true);
            ErrorText.text = "�����O��������x���͂��Ă�������";
        }

    }
}
