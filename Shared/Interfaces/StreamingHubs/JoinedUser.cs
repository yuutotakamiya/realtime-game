using MagicOnionServer.Model.Entity;
using MessagePack;
using System;
using System.Collections.Generic;
using System.Text;

namespace Shared.Interfaces.StreamingHubs
{
    /// <summary>
    /// フィールド設定
    /// </summary>
    [MessagePackObject]
    public class JoinedUser
    {
        [Key(0)]
        public Guid ConnectionId { get; set; }//接続ID

        [Key(1)]
        public User UserData { get; set; }//ユーザー情報

        [Key(2)]
        public int JoinOrder { get; set; }//参加順番
    }

}
