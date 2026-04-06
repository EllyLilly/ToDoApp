using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ToDoApp.Core.Interfaces
{
    public interface IAuthService
    {
        Task<bool> RegisterAsync(string userName, string email, string password);
        Task<string> LoginAsync(string userName, string password);
    }
}
