using Asp.Versioning;
using FluentValidation;
using FluentValidation.Results;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using System.Threading.Tasks;
using ToDoApp.Core.DTO;
using ToDoApp.Core.Entities;
using ToDoApp.Core.Interfaces;
using ToDoApp.Infrastructure.Data;
using ToDoApp.Infrastructure.Services;

namespace ToDoApp.API.Controllers
{
    [Authorize]
    [ApiController]
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/tasks")]
    public class TasksController : ControllerBase
    {
        private IValidator<TaskCreateDto> _createValidator;
        private IValidator<TaskUpdateDto> _updateValidator;

        private readonly ITaskService _taskService;

        public TasksController(IValidator<TaskCreateDto> createValidator, IValidator<TaskUpdateDto> updateValidator, ITaskService taskService) 
        {

            _createValidator = createValidator;
            _updateValidator = updateValidator;
            _taskService = taskService;
        }

        //GET: api/tasks
        [HttpGet]
        public async Task<IActionResult> GetTasks()
        {
            //return Ok(User.Claims.Select(c => new { c.Type, c.Value }).ToList());

            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (userIdClaim == null) return Unauthorized();

            int userId = int.Parse(userIdClaim);

            var tasks = await _taskService.GetUserTasksAsync(userId);
            return Ok(tasks);
        }

        //GET: api/tasks/{id}
        [HttpGet("{id}")]
        public async Task<IActionResult> GetOneTask(int id)
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            
            if (userIdClaim == null) return Unauthorized();
            
            int userId = int.Parse(userIdClaim);

            var oneTask = await _taskService.GetTaskByIdAsync(id, userId);

            if (oneTask == null)
            {
                return NotFound();
            }

            return Ok(oneTask);
        }

        //POST: /api/tasks
        [HttpPost]
        public async Task<ActionResult<TaskResponseDto>> CreateTask([FromBody] TaskCreateDto dto)
        {

            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            
            if (userIdClaim == null) return Unauthorized();
            
            int userId = int.Parse(userIdClaim);


            ValidationResult result = await _createValidator.ValidateAsync(dto);

            if(!result.IsValid)
            {
                return BadRequest(result.Errors);
            }
            
            var createdTask = await _taskService.CreateTaskAsync(userId, dto);
            
            return CreatedAtAction(nameof(GetOneTask), new { id = createdTask.Id }, createdTask);
        }

        //PUT: /api/tasks/5
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateTask(int id, [FromBody] TaskUpdateDto updatedTask)
        {

            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (userIdClaim == null) return Unauthorized();

            int userId = int.Parse(userIdClaim);

            ValidationResult validationResult = await _updateValidator.ValidateAsync(updatedTask);
            if(!validationResult.IsValid)
            { 
                return BadRequest(validationResult.Errors); 
            }
            
            if (id != updatedTask.Id)
            {
                return BadRequest();
            }

            var result = await _taskService.UpdateTaskAsync(id, userId, updatedTask);

            if (result == null)
            {
                return NotFound();
            }

            return Ok();

        }

        //DELETE: /api/tasks/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteTask(int id)
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (userIdClaim == null) return Unauthorized();

            int userId = int.Parse(userIdClaim);

            var deleted = await _taskService.DeleteTaskAsync(id, userId);

            if (!deleted)
            {
                return NotFound();
            }

            return NoContent();
        }
    }
}
