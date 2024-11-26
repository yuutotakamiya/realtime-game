using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace Shared.Interfaces.StreamingHubs
{
    public interface IRoomHubReceiver
    {
        //ここにサーバー側からクライアント側を呼び出す関数定義

        //ユーザーの入室通知
        void OnJoin(JoinedUser user);

        //ユーザーの退室通知
        void OnLeave(JoinedUser user);

        //ユーザーの移動通知
        void OnMove(Guid connectionId,Vector3 pos,Quaternion rotaition);
    }
}
