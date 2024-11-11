using MagicOnion;
using MagicOnion.Server;
using MagicOnionServer.Model.Context;
using MagicOnionServer.Model.Entity;
using Shared.Interfaces.Services;

namespace MagicOnionServer.Services
{
    public class UserService : ServiceBase<IUserService>, IUserService
    {
        public async UnaryResult<int> RegistUserAsync(string name)
        {
            using var context = new GameDbContext();

            if (context.Users.Where(user=>user.Name==name).Count()>0)
            {
                throw new ReturnStatusException(Grpc.Core.StatusCode.InvalidArgument, "");
            }

            //テーブルにレコード追加
           User user = new User();
            user.Name = name;
            user.Token = "";
            user.Created_at = DateTime.Now;
            user.Updated_at = DateTime.Now;
            context.Users.Add(user);
            await context.SaveChangesAsync();
            return user.Id;
        }
    }
   
}
