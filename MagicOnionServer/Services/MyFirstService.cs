using MagicOnion;
using MagicOnion.Server;
using Shared.Interfaces.Services;
namespace MagicOnionServer.Services
{
    public class MyFirstService : ServiceBase<IMyFirstService>,IMyFirstService
    {
        public async UnaryResult<int> SumAsync(int x, int y)
        {
            Console.WriteLine("Received:" + x + "," + y);
            return x + y;
        }

        public async UnaryResult<int> SumAllAsync(int[] numList)
        {
            int Total = 0;

            foreach (int num in numList)
            {
                Total += num;
                Console.WriteLine(num);
               
            }
            return Total;
        }

        public async UnaryResult<int[]> CalcForOperationAsync(int x,int y)
        {
            int[] num = new int[4];

            num[0] = x + y;
            num[1] = x - y;
            num[2] = x * y;    
            num[3] = x / y;

            return num;
        }

        public async UnaryResult<float> SumAllNumberAsync(Number numArray)
        {
            return numArray.x + numArray.y;
        }
    }
}
