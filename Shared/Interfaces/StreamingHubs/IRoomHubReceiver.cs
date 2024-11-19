using System;
using System.Collections.Generic;
using System.Text;

namespace Shared.Interfaces.StreamingHubs
{
    public interface IRoomHubReceiver
    {
        //ここにサーバー側からクライアント側を呼び出す関数定義

        void OnJoin(JoinedUser user);
    }
}
