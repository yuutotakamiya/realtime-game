﻿using MagicOnion.Server.Hubs;
using MagicOnionServer.Model.Context;
using MagicOnionServer.Model.Entity;
using Shared.Interfaces.StreamingHubs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.Xml;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using static Shared.Interfaces.StreamingHubs.IRoomHubReceiver;

namespace StreamingHubs
{
    public class RoomHub : StreamingHubBase<IRoomHub, IRoomHubReceiver>, IRoomHub
    {
        private IGroup room;

        /// <summary>
        /// ユーザー入室
        /// </summary>
        /// <param name="roomName"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        public async Task<JoinedUser[]> JoinAsync(string roomName, int userId)//スレッドセーフ
        {
            //ルームに参加&ルーム保持
            this.room = await this.Group.AddAsync(roomName);

            //DBからユーザー情報取得
            GameDbContext context = new GameDbContext();
            var user = context.Users.Where(user => user.Id == userId).First();

            //グループストレージにユーザーデータを格納
            var roomStorage = this.room.GetInMemoryStorage<RoomData>();

            //排他制御(全員で何か共有している時)
            lock (roomStorage)
            {
                var joinedUser = new JoinedUser() { ConnectionId = this.ConnectionId, UserData = user };
                var roomData = new RoomData() { JoinedUser = joinedUser };
                roomStorage.Set(this.ConnectionId, roomData);

                //ルーム内の情報を取得
                joinedUser.JoinOrder = roomStorage.AllValues.Count();

                bool isOrderAssigned = false;

                //最大の人数分ループ
                for (int i = 0; i < 4; ++i)
                {
                    bool isOrderUsed = false;
                    // 既に使われている JoinOrder があるかチェック
                    foreach (var roomJoinedUser in roomStorage.AllValues)
                    {
                        if (roomJoinedUser.JoinedUser.JoinOrder == i)
                        {
                            isOrderUsed = true;
                            break;
                        }
                    }
                    // 空いているJoinOrderを見つけた場合
                    if (!isOrderUsed)
                    {
                        joinedUser.JoinOrder = i; // 空いているJoinOrderを割り当て
                        isOrderAssigned = true;
                        break;
                    }

                }

                //ルーム参加者全員に(自分以外)、ユーザーの入室通知を送信
                this.BroadcastExceptSelf(room).OnJoin(joinedUser);

                RoomData[] roomDataList = roomStorage.AllValues.ToArray();

                //参加者中のユーザー情報を返す
                JoinedUser[] joinedUserList = new JoinedUser[roomDataList.Length];

                for (int i = 0; i < roomDataList.Length; i++)
                {
                    joinedUserList[i] = roomDataList[i].JoinedUser;
                }

                return joinedUserList;
            }
        }

        /// <summary>
        /// ユーザーの退室
        /// </summary>
        /// <returns></returns>
        public async Task LeaveAsync()
        {
            //グループデータから削除
            this.room.GetInMemoryStorage<RoomData>().Remove(this.ConnectionId);

            //ルーム内のメンバーから自分を削除
            await room.RemoveAsync(this.Context);

            var joinedUser = new JoinedUser() { ConnectionId = this.ConnectionId };

            //ルーム参加者全員にユーザーの退室通知を送信
            this.Broadcast(room).OnLeave(joinedUser);

        }

        /// <summary>
        /// ユーザーの移動、回転、アニメーション
        /// </summary>
        /// <param name="pos"></param>
        /// <param name="rotaition"></param>
        /// <param name="characterState"></param>
        /// <returns></returns>
        public async Task MoveAsync(Vector3 pos, Quaternion rotaition, CharacterState characterState)
        {
            //RoomDataの情報を取得
            var roomStorage = this.room.GetInMemoryStorage<RoomData>();
            //グループストレージにある自身の接続IDを取得
            var roomData = roomStorage.Get(this.ConnectionId);

            //位置と回転を更新
            roomData.Position = pos;
            roomData.Rotation = rotaition;
            roomStorage.Set(this.ConnectionId, roomData);  // 更新されたデータを保存

            //ルーム参加者(自分以外)に、ユーザーの位置、回転、アニメーションを通知
            this.BroadcastExceptSelf(room).OnMove(this.ConnectionId, pos, rotaition, characterState);
        }

        /// <summary>
        /// ユーザーの準備
        /// </summary>
        /// <returns></returns>
        public async Task ReadyAsync()//スレッドセーフ
        {
            //準備できたことをRoomDataに保存
            var roomStorage = this.room.GetInMemoryStorage<RoomData>();

            //排他制御(全員で何か共有しているとき)
            lock (roomStorage)
            {
                var roomData = roomStorage.Get(this.ConnectionId);
                roomData.IsReady = true;

                //準備できたかどうか
                bool isReady = false;
                var roomDataList = roomStorage.AllValues.ToArray<RoomData>();
                foreach (var rData in roomDataList)
                {
                    if (!rData.IsReady)
                    {
                        isReady = false;
                        break;
                    }
                    else
                    {
                        isReady = true;

                    }
                }

                if (isReady == true)
                {
                    //ルーム内の全員が準備完了していたら、ゲーム開始を通知
                    this.Broadcast(room).OnReady(this.ConnectionId, isReady);
                }
            }

        }

        /// <summary>
        /// ゲームの制限時間処理
        /// </summary>
        /// <param name="time"></param>
        /// <returns></returns>
        public async Task TimeAsync(float time)
        {
            var roomStorage = this.room.GetInMemoryStorage<RoomData>();
            var roomData = roomStorage.Get(this.ConnectionId);
            var joinedUser = new JoinedUser() { ConnectionId = this.ConnectionId };
            roomData.Timer = time;//タイマーの更新

            if (time == 0)
            {
                await EndGameAsync(false);
            }

            //ルーム内の全員に現在の制限時間を通知
            this.Broadcast(room).OnTimer(joinedUser, time);
        }

        /// <summary>
        /// 鬼のキル数更新処理
        /// </summary>
        /// <returns></returns>
        public async Task KillAsync()
        {
            var roomStorage = this.room.GetInMemoryStorage<RoomData>();
            var roomData = roomStorage.Get(this.ConnectionId);
            roomData.KillNum++;
            var roomDataList = roomStorage.AllValues.ToArray<RoomData>();

            int totalKillNum = 0;

            foreach (var rData in roomDataList)
            {
                totalKillNum += rData.KillNum;
            }

            //ルーム内の全員に誰をキルしたかを通知
            this.Broadcast(room).OnKill(this.ConnectionId, totalKillNum, roomData.JoinedUser.UserData.Name);
        }

        /// <summary>
        /// 宝箱の獲得合計数同期
        /// </summary>
        /// <returns></returns>
        public async Task GainChest()
        {
            var roomStorage = this.room.GetInMemoryStorage<RoomData>();
            var roomData = roomStorage.Get(this.ConnectionId);
            roomData.ChestNum++;
            var roomDataList = roomStorage.AllValues.ToArray<RoomData>();

            //宝箱の合計数を格納
            int totalChestNum = 0;

            //プレイヤー毎の宝箱を格納する変数
            Dictionary<string, int> keyValuePairs = new Dictionary<string, int>();

            // すべてのプレイヤーのデータをループし、宝箱数をカウント
            foreach (var rData in roomDataList)
            {
                // 各プレイヤーの名前をキーに、宝箱数をディクショナリに追加
                keyValuePairs[rData.JoinedUser.UserData.Name] = rData.ChestNum;

                // 合計宝箱数に加算
                totalChestNum += rData.ChestNum;
            }

            if (totalChestNum == 2)
            {
                await EndGameAsync(true);
            }

            //ルーム内の全員に宝箱の獲得合計数を通知
            this.Broadcast(room).OnChestNum(totalChestNum, keyValuePairs);

        }

        //自動マッチング処理
        public async Task<JoinedUser[]> JoinLobbyAsync(int userId)
        {
            JoinedUser[] joinedUserList = await JoinAsync("Lobby", userId);
            //排他制御
            lock (joinedUserList)
            {
                //最低4人集まっていたら
                if (joinedUserList.Length >= 4)
                {
                    Guid guid = Guid.NewGuid();
                    string roomName = guid.ToString();//ランダムなルーム名を生成
                    this.Broadcast(room).OnMatching(roomName);
                }
            }

            return joinedUserList;
        }


        /// <summary>
        /// 宝箱の位置同期
        /// </summary>
        /// <param name="pos"></param>
        /// <param name="rotaition"></param>
        /// <param name="Namechest"></param>
        /// <returns></returns>
        public async Task MoveChest(Vector3 pos, Quaternion rotaition, string Namechest)
        {
            //RoomDataの情報を取得
            var roomStorage = this.room.GetInMemoryStorage<RoomData>();
            //グループストレージにある自身の接続IDを取得
            var roomData = roomStorage.Get(this.ConnectionId);

            //宝箱の位置を更新
            roomData.Position = pos;
            roomData.Rotation = rotaition;
            roomStorage.Set(this.ConnectionId, roomData);  // 更新されたデータを保存

            //ルーム参加者(自分以外)に、宝箱位置、回転、宝箱の名前を通知
            this.BroadcastExceptSelf(room).OnMoveChest(pos, rotaition, Namechest);
        }

        /// <summary>
        /// ゲーム終了同期
        /// </summary>
        /// <returns></returns>
        public async Task EndGameAsync(bool isEndGame)
        {
            Console.WriteLine("EndGameAsync start");

            var roomStorage = this.room.GetInMemoryStorage<RoomData>();

            var roomDataList = roomStorage.AllValues.ToArray<RoomData>();

            int point = 10;

            using var context = new GameDbContext();

            List<ResultData> resultData = new List<ResultData>();

            foreach (var roomData in roomDataList)
            {
                // ユーザーのIDを取得
                int userId = roomData.JoinedUser.UserData.Id;

                // データベースからユーザー情報を取得
                var user = await context.Users.FindAsync(userId);

                user.point += point;

                //リザルトデータを作成
                resultData.Add(new ResultData
                {
                    Name = roomData.JoinedUser.UserData.Name,
                    ChestNum = roomData.ChestNum,
                    KillCount = roomData.KillNum,
                    Point = user.point,
                });

                await context.SaveChangesAsync();
            }

            //ゲーム終了、リザルトデータを通知
            this.Broadcast(room).OnEndGame(isEndGame, resultData);
        }


        /// <summary>
        /// ユーザーが切断したときの処理
        /// </summary>
        /// <returns></returns>
        protected override ValueTask OnDisconnected()
        {
            //グループデータから削除
            this.room.GetInMemoryStorage<RoomData>().Remove(this.ConnectionId);

            var joinedUser = new JoinedUser() { ConnectionId = this.ConnectionId };

            //ルーム参加者全員に、ユーザーの退室通知を送信
            this.Broadcast(room).OnLeave(joinedUser);

            //ルーム内のメンバーから自分を削除
            room.RemoveAsync(this.Context);

            return CompletedTask;
        }
    }
}
