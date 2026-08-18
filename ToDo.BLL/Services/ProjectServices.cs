using System;
using System.Collections.Generic;
using System.Globalization;
using System.Runtime.CompilerServices;
using System.Text;
using ToDo.BLL.Dto.Project;
using ToDo.BLL.Interface;
using ToDo.DAL.Entities;
using ToDo.DAL.IRepository;

namespace ToDo.BLL.Services
{
    public class ProjectService : IProjectService
    {
        private readonly IProjectRepository projectRepository;

        public ProjectService(IProjectRepository projectRepository)
        {
            this.projectRepository = projectRepository;
        }

        // ADD Function
        public async Task<string> AddProjectAsync(CreateProjectDto Adddto)
        {
            if (string.IsNullOrWhiteSpace(Adddto.DueDate))
            {
                return "Due Date is required.";
            }

            bool isValidDate = DateTime.TryParseExact(
                Adddto.DueDate,
                "dd-MM-yyyy",
                CultureInfo.InvariantCulture,
                DateTimeStyles.None,
                out DateTime dueDate);

            if (!isValidDate)
            {
                return "Invalid Due Date. Please use dd-MM-yyyy format.";
            }

            var task = new Tasks
            {
                Title = Adddto.Title,
                Description = Adddto.Description,

                // Automatically 00:00:00 time hoga
                DueDate = dueDate,

                Priority = Adddto.Priority,
                Status = Adddto.Status,
                UserId = Adddto.UserId,
                CreatedAt = DateTime.Now
            };

            await projectRepository.AddProjectAsync(task);

            return "Project Added Successfully";
        }
        public async Task<List<ProjectDto>> GetAllProjectsAsync()
        {
            var projects = await projectRepository.GetAllProjectsAsync();
            
            var projectDtos = new List<ProjectDto>();
            foreach (var project in projects)
            {
                var projectDto = new ProjectDto
                {
                    Id = project.Id,
                    Title = project.Title,
                    Description = project.Description,
                    DueDate = project.DueDate,
                    Priority = project.Priority,
                    Status = project.Status,
                    UserId = project.UserId,
                    CreatedAt = project.CreatedAt
                };
                projectDtos.Add(projectDto);
            }
            return projectDtos;
        }
        public async Task<ProjectDto> GetProjectByIdAsync(int id)
        {
            var project = await projectRepository.GetProjectByIdAsync(id);
            if (project == null)
            {
                return null;
            }
            var projectDto = new ProjectDto
            {
                Id = project.Id,
                Title = project.Title,
                Description = project.Description,
                DueDate = project.DueDate,
                Priority = project.Priority,
                Status = project.Status,
                UserId = project.UserId
            };

            return projectDto;
        }

        //  UPDATE Function //
        public async Task UpdateAsync(UpdateprojectDto updatedto)
        {
            // ID valid honi chahiye
            if (updatedto == null || updatedto.ID <= 0)
            {
                return;
            }

            // Database se existing project find karo
            var project = await projectRepository.GetProjectByIdAsync(updatedto.ID);

            // Project nahi mila to update mat karo
            if (project == null)
            {
                return;
            }

            // Required fields check
            if (string.IsNullOrWhiteSpace(updatedto.Title))
            {
                return;
            }

            // Existing project update
            project.Title = updatedto.Title;
            project.Description = updatedto.Description;
            project.Priority = updatedto.Priority;
            project.Status = updatedto.Status;
            project.UserId = updatedto.UserId;

            // Repository ko update ke liye bhejo
            await projectRepository.UpdateAsync(project);
        }
        public async Task<bool> DeleteAsync(int id)
        {
            if (id <= 0)
            {
                return false;
            }

            var project = await projectRepository.GetProjectByIdAsync(id);

            if (project == null)
            {
                return false;
            }

            await projectRepository.DeleteAsync(id);

            return true;
        }

        public async Task<List<ProjectDto>> SearchProjectsAsync(SearchQuerry searchQuerry)
        {
            var projects = await projectRepository.SearchProjectsAsync(searchQuerry.ID, searchQuerry.Title);
            var projectDtos = new List<ProjectDto>();
            foreach (var project in projects)
            {
                var projectDto = new ProjectDto
                {
                    Id = project.Id,
                    Title = project.Title,
                    Description = project.Description,
                    DueDate = project.DueDate,
                    Priority = project.Priority,
                    Status = project.Status,
                    UserId = project.UserId
                };
                projectDtos.Add(projectDto);
            }
            return projectDtos;
        }
    }
}
