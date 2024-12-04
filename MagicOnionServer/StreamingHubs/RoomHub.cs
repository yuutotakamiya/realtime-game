using MagicOnion.Server.Hubs;
using MagicOnionServer.Model.Context;
using MagicOnionServer.Model.Entity;
using Shared.Interfaces.StreamingHubs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using static Shared.Interfaces.StreamingHubs.IRoomHubReceiver;

namespace StreamingHubs
{
    public class RoomHub : StreamingHubBase<IRoomHub, IRoomHubReceiver>, IRoomHub
    {
        private IGroup room;

        //ユーザー入室
        public async Task<JoinedUser[]> JoinAsync(string roomName, int userId)
        {
            //ルームに参加&ルーム保持
            this.room = await this.Group.AddAsync(roomName);

            //DBからユーザー情報取得
            GameDbContext context = new GameDbContext();
            var user = context.Users.Where(user => user.Id == userId).First();


            //グループストレージにユーザーデータを格納
            var roomStorage = this.room.GetInMemoryStorage<RoomData>();
            var joinedUser = new JoinedUser() { ConnectionId = this.ConnectionId, UserData = user };
            var roomData = new RoomData() { JoinedUser = joinedUser };
            roomStorage.Set(this.ConnectionId, roomData);
            joinedUser.JoinOrder = roomStorage.AllValues.Count();

            /*ルーム参加者全員に(自分を含む)、ユーザーの入室通知を送信
            this.Broadcast(room).OnJoin(joinedUser);*/

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

        //ユーザーの退室
        public async Task LeaveAsync()
        {
            //グループデータから削除
            this.room.GetInMemoryStorage<RoomData>().Remove(this.ConnectionId);

            //ルーム内のメンバーから自分を削除
            await room.RemoveAsync(this.Context);

            var joinedUser = new JoinedUser() { ConnectionId = this.ConnectionId };

            //ルーム参加者全員に(自分以外)、ユーザーの退室通知を送信
            this.Broadcast(room).OnLeave(joinedUser);

        }

        //ユーザーの移動、回転、アニメーション
        public async Task MoveAsync(Vector3 pos, Quaternion rotaition,CharacterState characterState)
        {
            //RoomDataの情報を取得
            var roomStorage = this.room.GetInMemoryStorage<RoomData>();
            //グループストレージにある自身の接続IDを取得
            var roomData = roomStorage.Get(this.ConnectionId);

            //位置と回転を更新
            roomData.Position = pos;
            roomData.Rotation = rotaition;
            roomStorage.Set(this.ConnectionId, roomData);  // 更新されたデータを保存

            //ルーム参加者全員に(自分以外)、ユーザーの位置、回転、アニメーションを通知
            this.BroadcastExceptSelf(room).OnMove(this.ConnectionId, pos, rotaition,characterState);
        }

        //ユーザーの準備
        public async Task ReadyAsync()
        {
            //準備できたことをRoomDataに保存
            var roomStorage = this.room.GetInMemoryStorage<RoomData>();
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

        //ゲームの制限時間処理
        public async Task TimeAsync(float time)
        {
            var roomStorage = this.room.GetInMemoryStorage<RoomData>();
            var roomData = roomStorage.Get(this.ConnectionId);
            roomData.Timer = time;

            this.Broadcast(room).OnTimer(this.ConnectionId,time);
        }

        //ユーザーが切断したときの処理
        protected override ValueTask OnDisconnected()
        {
            //グループデータから削除
            this.room.GetInMemoryStorage<RoomData>().Remove(this.ConnectionId);

            var joinedUser = new JoinedUser() { ConnectionId = this.ConnectionId };
            //ルーム参加者全員に(自分以外)、ユーザーの退室通知を送信
            this.Broadcast(room).OnLeave(joinedUser);

            //ルーム内のメンバーから自分を削除
            room.RemoveAsync(this.Context);

            return CompletedTask;
        }
    }
}
