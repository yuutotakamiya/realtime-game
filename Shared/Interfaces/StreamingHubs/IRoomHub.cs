using MagicOnion;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using static Shared.Interfaces.StreamingHubs.IRoomHubReceiver;

namespace Shared.Interfaces.StreamingHubs
{
    public interface IRoomHub : IStreamingHub<IRoomHub,IRoomHubReceiver>
    {
        //クライアント側からサーバー側を呼び出す関数を定義

        //ユーザー入室
        Task<JoinedUser[]> JoinAsync(string roomName, int userId);

        //ユーザーの退室
        Task LeaveAsync();

        //ユーザーの移動、回転、アニメーション
        Task MoveAsync(Vector3 pos ,Quaternion rotaition,CharacterState state);

        //ユーザーの準備
        Task ReadyAsync();

        //ゲームの制限時間
        Task TimeAsync(float time);

        //鬼のキル数
        Task KillAsync();

        //自動マッチング同期
        Task<JoinedUser[]> JoinLobbyAsync(int userId);

        //ゲーム内オブジェクトの同期
        Task MoveChest(Vector3 pos,Quaternion rotaition, string Namechest);
    }
}
