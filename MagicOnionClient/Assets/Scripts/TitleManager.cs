//==========================================================
//
//�^�C�g���Ǘ�����
//Author:���{�S��
//
//==========================================================
using DG.Tweening;
using MessagePack.Formatters.MagicOnionServer.Model.Entity;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using static UnityEngine.Rendering.DebugUI;

/// <summary>
/// �^�C�g�����Ǘ����Ă���X�N���v�g
/// </summary>
public class TitleManager : BaseModel
{
    [SerializeField] InputField inputFieldName;//���O����͂���t�B�[���h
    [SerializeField] InputField InputFieldUserID;//Debug�p���[�U�[��ID���̓t�B�[���h

    [SerializeField] Text InputText;//���O���͂��ꂽ�e�L�X�g
    [SerializeField] Text ErrorText;//�G���[�e�L�X�g

    [SerializeField] UserModel userModel;//���[�U�[���f���N���X��`
    [SerializeField] GameObject StartButton;//�X�^�[�g�{�^��

    [SerializeField] CanvasGroup buttonCanvasGroup; // �{�^���ɒǉ�����CanvasGroup
    [SerializeField] float fadeDuration = 0.5f; // �t�F�[�h�̎���
    [SerializeField] float fadeDelay = 0.5f; // ���̃t�F�[�h�܂ł̒x��

    [SerializeField]AudioClip startbuttonSE;
    AudioSource audioSource;


    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        // �{�^���̃t�F�[�h�C���ƃt�F�[�h�A�E�g���J��Ԃ�
        buttonCanvasGroup.DOFade(0, fadeDuration) // �t�F�[�h�A�E�g
            .SetLoops(-1, LoopType.Yoyo) // ���[�v�ݒ�BYoyo�͍s�����藈����B
            .SetEase(Ease.InOutQuad) // �C�[�W���O����
            .SetDelay(fadeDelay); // �t�F�[�h�Ԃ̒x��
    }

    void Update()
    {

    }

    //�X�^�[�g�{�^���������ꂽ��Ăяo��
    public void OnStart()
    {
        if (!string.IsNullOrEmpty(InputFieldUserID.text))
        {
            UserModel.Instance.userId = int.Parse(InputFieldUserID.text);
            audioSource.PlayOneShot(startbuttonSE);
            Initiate.Fade("MachingScene", Color.black, 1.0f);
            return;
        }

        //���[�U�[�̃f�[�^��ǂݍ���
        bool isSuccess = UserModel.Instance.LoadUserData();

        if (isSuccess == true)
        {
            audioSource.PlayOneShot(startbuttonSE);
            Initiate.Fade("MachingScene", Color.black, 1.0f);
        }
        else
        {
            inputFieldName.gameObject.SetActive(true);
            StartButton.SetActive(false);
            //OKButton.SetActive(true);
        }
    }

    //���[�U�[�o�^API�Ăяo��
    public async void OnSubmitButton(string name)
    {
        name = inputFieldName.text;
        if (!string.IsNullOrEmpty(name))
        {
            await userModel.RegistUserAsync(name);
            audioSource.PlayOneShot(startbuttonSE);
            Initiate.Fade("MachingScene", Color.black, 1.0f);
        }
        else
        {
            ErrorText.gameObject.SetActive(true);
            ErrorText.text = "�����O��������x���͂��Ă�������";
        }

    }
}
