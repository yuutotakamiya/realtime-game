using MagicOnion;
using MessagePack;
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Security.Cryptography.X509Certificates;
using System.Text;

namespace Shared.Interfaces.Services
{
    [MessagePackObject]
    public class Number
    {
        [Key(0)]
        public float x;

        [Key(1)]
        public float y;
    }
    public interface IMyFirstService : IService<IMyFirstService>
    {
        /// <summary>
        /// 足し算処理を行う
        /// </summary>
        /// <param name="x">足す数</param>
        /// <param name="y">足される数</param>
        /// <returns></returns>
        UnaryResult<int> SumAsync(int x , int y);


        /// <summary>
        /// 受け取った配列を値の合計を返す
        /// </summary>
        /// <param name="numList"></param>
        /// <returns></returns>
        UnaryResult<int> SumAllAsync(int[] numList);

        /// <summary>
        /// x + yを[0]に、x-yを[1]に、x,yを[2]に、x/yを[3]に入れて配列で返す
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        UnaryResult<int[]> CalcForOperationAsync(int x, int y);

        /// <summary>
        /// xとyの小数をフィールドに持つNumberクラスを渡して、x+yの結果を返す
        /// </summary>
        /// <param name="numArray"></param>
        /// <returns></returns>
        UnaryResult<float> SumAllNumberAsync(Number numArray);
       
        
    }
}
