using Shared.Interfaces.StreamingHubs;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace StreamingHubs
{
    public class RoomData
    {
        //参加ユーザー
        public JoinedUser JoinedUser { get; set; }
        //位置
        public Vector3 Position { get; set; }
        //回転
        public Quaternion Rotation { get; set; }
        //準備完了
        public bool IsReady {  get; set; }
        //制限時間
        public float Timer { get; set; }
        //キルした数
        public int KillNum {  get; set; }
        //宝箱の数
        public int ChestNum {  get; set; }
        //ゲーム終了
        public bool IsEndGame {  get; set; }
    }
}
