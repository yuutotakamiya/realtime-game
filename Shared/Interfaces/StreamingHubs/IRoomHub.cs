﻿using MagicOnion;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Shared.Interfaces.StreamingHubs
{
    public interface IRoomHub : IStreamingHub<IRoomHub,IRoomHubReceiver>
    {
        //クライアント側からサーバー側を呼び出す関数を定義

        //ユーザー入室
        Task<JoinedUser[]> JoinAsync(string roomName, int userId);

        //ユーザーの退室
        Task LeaveAsync();

        //ユーザーの移動
        Task MoveAsync(Vector3 pos);

    }
}