using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ToDo.BLL.Dto.Project;
using ToDo.BLL.Interface;
using ToDo.DAL.Entities;

namespace ToDo_Web.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProjectController : ControllerBase
    {
        private readonly IProjectService projectService;

        public ProjectController(IProjectService projectService)
        {
            this.projectService = projectService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllProjects()
        {
            var projects = await projectService.GetAllProjectsAsync();
            return Ok(projects);
        }
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
                Data = project
            });


        }
        [HttpPut]
        public async Task<IActionResult> UpdateProject(UpdateprojectDto project)
        {
            await projectService.UpdateAsync(project);
            return NoContent();
        }
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteProject(int id)
        {
            await projectService.DeleteAsync(id);
            return NoContent();
        }
        
    }
}
