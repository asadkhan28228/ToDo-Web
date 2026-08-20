using System.Collections.Generic;
using System.Threading.Tasks;
using ToDo.DAL.Entities;
using ToDo.DAL.SQLiteEntities;

namespace ToDo.DAL.IRepository
{
    public interface IProjectRepository
    {
        // ADD
        Task AddProjectAsync(Tasks project);

        // GET ALL FROM SQL SERVER
        Task<List<Tasks>> GetAllProjectsAsync();

        // GET PENDING TASKS FROM SQLITE
        Task<List<LocalTask>> GetPendingLocalTasksAsync();

        // GET BY ID
        Task<Tasks?> GetProjectByIdAsync(int id);

        // UPDATE
        Task UpdateAsync(Tasks project);

        // DELETE
        Task DeleteAsync(int id);

        // SEARCH
        Task<List<Tasks>> SearchProjectsAsync(
            string? id,
            string? title);

        // SAVE
        Task SaveChangesAsync();
    }
}