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


        // ============================================
        // GET ALL PROJECTS
        // Data SQL Server se read hoga
        // ============================================

        public async Task<List<Tasks>> GetAllProjectsAsync()
        {
            return await context.Tasks.ToListAsync();
        }


        // ============================================
        // GET PROJECT BY ID
        // ============================================

        public async Task<Tasks?> GetProjectByIdAsync(int id)
        {
            return await context.Tasks.FindAsync(id);
        }


        // ============================================
        // ADD PROJECT
        //
        // 1. SQLite me save
        // 2. SQL Server me save
        // 3. SQLite ko Synced mark
        // ============================================

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

            // Sirf SQLite me save
            await sqliteContext.LocalTasks.AddAsync(localTask);

            await sqliteContext.SaveChangesAsync();
        }

        // ============================================
        // UPDATE PROJECT
        //
        // 1. SQLite update
        // 2. SQL Server update
        // 3. SQLite Synced
        // ============================================

        public async Task UpdateAsync(Tasks project)
        {
            // SQL Server ID ki help se
            // SQLite record find karenge

            var localTask =
                await sqliteContext.LocalTasks
                    .FirstOrDefaultAsync(
                        x => x.SqlServerId == project.Id);


            // ----------------------------------------
            // STEP 1: SQLite update
            // ----------------------------------------

            if (localTask != null)
            {
                localTask.Title = project.Title;

                localTask.Description = project.Description;

                localTask.DueDate = project.DueDate;

                localTask.Priority = project.Priority;

                localTask.Status = project.Status;

                localTask.UserId = project.UserId;

                localTask.SyncStatus = "PendingUpdate";


                sqliteContext.LocalTasks.Update(localTask);

                await sqliteContext.SaveChangesAsync();
            }


            try
            {
                // ----------------------------------------
                // STEP 2: SQL Server update
                // ----------------------------------------

                context.Tasks.Update(project);

                await context.SaveChangesAsync();


                // ----------------------------------------
                // STEP 3: SQLite synced
                // ----------------------------------------

                if (localTask != null)
                {
                    localTask.SyncStatus = "Synced";

                    localTask.SyncedAt = DateTime.Now;


                    sqliteContext.LocalTasks.Update(localTask);

                    await sqliteContext.SaveChangesAsync();
                }
            }
            catch
            {
                // SQL Server update fail
                // SQLite me updated data rahega

                if (localTask != null)
                {
                    localTask.SyncStatus = "PendingUpdate";

                    sqliteContext.LocalTasks.Update(localTask);

                    await sqliteContext.SaveChangesAsync();
                }

                throw;
            }
        }


        // ============================================
        // DELETE PROJECT
        //
        // 1. SQLite PendingDelete
        // 2. SQL Server delete
        // 3. SQLite delete
        // ============================================

        public async Task DeleteAsync(int id)
        {
            // SQL Server task
            var project =
                await context.Tasks.FindAsync(id);


            if (project == null)
            {
                return;
            }


            // SQLite local task
            var localTask =
                await sqliteContext.LocalTasks
                    .FirstOrDefaultAsync(
                        x => x.SqlServerId == id);


            // ----------------------------------------
            // STEP 1: SQLite ko PendingDelete karo
            // ----------------------------------------

            if (localTask != null)
            {
                localTask.SyncStatus = "PendingDelete";


                sqliteContext.LocalTasks.Update(localTask);

                await sqliteContext.SaveChangesAsync();
            }


            try
            {
                // ----------------------------------------
                // STEP 2: SQL Server se delete
                // ----------------------------------------

                context.Tasks.Remove(project);

                await context.SaveChangesAsync();


                // ----------------------------------------
                // STEP 3: SQLite se bhi delete
                // ----------------------------------------

                if (localTask != null)
                {
                    sqliteContext.LocalTasks.Remove(localTask);

                    await sqliteContext.SaveChangesAsync();
                }
            }
            catch
            {
                // SQL Server delete fail ho gaya
                // SQLite record PendingDelete rahega

                throw;
            }
        }


        // ============================================
        // SEARCH PROJECT
        // SQL Server se search
        // ============================================

        public async Task<List<Tasks>> SearchProjectsAsync(
            string id,
            string title)
        {
            var query = context.Tasks.AsQueryable();


            if (!string.IsNullOrWhiteSpace(id))
            {
                query = query.Where(
                    p => p.Id.ToString().Contains(id));
            }


            if (!string.IsNullOrWhiteSpace(title))
            {
                query = query.Where(
                    p => p.Title.Contains(title));
            }


            return await query.ToListAsync();
        }


        // ============================================
        // SAVE CHANGES
        // Existing interface ke liye
        // ============================================

        public async Task SaveChangesAsync()
        {
            await context.SaveChangesAsync();
        }
    }
}