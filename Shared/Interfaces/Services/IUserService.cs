using MagicOnion;
using MagicOnionServer.Model.Entity;
using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Text;

namespace Shared.Interfaces.Services
{
   
    public interface IUserService : IService<IUserService>
    {
        /// <summary>
        /// ユーザー登録を行うAPI
        /// </summary>
        UnaryResult<int> RegistUserAsync(string name);

        /// <summary>
        ///  id指定でユーザー情報を取得するAPI
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        UnaryResult<User> getUserAsync(int id);

        /// <summary>
        /// ユーザー一覧を取得するAPI
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        UnaryResult<User[]> GetUserListAsync();


        /// <summary>
        /// id指定でユーザー名を更新するAPI
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        UnaryResult<bool> UpdateUser(int id, string name);
    }

    
}
