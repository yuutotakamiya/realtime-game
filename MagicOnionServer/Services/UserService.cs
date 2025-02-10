using MagicOnion;
using MagicOnion.Server;
using MagicOnionServer.Model.Context;
using MagicOnionServer.Model.Entity;
using Shared.Interfaces.Services;
using MessagePack;
using System.Xml.Linq;

namespace MagicOnionServer.Services
{
    public class UserService : ServiceBase<IUserService>, IUserService
    {
        /// <summary>
        /// ユーザー登録API
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        /// <exception cref="ReturnStatusException"></exception>
        public async UnaryResult<int> RegistUserAsync(string name)
        {
            using var context = new GameDbContext();

            if (context.Users.Where(user=>user.Name==name).Count()>0)
            {
                throw new ReturnStatusException(Grpc.Core.StatusCode.InvalidArgument, "既に存在しています。");
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

        /// <summary>
        /// id指定でユーザー情報を取得するAPI
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public async UnaryResult<User> getUserAsync(int id)
        {
            using var context = new GameDbContext();

            User user = context.Users.Where(user => user.Id == id).First();
            return user;
        }

        /// <summary>
        /// ユーザー一覧を取得するAPI
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public async UnaryResult<User[]> GetUserListAsync()
        {
            using var context = new GameDbContext();

            User[] users = context.Users.ToArray();
            return users;
        }

        /// <summary>
        /// id指定でユーザー名を更新するAPI
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public async UnaryResult<bool> UpdateUser(int id, string name)
        {
            using var context = new GameDbContext();
            User user = context.Users.Where(user => user.Id == id).First();
            user.Name = "takamiya";
            await context.SaveChangesAsync();
            return true;
        }
    }
   
}
