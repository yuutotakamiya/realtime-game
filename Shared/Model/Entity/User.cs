using MagicOnionServer.Model.Entity;
using MessagePack;
using System;
using System.Collections.Generic;
using System.Text;

namespace MagicOnionServer.Model.Entity
{
    [MessagePackObject]
    public class User
    {
        [Key(0)]
        public int Id { get; set; }
        [Key(1)]
        public string Name { get; set; }
        [Key(2)]
        public string Token { get; set; }
        [Key(3)]
        public DateTime Created_at { get; set; }
        [Key(4)]
        public DateTime Updated_at { get; set; }
    }
}
