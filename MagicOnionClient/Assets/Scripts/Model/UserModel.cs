//==========================================================
//
//���[�U�[�Ǘ�����
//Author:���{�S��
//
//==========================================================
using Cysharp.Threading.Tasks;
using UnityEngine;
using Cysharp.Net.Http;
using Grpc.Net.Client;
using MagicOnion.Client;
using Shared.Interfaces.Services;
using Grpc.Core;
using System.IO;
using System;
using UnityEngine.Networking;
using Newtonsoft.Json;

/// <summary>
/// ���[�U�[���Ǘ����Ă���X�N���v�g
/// </summary>
public class UserModel : BaseModel
{
    private int userId;//���[�U�[ID��ۑ�����ϐ�
    private string authToken;//�g�[�N����ۑ����邽�߂̕ϐ�

    private static UserModel instance;

    //userId���v���p�e�B��
    public int UserID
    {
       get { return userId; }
       set { userId = value; }
    }

    //UserModel�N���X���C���X�^���X��
    public static UserModel Instance
    {
        get
        {
            if (instance == null)
            {
                GameObject gameObject = new GameObject("UserModel");
                instance = gameObject.AddComponent<UserModel>();
                DontDestroyOnLoad(gameObject);
            }
            return instance;
        }
    }
    
    /// <summary>
    /// ���[�U�[ID�����[�J���t�@�C���ɕۑ�����
    /// </summary>
    public void SaveUserData()
    {
        SaveData saveData = new SaveData();
        saveData.UserID = this.userId;
        //saveData.AuthToken = this.authToken;
        string json = JsonConvert.SerializeObject(saveData);
        var writer = new StreamWriter(Application.persistentDataPath + "/saveData.json");
        writer.Write(json);
        writer.Flush();
        writer.Close();
    }

    /// <summary>
    /// ���[�U�[ID�����[�J���t�@�C������ǂݍ���
    /// </summary>
    /// <returns></returns>
    public bool LoadUserData()
    {
        if (!File.Exists(Application.persistentDataPath + "/saveData.json"))
        {
            return false;
        }

        var reader = new StreamReader(Application.persistentDataPath + "/saveData.json");
        string json = reader.ReadToEnd();
        reader.Close();
        SaveData saveData = JsonConvert.DeserializeObject<SaveData>(json);
        this.userId = saveData.UserID;
        return true;
    }

    /// <summary>
    /// ���[�U�[�o�^API
    /// </summary>
    /// <param name="name"></param>
    /// <returns></returns>
    public async UniTask<bool> RegistUserAsync(string name)
    {
        using var handler = new YetAnotherHttpHandler() { Http2Only = true };
        var channel = GrpcChannel.ForAddress(ServerURL, new GrpcChannelOptions() { HttpHandler = handler });
        var client = MagicOnionClient.Create<IUserService>(channel);

        try
        {
            //�o�^����
            userId = await client.RegistUserAsync(name);
            SaveUserData();
            Debug.Log("�o�^����");
            return true;
        }
        catch(RpcException e)
        {
            //�o�^���s
            Debug.Log(e);
            return false;
        }
        
    }
}
