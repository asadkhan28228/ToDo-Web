using Org.BouncyCastle.Crypto.Generators;
using System;
using System.Collections.Generic;
using System.Text;
using ToDo.BLL.Dto.Auth;
using ToDo.BLL.Interface;
using ToDo.DAL.Entities;
using ToDo.DAL.IRepository;
using ToDo.DAL.Repository;

namespace ToDo.BLL.Services
{
    public class AuthServices : IAuthService
    {
        private readonly IAuthRepository _authRepository;
        private readonly IJwtService jwtService;

        public AuthServices(IAuthRepository authRepository,IJwtService jwtService)
        {
            _authRepository = authRepository;
            this.jwtService = jwtService;
        }

        public async Task<string> RegisterAsync(RegisteredDto registeredDto)
        {
            var existinguser = await _authRepository.GetUserByEmailAsync(registeredDto.Email);
            if (existinguser != null)
            {
               return("User with this email already exists.");
            }

            var newUser = new User
            {
                Email = registeredDto.Email,
                FullName = registeredDto.FullName,
                PasswordHash = registeredDto.Password // In a real application, you should hash the password before storing it  
            };

            await _authRepository.AddUserAsync(newUser);
            await _authRepository.SaveChangesAsync();

            return "User registered successfully.";
        }

        public async Task<AuthReponse> LoginAsync(loginDto loginDto)
        {
            var user = await _authRepository.GetUserByEmailAsync(loginDto.Email);
            if (user == null)
            {
                return null;
            }

            //bool passwordIsValid = BCrypt.Net.BCrypt.Verify(loginDto.Password,user.PasswordHash);

            //if (!passwordIsValid)
            //{
                //return null;
            //}

            var token = jwtService.GenerateToken(user);

            var expire = DateTime.Now.AddDays(7);

            user.Token = token;
            user.ExpiresAt = expire;
            user.IsRevoked = true;


            await _authRepository.SaveChangesAsync();

            return new AuthReponse
            {
                Token = token,
                ExpiresAt = DateTime.UtcNow.AddHours(1), // Example expiration time
                FullName = user.FullName,
                Email = user.Email,
                UserId = user.Id
            };
        }
    }
}
