using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ToDo.BLL.Dto.Project;
using ToDo.BLL.Interface;

namespace ToDo_Web.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class TaskController : ControllerBase
    {
        private readonly IProjectService projectService;

        public TaskController(IProjectService projectService)
        {
            this.projectService = projectService;
        }


        // ============================
        // GET ALL
        // ============================
        [HttpGet]
        public async Task<IActionResult> GetAllProjects()
        {
            var projects =await projectService.GetAllProjectsAsync();

            return Ok(projects);
        }


        // ============================
        // GET BY ID
        // ============================
        [HttpGet("{id}")]
        public async Task<IActionResult> GetProjectById(int id)
        {
            if (id <= 0)
            {
                return BadRequest(new
                {
                    Message = "Invalid project id."
                });
            }

            var project =await projectService.GetProjectByIdAsync(id);

            if (project == null)
            {
                return NotFound(new
                {
                    Message = "Project not found."
                });
            }

            return Ok(project);
        }


        // ============================
        // ADD
        // ============================
        [HttpPost]
        public async Task<IActionResult> AddProject(CreateProjectDto project)
        {
            if (project == null)
            {
                return BadRequest(new
                {
                    Message = "Project data is required."
                });
            }

            if (string.IsNullOrWhiteSpace(project.Title))
            {
                return BadRequest(new
                {
                    Message = "Project title is required."
                });
            }

            await projectService.AddProjectAsync(project);

            return Ok(new
            {
                Message = "Project added successfully."
            });
        }


        // ============================
        // UPDATE
        // ============================
        [HttpPut]
        public async Task<IActionResult> UpdateProject(
            UpdateprojectDto project)
        {
            if (project == null)
            {
                return BadRequest(new
                {
                    Message = "Project data is required."
                });
            }

            if (project.ID <= 0)
            {
                return BadRequest(new
                {
                    Message = "Invalid project id."
                });
            }

            if (string.IsNullOrWhiteSpace(project.Title))
            {
                return BadRequest(new
                {
                    Message = "Project title is required."
                });
            }

            // Pehle check karo project exist karta hai
            var existingProject =
                await projectService.GetProjectByIdAsync(
                    project.ID);

            if (existingProject == null)
            {
                return NotFound(new
                {
                    Message = "Project not found."
                });
            }

            await projectService.UpdateAsync(project);

            return Ok(new
            {
                Message = "Project updated successfully."
            });
        }


        // ============================
        // DELETE
        // ============================
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteProject(int id)
        {
            if (id <= 0)
            {
                return BadRequest(new
                {
                    Message = "Invalid project id."
                });
            }

            // Pehle check karo project exist karta hai
            var existingProject =
                await projectService.GetProjectByIdAsync(id);

            if (existingProject == null)
            {
                return NotFound(new
                {
                    Message = "Project not found."
                });
            }

            await projectService.DeleteAsync(id);

            return Ok(new
            {
                Message = "Project deleted successfully."
            });
        }


        // ============================
        // SEARCH
        // ============================
        [HttpGet("search")]
        public async Task<IActionResult> SearchProjects(
            [FromQuery] SearchQuerry searchQuerry)
        {
            var projects =
                await projectService.SearchProjectsAsync(
                    searchQuerry);

            return Ok(projects);
        }
    }
}