using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using ToDo.DAL.Context;
using ToDo.DAL.Entities;

namespace ToDo_Web.Services
{
    public class TaskSyncService : BackgroundService
    {
        private readonly IServiceScopeFactory scopeFactory;
        private readonly ILogger<TaskSyncService> logger;

        public TaskSyncService(
            IServiceScopeFactory scopeFactory,
            ILogger<TaskSyncService> logger)
        {
            this.scopeFactory = scopeFactory;
            this.logger = logger;
        }


        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    await SyncPendingTasks();
                }
                catch (Exception ex)
                {
                    logger.LogError(ex,"SQLite to SQL Server synchronization failed.");
                }
                // Har 1 minute baad sync
                await Task.Delay(TimeSpan.FromMinutes(1),stoppingToken);
            }
        }


        private async Task SyncPendingTasks()
        {
            using var scope = scopeFactory.CreateScope();

            var sqliteContext =scope.ServiceProvider.GetRequiredService<SqliteToDoContext>();

            var sqlContext = scope.ServiceProvider.GetRequiredService<ToDoContext>();

            // Sirf Pending records uthao
            var pendingTasks =await sqliteContext.LocalTasks.Where(x =>x.SyncStatus == "Pending").ToListAsync();

            if (!pendingTasks.Any())
            {
                return;
            }


            foreach (var localTask in pendingTasks)
            {
                try
                {
                    var sqlTask = new Tasks
                    {
                        Title = localTask.Title,

                        Description =localTask.Description,

                        DueDate =localTask.DueDate,

                        Priority =localTask.Priority,

                        Status =localTask.Status,

                        UserId =localTask.UserId,

                        CreatedAt =localTask.CreatedAt
                    };


                    // SQL Server me save
                    await sqlContext.Tasks.AddAsync(sqlTask);

                    await sqlContext.SaveChangesAsync();


                    // SQL Server generated ID
                    // SQLite me store karo
                    localTask.SqlServerId =sqlTask.Id;

                    localTask.SyncStatus ="Synced";

                    localTask.SyncedAt =DateTime.Now;


                    sqliteContext.LocalTasks.Update(localTask);

                    await sqliteContext.SaveChangesAsync();


                    logger.LogInformation("Task {LocalId} synced successfully.",localTask.LocalId);
                }
                catch (Exception ex)
                {
                    // Failed record Pending hi rahe
                    localTask.SyncStatus ="Pending";

                    await sqliteContext.SaveChangesAsync();

                    logger.LogError(ex,"Task {LocalId} could not sync.",localTask.LocalId);
                }
            }
        }
    }
}