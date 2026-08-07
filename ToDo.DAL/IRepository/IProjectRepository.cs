using System;
using System.Collections.Generic;
using System.Text;
using ToDo.DAL.Entities;

namespace ToDo.DAL.IRepository
{
    public interface IProjectRepository
    {
        Task AddProjectAsync(Project project);

        Task<List<Project>> GetAllProjectsAsync();
        Task<Project> GetProjectByIdAsync(int id);

        Task UpdateAsync(Project project);

        Task DeleteAsync(int id);

        Task<List<Project>> SearchProjectsAsync(string id ,string Title);

        Task SaveChangesAsync();
    }
}
