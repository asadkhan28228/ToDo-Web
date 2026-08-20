using ToDo.BLL.Dto.Project;

namespace ToDo.BLL.Interface
{
    public interface IProjectService
    {
        // ADD
        Task<string> AddProjectAsync(
            CreateProjectDto Adddto);


        // GET ALL
        Task<List<ProjectDto>> GetAllProjectsAsync();


        // GET BY ID
        Task<ProjectDto?> GetProjectByIdAsync(
            int id);


        // UPDATE
        Task UpdateAsync(
            UpdateprojectDto updatedto);


        // DELETE
        Task<bool> DeleteAsync(
            int id);


        // SEARCH
        Task<List<ProjectDto>> SearchProjectsAsync(
            SearchQuerry searchQuerry);
    }
}