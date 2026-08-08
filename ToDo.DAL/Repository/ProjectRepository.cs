using Microsoft.EntityFrameworkCore;
using ToDo.DAL.Context;
using ToDo.DAL.Entities;
using ToDo.DAL.IRepository;


namespace ToDo.DAL.Repository
{
    public class ProjectRepository : IProjectRepository
    {
        private readonly ToDoContext context;

        public ProjectRepository(ToDoContext context)
        {
            this.context = context;
        }
        

        public async Task<List<Tasks>> GetAllProjectsAsync()
        {
            return await context.Tasks.ToListAsync();
        }
        public async Task<Tasks> GetProjectByIdAsync(int id)
        {
            return await context.Tasks.FindAsync(id);
        }
        public async Task AddProjectAsync(Tasks project)
        {
            await context.Tasks.AddAsync(project);

            await context.SaveChangesAsync();
        }

        public async Task UpdateAsync(Tasks project)
        {
            context.Tasks.Update(project);

            await context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var project = await context.Tasks.FindAsync(id);

            if (project != null)
            {
                context.Tasks.Remove(project);

                await context.SaveChangesAsync();
            }

        }

        public async Task<List<Tasks>> SearchProjectsAsync(string id, string Title)
        {
            return await context.Tasks
                .Where(p => p.Id.ToString().Contains(id) || p.Title.Contains(Title))
                .ToListAsync();
        }
        public async Task SaveChangesAsync()
        {
            await context.SaveChangesAsync();
        }
    }
}