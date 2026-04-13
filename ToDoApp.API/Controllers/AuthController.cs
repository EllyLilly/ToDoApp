
using Microsoft.AspNetCore.Mvc;
//using System.ComponentModel.DataAnnotations;
using ToDoApp.Core.Entities;
using ToDoApp.API.DTO;
using ToDoApp.Core.Interfaces;
using ToDoApp.Infrastructure.Services;
using FluentValidation;
using FluentValidation.Results;

namespace ToDoApp.API.Controllers
{

    [ApiController]
    [Route("api/auth")]
    public class AuthController : ControllerBase
    {
        private IValidator<RegisterRequest> _registerValidator;
        private IValidator<LoginRequest> _loginValidator;
        
        private readonly IAuthService _authService;
        public AuthController(IValidator<RegisterRequest> registerValidator, IValidator<LoginRequest> loginValidator, IAuthService authService)
        {
            _registerValidator = registerValidator;
            _loginValidator = loginValidator;
            _authService = authService;
        }

        //POST /api/auth/register
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterRequest request)
        {
            
            ValidationResult validationResult = await _registerValidator.ValidateAsync(request);

            if(!validationResult.IsValid)
            {
                return BadRequest(validationResult.Errors);
            }
            
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
            ValidationResult loginResult = await _loginValidator.ValidateAsync(request);
            
            if (!loginResult.IsValid)
            {
                return BadRequest($"{loginResult.Errors.ToString()}");
            }
            
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
