using Microsoft.EntityFrameworkCore;
using ToDo.DAL.Context;
using ToDo.DAL.Entities;
using ToDo.DAL.IRepository;

namespace ToDo.DAL.Repository
{
    public class ProjectRepository : IProjectRepository
    {
        private readonly ToDoDbContext context;

        public ProjectRepository(ToDoDbContext context)
        {
            this.context = context;
        }
        

        public async Task<List<Project>> GetAllProjectsAsync()
        {
            return await context.Projects.ToListAsync();
        }
        public async Task<Project> GetProjectByIdAsync(int id)
        {
            return await context.Projects.FindAsync(id);
        }
        public async Task AddProjectAsync(Project project)
        {
            await context.Projects.AddAsync(project);

            await context.SaveChangesAsync();
        }

        public async Task UpdateAsync(Project project)
        {
            context.Projects.Update(project);

            await context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var project = await context.Projects.FindAsync(id);

            if (project != null)
            {
                context.Projects.Remove(project);

                await context.SaveChangesAsync();
            }

        }

        public async Task<List<Project>> SearchProjectsAsync(string id, string Title)
        {
            return await context.Projects
                .Where(p => p.Id.ToString().Contains(id) || p.Title.Contains(Title))
                .ToListAsync();
        }
        public async Task SaveChangesAsync()
        {
            await context.SaveChangesAsync();
        }
    }
}