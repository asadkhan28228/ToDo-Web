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
                await Task.Delay(TimeSpan.FromSeconds(10),stoppingToken);
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

                    Console.WriteLine(
                        $"[{DateTime.Now:HH:mm:ss}] SQL SERVER SAVE: " +
                        $"Id={sqlTask.Id}, Title={sqlTask.Title}"
                    );
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












//using Microsoft.EntityFrameworkCore;
//using Microsoft.Extensions.DependencyInjection;
//using Microsoft.Extensions.Hosting;
//using Microsoft.Extensions.Logging;
//using ToDo.DAL.Context;
//using ToDo.DAL.Entities;

//namespace ToDo_Web.Services
//{
//    public class TaskSyncService : BackgroundService
//    {
//        private readonly IServiceScopeFactory scopeFactory;
//        private readonly ILogger<TaskSyncService> logger;

//        public TaskSyncService(
//            IServiceScopeFactory scopeFactory,
//            ILogger<TaskSyncService> logger)
//        {
//            this.scopeFactory = scopeFactory;
//            this.logger = logger;
//        }


//        // =========================================================
//        // BACKGROUND SERVICE
//        // Har 10 seconds baad:
//        // 1. Pending tasks SQL Server me sync karega
//        // 2. 1 din purane Synced tasks SQLite se delete karega
//        // =========================================================
//        protected override async Task ExecuteAsync(
//            CancellationToken stoppingToken)
//        {
//            while (!stoppingToken.IsCancellationRequested)
//            {
//                try
//                {
//                    // Pending SQLite records SQL Server me save
//                    await SyncPendingTasks();

//                    // 1 din purane synced SQLite records delete
//                    await DeleteOldSyncedTasks();
//                }
//                catch (Exception ex)
//                {
//                    logger.LogError(
//                        ex,
//                        "Task background synchronization failed."
//                    );
//                }


//                // Har 10 seconds baad service dobara chalegi
//                await Task.Delay(
//                    TimeSpan.FromSeconds(10),
//                    stoppingToken
//                );
//            }
//        }


//        // =========================================================
//        // SQLITE -> SQL SERVER SYNC
//        // Sirf Pending tasks uthata hai
//        // =========================================================
//        private async Task SyncPendingTasks()
//        {
//            using var scope =
//                scopeFactory.CreateScope();


//            var sqliteContext =
//                scope.ServiceProvider
//                    .GetRequiredService<SqliteToDoContext>();


//            var sqlContext =
//                scope.ServiceProvider
//                    .GetRequiredService<ToDoContext>();


//            // Sirf Pending records SQLite se uthao
//            var pendingTasks =
//                await sqliteContext.LocalTasks
//                    .Where(x => x.SyncStatus == "Pending")
//                    .ToListAsync();


//            // Koi Pending task nahi hai
//            if (!pendingTasks.Any())
//            {
//                return;
//            }


//            foreach (var localTask in pendingTasks)
//            {
//                try
//                {
//                    // =============================================
//                    // SQLite LocalTask ko SQL Server Task me convert
//                    // =============================================
//                    var sqlTask = new Tasks
//                    {
//                        Title = localTask.Title,

//                        Description = localTask.Description,

//                        DueDate = localTask.DueDate,

//                        Priority = localTask.Priority,

//                        Status = localTask.Status,

//                        UserId = localTask.UserId,

//                        CreatedAt = localTask.CreatedAt
//                    };


//                    // =============================================
//                    // SQL SERVER ME SAVE
//                    // =============================================
//                    await sqlContext.Tasks.AddAsync(sqlTask);

//                    await sqlContext.SaveChangesAsync();


//                    Console.WriteLine(
//                        $"[{DateTime.Now:HH:mm:ss}] " +
//                        $"SQL SERVER SAVE: " +
//                        $"Id={sqlTask.Id}, " +
//                        $"Title={sqlTask.Title}"
//                    );


//                    // =============================================
//                    // SQL Server generated ID SQLite me save
//                    // =============================================
//                    localTask.SqlServerId =
//                        sqlTask.Id;


//                    // Record successfully synced
//                    localTask.SyncStatus =
//                        "Synced";


//                    // Sync ka current time
//                    localTask.SyncedAt =
//                        DateTime.Now;


//                    sqliteContext.LocalTasks.Update(
//                        localTask
//                    );


//                    await sqliteContext.SaveChangesAsync();


//                    logger.LogInformation(
//                        "Task {LocalId} synced successfully. SQL Server Id: {SqlServerId}",
//                        localTask.LocalId,
//                        sqlTask.Id
//                    );
//                }
//                catch (Exception ex)
//                {
//                    // =============================================
//                    // Agar SQL Server save fail ho
//                    // record Pending hi rahe
//                    // next 10 sec me dobara try hoga
//                    // =============================================
//                    localTask.SyncStatus =
//                        "Pending";


//                    await sqliteContext.SaveChangesAsync();


//                    logger.LogError(
//                        ex,
//                        "Task {LocalId} could not sync.",
//                        localTask.LocalId
//                    );
//                }
//            }
//        }


//        // =========================================================
//        // OLD SQLITE DATA CLEANUP
//        //
//        // Sirf:
//        // SyncStatus = Synced
//        // AND
//        // SyncedAt 1 din se purana
//        //
//        // records SQLite se delete honge
//        // SQL Server ko touch nahi karega
//        // =========================================================
//        private async Task DeleteOldSyncedTasks()
//        {
//            using var scope =
//                scopeFactory.CreateScope();


//            var sqliteContext =
//                scope.ServiceProvider
//                    .GetRequiredService<SqliteToDoContext>();


//            // Current time se 1 din pehle
//            var oneDayAgo =
//                DateTime.Now.AddDays(-1);


//            // Sirf successfully synced aur 1 din purane records
//            var oldSyncedTasks =
//                await sqliteContext.LocalTasks
//                    .Where(x =>
//                        x.SyncStatus == "Synced"
//                        &&
//                        x.SyncedAt != null
//                        &&
//                        x.SyncedAt <= oneDayAgo
//                    )
//                    .ToListAsync();


//            // Delete karne ke liye kuch nahi
//            if (!oldSyncedTasks.Any())
//            {
//                return;
//            }


//            // Sirf SQLite se delete
//            sqliteContext.LocalTasks.RemoveRange(
//                oldSyncedTasks
//            );


//            await sqliteContext.SaveChangesAsync();


//            Console.WriteLine(
//                $"[{DateTime.Now:HH:mm:ss}] " +
//                $"{oldSyncedTasks.Count} old synced tasks " +
//                $"deleted from SQLite."
//            );


//            logger.LogInformation(
//                "{Count} synced tasks older than 1 day deleted from SQLite.",
//                oldSyncedTasks.Count
//            );
//        }
//    }
//}