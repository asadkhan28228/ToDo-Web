using Microsoft.EntityFrameworkCore;
using ToDo.DAL.Context;
using ToDo.DAL.Entities;
using ToDo.DAL.IRepository;
using ToDo.DAL.SQLiteEntities;

namespace ToDo.DAL.Repository
{
    public class ProjectRepository : IProjectRepository
    {
        private readonly ToDoContext context;
        private readonly SqliteToDoContext sqliteContext;

        public ProjectRepository(
            ToDoContext context,
            SqliteToDoContext sqliteContext)
        {
            this.context = context;
            this.sqliteContext = sqliteContext;
        }
        // ============================
        // GET ALL PROJECTS
        // ============================
        public async Task<List<Tasks>> GetAllProjectsAsync()
        {
            return await context.Tasks
                .ToListAsync();
        }

        // ============================
        // GET PROJECT BY ID
        // ============================
        public async Task<Tasks?> GetProjectByIdAsync(int id)
        {
            return await context.Tasks
                .FirstOrDefaultAsync(x => x.Id == id);
        }

        // ============================
        // ADD PROJECT
        // Direct SQL Server save
        // ============================
        public async Task AddProjectAsync(Tasks project)
        {
            if (project.CreatedAt == default)
            {
                project.CreatedAt = DateTime.Now;
            }

            var localTask = new LocalTask
            {
                Title = project.Title,
                Description = project.Description,
                DueDate = project.DueDate,
                Priority = project.Priority,
                Status = project.Status,
                UserId = project.UserId,
                CreatedAt = project.CreatedAt,
                SyncStatus = "Pending"
            };

            // Pehle sirf SQLite mein save hoga
            await sqliteContext.LocalTasks.AddAsync(localTask);

            await sqliteContext.SaveChangesAsync();
        }

        // ============================
        // UPDATE PROJECT
        // Direct SQL Server update
        // ============================
        public async Task UpdateAsync(Tasks project)
        {
            context.Tasks.Update(project);

            await context.SaveChangesAsync();
        }

        // ============================
        // DELETE PROJECT
        // Direct SQL Server delete
        // ============================
        public async Task DeleteAsync(int id)
        {
            var project =
                await context.Tasks
                    .FirstOrDefaultAsync(x => x.Id == id);

            if (project == null)
            {
                return;
            }

            context.Tasks.Remove(project);

            await context.SaveChangesAsync();
        }

        // ============================
        // SEARCH PROJECTS
        // ============================
        public async Task<List<Tasks>> SearchProjectsAsync(
            string id,
            string title)
        {
            var query =
                context.Tasks.AsQueryable();

            if (!string.IsNullOrWhiteSpace(id))
            {
                query = query.Where(
                    x => x.Id.ToString().Contains(id));
            }

            if (!string.IsNullOrWhiteSpace(title))
            {
                query = query.Where(
                    x => x.Title.Contains(title));
            }

            return await query.ToListAsync();
        }

        // ============================
        // SAVE CHANGES
        // ============================
        public async Task SaveChangesAsync()
        {
            await context.SaveChangesAsync();
        }
    }
}