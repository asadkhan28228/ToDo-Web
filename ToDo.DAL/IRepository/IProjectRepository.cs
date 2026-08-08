using System;
using System.Collections.Generic;
using System.Text;
using ToDo.DAL.Entities;

namespace ToDo.DAL.IRepository
{
    public interface IProjectRepository
    {
        Task AddProjectAsync(Tasks project);

        Task<List<Tasks>> GetAllProjectsAsync();
        Task<Tasks> GetProjectByIdAsync(int id);

        Task UpdateAsync(Tasks project);

        Task DeleteAsync(int id);

        Task<List<Tasks>> SearchProjectsAsync(string id ,string Title);

        Task SaveChangesAsync();
    }
}
