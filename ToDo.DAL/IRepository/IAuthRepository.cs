using System;
using System.Collections.Generic;
using System.Text;
using ToDo.DAL.Entities;

namespace ToDo.DAL.IRepository
{
    public interface IAuthRepository
    {
        Task<User> GetUserByEmailAsync(string email);

        Task<User> GetUserByIdAsync(int id);

        Task<User> GetUserByRefreshToken(string refreshToken);

        Task AddUserAsync(User user);

        Task UpdateUserAsync(User user  );

        Task SaveChangesAsync();
    }
}
