using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ToDo.BLL.Dto.Project;
using ToDo.BLL.Interface;
using ToDo.DAL.Entities;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace ToDo_Web.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
  
    public class TaskController : ControllerBase
    {
        private readonly IProjectService projectService;

        public TaskController(IProjectService projectService)
        {
            this.projectService = projectService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllProjects()
        {
            var projects = await projectService.GetAllProjectsAsync();
            return Ok(projects);
        }
        [HttpGet("{id}")]
        public async Task<IActionResult> GetProjectById(int id)
        {
            var project = await projectService.GetProjectByIdAsync(id);
            if (project == null)
            {
                return NotFound();
            }
            return Ok(project);
        }
        [HttpPost]
        public async Task<IActionResult> AddProject(CreateProjectDto project)
        {
            await projectService.AddProjectAsync(project);
            return Ok(new
            {
                Message = "Project added successfully",
                
            });
        }

        [HttpPut]
        public async Task<IActionResult> UpdateProject(UpdateprojectDto project)
        {
            await projectService.UpdateAsync(project);
            return Ok(new
            {
                Message = "Project Updated successfully",
                Data = project
            });
        }
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteProject(int id)
        {
            await projectService.DeleteAsync(id);
            return Ok(new
            {
                Message = "Project Updated successfully",
            });
        }

        [HttpGet("search")]
        public async Task<IActionResult> SearchProjects([FromQuery]SearchQuerry searchQuerry)
        {
            var projects = await projectService.SearchProjectsAsync(searchQuerry);
            return Ok(projects);
        }

    }
}
