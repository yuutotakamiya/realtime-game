using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cysharp.Net.Http;
using Cysharp.Threading.Tasks;
using Grpc.Net.Client;
using MagicOnion.Client;
using Shared.Interfaces.Services;
using Grpc.Core;

public class UserModel : BaseModel
{
    private int userId;//�o�^���[�U�[ID


    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
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
