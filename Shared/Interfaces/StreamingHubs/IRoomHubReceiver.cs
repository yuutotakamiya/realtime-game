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
        void OnMove(Guid connectionId,Vector3 pos,Quaternion rotaition,CharacterState characterState);

        //ユーザーの準備
        void OnReady(Guid connectionId,bool isReady);

        //制限時間
        void OnTimer(Guid connectionId, float time);

        public enum CharacterState
        {
            Idel = 0,
            Run = 1,
            Magic= 2,
            Dead = 3

        }
    }
}
