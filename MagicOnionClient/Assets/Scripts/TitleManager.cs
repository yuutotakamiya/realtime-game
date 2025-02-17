//==========================================================
//
//�^�C�g���Ǘ�����
//Author:���{�S��
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
    [SerializeField] GameObject SubmitButton;
 
    [SerializeField] CanvasGroup buttonCanvasGroup; // �{�^���ɒǉ�����CanvasGroup
    [SerializeField] float fadeDuration = 0.5f; // �t�F�[�h�̎���
    [SerializeField] float fadeDelay = 0.5f; // ���̃t�F�[�h�܂ł̒x��

    [SerializeField]AudioClip startbuttonSE;
    AudioSource audioSource;

    /// <summary>
    /// �J�n����
    /// </summary>
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

    /// <summary>
    /// �X�^�[�g�{�^���������ꂽ��Ăяo��
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

        //���[�U�[�̃f�[�^��ǂݍ���
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
    /// ���[�U�[�o�^API�Ăяo��
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
            ErrorText.text = "�����O��������x���͂��Ă�������";
        }

    }
}
