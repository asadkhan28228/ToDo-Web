using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ToDo.BLL.Dto.Auth;
using ToDo.BLL.Interface;

namespace ToDo_Web.Controllers
{

    
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService authService;

        public AuthController(IAuthService authService)
        {
            this.authService = authService;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register( RegisteredDto registeredDto)
        {
            var result = await authService.RegisterAsync(registeredDto);
            return Ok(result);
        }


        [HttpPost("login")]
        public async Task<IActionResult> Login(loginDto loginDto)
        {
            var result = await authService.LoginAsync(loginDto);
            if (result == null)
            {
                return BadRequest("Invalid email or password.");
            }
            return Ok(result);
        }

    }
}
