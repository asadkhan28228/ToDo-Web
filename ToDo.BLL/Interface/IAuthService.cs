 using System;
using System.Collections.Generic;
using System.Text;
using ToDo.BLL.Dto.Auth;

namespace ToDo.BLL.Interface
{
    public interface IAuthService
    {
        Task<string> RegisterAsync(RegisteredDto registeredDto);

        Task<AuthReponse> LoginAsync(loginDto loginDto);
        //Task<AuthReponse> RefreshTokenAsync(string token);

        //Task<string> LogoutAsync(string refreshToken);
    }
}
