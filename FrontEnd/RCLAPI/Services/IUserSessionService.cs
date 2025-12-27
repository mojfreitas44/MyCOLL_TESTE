using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RCLAPI.DTO;

namespace RCLAPI.Services
{
    public interface IUserSessionService
    {
        Task<Token> GetToken();
        Task Login(Token token);
        bool IsUserLoggedIn();
        Task Logout();
    }
}
