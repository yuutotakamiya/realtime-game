using MagicOnion;
using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Text;

namespace Shared.Interfaces.Services
{
    /// <summary>
    /// ユーザー登録を行うAPI
    /// </summary>
    public interface IUserService : IService<IUserService>
    {
        UnaryResult<int> RegistUserAsync(string name);
    }
    
}
