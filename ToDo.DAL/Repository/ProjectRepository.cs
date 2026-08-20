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


        // =====================================================
        // GET ALL PROJECTS
        // Synced tasks SQL Server se
        // =====================================================
        public async Task<List<Tasks>> GetAllProjectsAsync()
        {
            return await context.Tasks
                .AsNoTracking()
                .ToListAsync();
        }


        // =====================================================
        // GET PENDING LOCAL TASKS
        //
        // Jo task SQLite me save ho chuka hai
        // lekin abhi SQL Server me sync nahi hua
        // =====================================================
        public async Task<List<LocalTask>> GetPendingLocalTasksAsync()
        {
            return await sqliteContext.LocalTasks
                .AsNoTracking()
                .Where(x => x.SyncStatus == "Pending")
                .ToListAsync();
        }


        // =====================================================
        // GET PROJECT BY ID
        // SQL Server se
        // =====================================================
        public async Task<Tasks?> GetProjectByIdAsync(int id)
        {
            return await context.Tasks
                .FirstOrDefaultAsync(x => x.Id == id);
        }


        // =====================================================
        // ADD PROJECT
        //
        // IMPORTANT:
        // Pehle sirf SQLite me save hoga.
        //
        // TaskSyncService 10 seconds baad
        // SQL Server me save karega.
        // =====================================================
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


            // SQLite me save
            await sqliteContext.LocalTasks
                .AddAsync(localTask);


            await sqliteContext.SaveChangesAsync();
        }


        // =====================================================
        // UPDATE PROJECT
        //
        // Filhal existing synced task
        // directly SQL Server me update hoga.
        // =====================================================
        public async Task UpdateAsync(Tasks project)
        {
            context.Tasks.Update(project);

            await context.SaveChangesAsync();
        }


        // =====================================================
        // DELETE PROJECT
        //
        // Filhal SQL Server se delete hoga.
        // =====================================================
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


        // =====================================================
        // SEARCH PROJECTS
        // SQL Server se search
        // =====================================================
        public async Task<List<Tasks>> SearchProjectsAsync(
            string? id,
            string? title)
        {
            var query =
                context.Tasks
                    .AsNoTracking()
                    .AsQueryable();


            // Search by ID
            if (!string.IsNullOrWhiteSpace(id))
            {
                query = query.Where(
                    x => x.Id.ToString().Contains(id));
            }


            // Search by Title
            if (!string.IsNullOrWhiteSpace(title))
            {
                query = query.Where(
                    x => x.Title.Contains(title));
            }


            return await query
                .ToListAsync();
        }


        // =====================================================
        // SAVE CHANGES
        // =====================================================
        public async Task SaveChangesAsync()
        {
            await context.SaveChangesAsync();
        }
    }
}