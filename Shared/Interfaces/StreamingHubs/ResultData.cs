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
        /// <summary>
        /// キルした数
        /// </summary>
        [Key(0)]
        public int KillCount { get; set; }
        /// <summary>
        /// 宝箱の獲得した数
        /// </summary>
        [Key(1)]
        public int ChestNum { get; set; }
        /// <summary>
        /// ポイント
        /// </summary>
        [Key(2)]
        public int Point { get; set; }
        /// <summary>
        /// 自分の名前
        /// </summary>
        [Key(3)]
        public string Name { get; set; }
    }

}
