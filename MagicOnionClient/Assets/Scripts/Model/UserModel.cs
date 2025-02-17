//==========================================================
//
//ユーザー管理処理
//Author:高宮祐翔
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
/// ユーザーを管理しているスクリプト
/// </summary>
public class UserModel : BaseModel
{
    private int userId;//ユーザーIDを保存する変数
    private string authToken;//トークンを保存するための変数

    private static UserModel instance;

    //userIdをプロパティ化
    public int UserID
    {
       get { return userId; }
       set { userId = value; }
    }

    //UserModelクラスをインスタンス化
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
    /// ユーザーIDをローカルファイルに保存する
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
    /// ユーザーIDをローカルファイルから読み込む
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
    /// ユーザー登録API
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
            //登録成功
            userId = await client.RegistUserAsync(name);
            SaveUserData();
            Debug.Log("登録成功");
            return true;
        }
        catch(RpcException e)
        {
            //登録失敗
            Debug.Log(e);
            return false;
        }
        
    }
}
