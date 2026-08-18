using System;
using System.Collections.Generic;
using System.Text;
using ToDo.BLL.Dto.Project;

namespace ToDo.BLL.Interface
{
    public interface IProjectService
    {
        Task<string> AddProjectAsync(CreateProjectDto Adddto);
        Task<List<ProjectDto>> GetAllProjectsAsync();
        Task<ProjectDto> GetProjectByIdAsync(int id);
        Task UpdateAsync(UpdateprojectDto updatedto);

        Task<bool> DeleteAsync(int id);
        Task<List<ProjectDto>> SearchProjectsAsync(SearchQuerry searchQuerry);
    }
}
