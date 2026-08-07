using System;
using System.Collections.Generic;
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

        //  ADD Function //
        public async Task<string> AddProjectAsync(CreateProjectDto Adddto)
        {
            var project = new Project
            {
                Title = Adddto.Title,
                Description = Adddto.Description,
                DueDate = Adddto.DueDate,
                Priority = Adddto.Priority,
                Status = Adddto.Status,
                UserId = Adddto.UserId,
                
            };
            await projectRepository.AddProjectAsync(project);

            await projectRepository.SaveChangesAsync();

            return"Project will Added Successfully Added";

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
            var project = await projectRepository.GetProjectByIdAsync(updatedto.ID);
            if (project != null)
            {
                project.Title = updatedto.Title;
                project.Description = updatedto.Description;
                project.DueDate = updatedto.DueDate;
                project.Priority = updatedto.Priority;
                project.Status = updatedto.Status;
                project.UserId = updatedto.UserId;
                await projectRepository.UpdateAsync(project);
            }
        }
        public async Task DeleteAsync(int id)
        {
            var project = await projectRepository.GetProjectByIdAsync(id); 

            if(project != null)
            {
                await projectRepository.DeleteAsync(id);
            }
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
