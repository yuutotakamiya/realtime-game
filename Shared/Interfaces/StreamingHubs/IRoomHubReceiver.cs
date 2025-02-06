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
        void OnMove(Guid connectionId,Vector3 pos,Quaternion rotaition, CharacterState  characterState);

        //ユーザーの準備
        void OnReady(Guid connectionId,bool isReady);

        //制限時間
        void OnTimer(JoinedUser user, float time);

        //アニメーションの宣言
        public enum CharacterState
        {
            Idle = 0,
            Run = 1,
            Attack= 2,
            Dead = 3
        }

        //鬼のキル数通知
        void OnKill(Guid connectionId, int killnum, string username);

        //自動マッチング通知
        void OnMatching(string roomName);

        //宝箱の位置を通知
        void OnMoveChest(Vector3 pos, Quaternion rotaition, string Namechest);

        //宝箱の獲得数を通知
        void OnChestNum(int TotalChestNum,Dictionary<string,int> keyValuePairs);

        //ゲーム終了通知
        void OnEndGame(bool isHumanEndGame);
    }
}
