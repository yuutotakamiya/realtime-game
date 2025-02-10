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
    public class ResultData
    {
        [Key(0)]
        //キルした数
        public int KillNum { get; set; }
        [Key(1)]
        //宝箱の数
        public int ChestNum { get; set; }
        [Key(2)]
        public int Point {  get; set; }
        [Key(3)]
        public string Name { get; set; }
    }

}
