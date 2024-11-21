using System;
using System.Collections.Generic;
using System.Text;

namespace Shared.Interfaces.StreamingHubs
{
    public interface IRoomHubReceiver
    {
        //ここにサーバー側からクライアント側を呼び出す関数定義

        //ユーザーの入室通知
        void OnJoin(JoinedUser user);

        //ユーザーの退室通知
        void OnLeave(JoinedUser user);
    }
}
