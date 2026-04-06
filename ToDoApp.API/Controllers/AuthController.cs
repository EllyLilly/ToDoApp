
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using ToDoApp.Core.Entities;
using ToDoApp.API.DTO;
using ToDoApp.Core.Interfaces;
using ToDoApp.Infrastructure.Services;

namespace ToDoApp.API.Controllers
{

    [ApiController]
    [Route("api/auth")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;
        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        //POST /api/auth/register
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterRequest request)
        {
            
            var result = await _authService.RegisterAsync(request.UserName, request.Email, request.Password);

            if (result)
            {
                return Ok();
            }
            else
            {
                return BadRequest(error: new { error = "Unable to return user" });
            }
        }

        //POST /api/auth/login
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest request)
        {
            var token = await _authService.LoginAsync(request.UserName, request.Password);

            if(token != null)
            {
                return Ok(token);
            }
            else
            {
                return Unauthorized();            
            }
        }
    }
}
