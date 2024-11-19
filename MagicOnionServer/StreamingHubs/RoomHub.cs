using MagicOnion.Server.Hubs;
using MagicOnionServer.Model.Context;
using Shared.Interfaces.StreamingHubs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StreamingHubs
{
    public class RoomHub: StreamingHubBase<IRoomHub,IRoomHubReceiver>, IRoomHub
    {
        private IGroup room;

        //ユーザー入室
       public async Task<JoinedUser[]> JoinAsync(string roomName, int userId)
       {
            //ルームに参加&ルーム保持
            this.room = await this.Group.AddAsync(roomName);

            //DBからユーザー情報取得
            GameDbContext context= new GameDbContext();
            var user = context.Users.Where(user => user.Id == userId).First();


            //グループストレージにユーザーデータを格納
            var roomStorage = this.room.GetInMemoryStorage<RoomData>();
            var joinedUser = new JoinedUser() { ConnectionId = this.ConnectionId,UserData = user};
            var roomData = new RoomData() { JoinedUser = joinedUser };
            roomStorage.Set(this.ConnectionId,roomData);


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
    }
}
