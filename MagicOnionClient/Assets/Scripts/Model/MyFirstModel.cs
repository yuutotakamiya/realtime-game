using Cysharp.Net.Http;
using Cysharp.Threading.Tasks;
using Grpc.Net.Client;
using MagicOnion.Client;
using Shared.Interfaces.Services;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MyFirstModel : MonoBehaviour
{
    const string ServerURL = "http://localhost:7000";

    // Start is called before the first frame update
    async void Start()
    {
        /*Sum(100, 323, result =>
        {
            Debug.Log(result);
        });*/

        /*int result = await Sum(100, 323);
       
           Debug.Log(result);*/

        /*int[] num = new int[2] { 1,2 };

        int Toatal = await SumAll(num);

        Debug.Log(Toatal);*/

        /*int[] Total = await CalcForOperationAsync(1,2);
        Debug.Log(Total[0]);
        Debug.Log(Total[1]);
        Debug.Log(Total[2]);
        Debug.Log(Total[3]);*/


       Number number = new Number();
        number.x = 1;
        number.y = 2;
       float  num2  = await SumAllNumberAsync(number);
        Debug.Log(num2);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    /*public async void Sum(int x, int y, Action<int> callback)
    {
        using var handler = new YetAnotherHttpHandler() { Http2Only = true };
        var channel = GrpcChannel.ForAddress(ServerURL, new GrpcChannelOptions() { HttpHandler = handler });
        var client= MagicOnionClient.Create<IMyFirstService>(channel);
        var result = await client.SumAsync(x, y);
        callback?.Invoke(result);
    }*/

    public async UniTask<int> Sum(int x, int y)
    {
        using var handler = new YetAnotherHttpHandler() { Http2Only = true };
        var channel = GrpcChannel.ForAddress(ServerURL, new GrpcChannelOptions() { HttpHandler = handler });
        var client = MagicOnionClient.Create<IMyFirstService>(channel);
        var result = await client.SumAsync(x, y);
        return result;
    }

    public async UniTask<int> SumAll(int[] numList)
    {
        using var handler = new YetAnotherHttpHandler() { Http2Only = true };
        var channel = GrpcChannel.ForAddress(ServerURL, new GrpcChannelOptions() { HttpHandler = handler });
        var client = MagicOnionClient.Create<IMyFirstService>(channel);
        var result = await client.SumAllAsync(numList);
        return result;
    }


    public async UniTask<int[]> CalcForOperationAsync(int x,int y)
    {
        using var handler = new YetAnotherHttpHandler() { Http2Only = true };
        var channel = GrpcChannel.ForAddress(ServerURL, new GrpcChannelOptions() { HttpHandler = handler });
        var client = MagicOnionClient.Create<IMyFirstService>(channel);
        var result = await client.CalcForOperationAsync(x,y);
        return result;
    }

    public async UniTask<float> SumAllNumberAsync(Number numArray)
    {
        using var handler = new YetAnotherHttpHandler() { Http2Only = true };
        var channel = GrpcChannel.ForAddress(ServerURL, new GrpcChannelOptions() { HttpHandler = handler });
        var client = MagicOnionClient.Create<IMyFirstService>(channel);
        var result = await client.SumAllNumberAsync(numArray);
        return result;
    }
}

