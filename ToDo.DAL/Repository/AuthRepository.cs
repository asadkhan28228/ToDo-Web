using Microsoft.EntityFrameworkCore;
using ToDo.DAL.Context;
using ToDo.DAL.Entities;
using ToDo.DAL.IRepository;

namespace ToDo.DAL.Repository
{
    public class AuthRepository : IAuthRepository
    {
        private readonly ToDoContext context;

        public AuthRepository(ToDoContext context)
        {
            this.context = context;
        }

        public async Task<User> GetUserByEmailAsync(string email)
        {
            return await context.Users.FirstOrDefaultAsync(u => u.Email == email);
        }

        public async Task<User> GetUserByIdAsync(int id)
        {
            return await context.Users.FirstOrDefaultAsync(u => u.Id == id);
        }

        public async Task<User> GetUserByRefreshToken(string refreshToken)
        {
            return await context.Users.FirstOrDefaultAsync(u => u.Token == refreshToken);
        }

        public async Task AddUserAsync(User user)
        {
            await context.Users.AddAsync(user);
        }

        public async Task UpdateUserAsync(User user)
        {
            context.Users.Update(user);
        }

        public async Task SaveChangesAsync()
        {
            await context.SaveChangesAsync();
        }
    }
}
