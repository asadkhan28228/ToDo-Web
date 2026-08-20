using System.Globalization;
using ToDo.BLL.Dto.Project;
using ToDo.BLL.Interface;
using ToDo.DAL.Entities;
using ToDo.DAL.IRepository;

namespace ToDo.BLL.Services
{
    public class ProjectService : IProjectService
    {
        private readonly IProjectRepository projectRepository;


        public ProjectService(
            IProjectRepository projectRepository)
        {
            this.projectRepository = projectRepository;
        }


        // =====================================================
        // ADD PROJECT
        // =====================================================
        public async Task<string> AddProjectAsync(
            CreateProjectDto Adddto)
        {
            // Due Date required
            if (string.IsNullOrWhiteSpace(Adddto.DueDate))
            {
                return "Due Date is required.";
            }


            // dd-MM-yyyy format convert
            bool isValidDate =
                DateTime.TryParseExact(
                    Adddto.DueDate,
                    "dd-MM-yyyy",
                    CultureInfo.InvariantCulture,
                    DateTimeStyles.None,
                    out DateTime dueDate
                );


            if (!isValidDate)
            {
                return
                    "Invalid Due Date. Please use dd-MM-yyyy format.";
            }


            // Task Entity
            var task = new Tasks
            {
                Title = Adddto.Title,

                Description = Adddto.Description,

                DueDate = dueDate,

                Priority = Adddto.Priority,

                Status = Adddto.Status,

                UserId = Adddto.UserId,

                // Automatically current date/time
                CreatedAt = DateTime.Now
            };


            // Repository pehle SQLite me save karega
            await projectRepository
                .AddProjectAsync(task);


            return "Project Added Successfully";
        }


        // =====================================================
        // GET ALL PROJECTS
        //
        // SQL Server +
        // SQLite Pending Tasks
        //
        // Dono combine honge.
        // =====================================================
        public async Task<List<ProjectDto>> GetAllProjectsAsync()
        {
            // -----------------------------------------
            // SQL SERVER TASKS
            // -----------------------------------------
            var sqlProjects =
                await projectRepository
                    .GetAllProjectsAsync();


            // -----------------------------------------
            // SQLITE PENDING TASKS
            // -----------------------------------------
            var pendingLocalTasks =
                await projectRepository
                    .GetPendingLocalTasksAsync();


            var projectDtos =
                new List<ProjectDto>();


            // =========================================
            // SQL SERVER TASKS DTO ME CONVERT
            // =========================================
            foreach (var project in sqlProjects)
            {
                var projectDto =
                    new ProjectDto
                    {
                        Id = project.Id,

                        Title = project.Title,

                        Description =
                            project.Description,

                        DueDate =
                            project.DueDate,

                        Priority =
                            project.Priority,

                        Status =
                            project.Status,

                        UserId =
                            project.UserId,

                        CreatedAt =
                            project.CreatedAt
                    };


                projectDtos.Add(projectDto);
            }


            // =========================================
            // SQLITE PENDING TASKS DTO ME CONVERT
            // =========================================
            foreach (var localTask in pendingLocalTasks)
            {
                var projectDto =
                    new ProjectDto
                    {
                        // IMPORTANT:
                        // Pending SQLite task ka
                        // SQL Server ID abhi nahi hai.
                        //
                        // Isliye temporary negative ID.
                        Id = -localTask.LocalId,

                        Title =
                            localTask.Title,

                        Description =
                            localTask.Description,

                        DueDate =
                            localTask.DueDate,

                        Priority =
                            localTask.Priority,

                        Status =
                            localTask.Status,

                        UserId =
                            localTask.UserId,

                        CreatedAt =
                            localTask.CreatedAt
                    };


                projectDtos.Add(projectDto);
            }


            // Newest task sabse upar
            return projectDtos
                .OrderByDescending(
                    x => x.CreatedAt)
                .ToList();
        }


        // =====================================================
        // GET PROJECT BY ID
        // =====================================================
        public async Task<ProjectDto?> GetProjectByIdAsync(
            int id)
        {
            // Negative ID temporary SQLite task hai.
            // Isko Edit/Delete nahi karenge jab tak sync na ho.
            if (id <= 0)
            {
                return null;
            }


            var project =
                await projectRepository
                    .GetProjectByIdAsync(id);


            if (project == null)
            {
                return null;
            }


            var projectDto =
                new ProjectDto
                {
                    Id =
                        project.Id,

                    Title =
                        project.Title,

                    Description =
                        project.Description,

                    DueDate =
                        project.DueDate,

                    Priority =
                        project.Priority,

                    Status =
                        project.Status,

                    UserId =
                        project.UserId,

                    CreatedAt =
                        project.CreatedAt
                };


            return projectDto;
        }


        // =====================================================
        // UPDATE PROJECT
        // =====================================================
        public async Task UpdateAsync(
            UpdateprojectDto updatedto)
        {
            // Invalid model / ID
            if (updatedto == null ||
                updatedto.ID <= 0)
            {
                return;
            }


            // Existing SQL task
            var project =
                await projectRepository
                    .GetProjectByIdAsync(
                        updatedto.ID);


            if (project == null)
            {
                return;
            }


            // Title required
            if (string.IsNullOrWhiteSpace(
                updatedto.Title))
            {
                return;
            }


            // Update fields
            project.Title =
                updatedto.Title;

            project.Description =
                updatedto.Description;

            project.Priority =
                updatedto.Priority;

            project.Status =
                updatedto.Status;

            project.UserId =
                updatedto.UserId;


            // IMPORTANT:
            // Aapke existing code ki tarah
            // DueDate ko yahan change nahi kar raha.
            //
            // Agar UpdateprojectDto me DueDate
            // correct DateTime type hai to separately
            // update kar sakte hain.


            await projectRepository
                .UpdateAsync(project);
        }


        // =====================================================
        // DELETE PROJECT
        // =====================================================
        public async Task<bool> DeleteAsync(
            int id)
        {
            // Negative/zero ID delete nahi hogi.
            // Negative ID SQLite pending task hai.
            if (id <= 0)
            {
                return false;
            }


            var project =
                await projectRepository
                    .GetProjectByIdAsync(id);


            if (project == null)
            {
                return false;
            }


            await projectRepository
                .DeleteAsync(id);


            return true;
        }


        // =====================================================
        // SEARCH PROJECTS
        // =====================================================
        public async Task<List<ProjectDto>>
            SearchProjectsAsync(
                SearchQuerry searchQuerry)
        {
            var projects =
                await projectRepository
                    .SearchProjectsAsync(
                        searchQuerry.ID,
                        searchQuerry.Title
                    );


            var projectDtos =
                new List<ProjectDto>();


            foreach (var project in projects)
            {
                var projectDto =
                    new ProjectDto
                    {
                        Id =
                            project.Id,

                        Title =
                            project.Title,

                        Description =
                            project.Description,

                        DueDate =
                            project.DueDate,

                        Priority =
                            project.Priority,

                        Status =
                            project.Status,

                        UserId =
                            project.UserId,

                        CreatedAt =
                            project.CreatedAt
                    };


                projectDtos.Add(projectDto);
            }


            return projectDtos;
        }
    }
}