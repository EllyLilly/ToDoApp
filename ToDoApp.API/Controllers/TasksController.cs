using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using System.Threading.Tasks;
using ToDoApp.Core.Entities;
using ToDoApp.Core.Interfaces;
using ToDoApp.Infrastructure.Data;
using ToDoApp.Core.DTO;
using ToDoApp.Infrastructure.Services;
using FluentValidation;
using FluentValidation.Results;

namespace ToDoApp.API.Controllers
{
    [ApiController]
    [Route("api/tasks")]
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
        public async Task<IActionResult> GetTasks([FromQuery]int userId)
        {
            var tasks = await _taskService.GetUserTasksAsync(userId);
            return Ok(tasks);
        }

        //GET: api/tasks/{id}
        [HttpGet("{id}")]
        public async Task<IActionResult> GetOneTask(int id, [FromQuery] int userId)
        {
            var oneTask = await _taskService.GetTaskByIdAsync(id, userId);

            if (oneTask == null)
            {
                return NotFound();
            }

            return Ok(oneTask);
        }

        //POST: /api/tasks
        [HttpPost]
        public async Task<ActionResult<TaskResponseDto>> CreateTask([FromQuery] int userId, [FromBody] TaskCreateDto dto)
        {
            
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
        public async Task<IActionResult> UpdateTask(int id, [FromQuery] int userId, [FromBody] TaskUpdateDto updatedTask)
        {

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
        public async Task<IActionResult> DeleteTask(int id, [FromQuery] int userId)
        {
            var deleted = await _taskService.DeleteTaskAsync(id, userId);

            if (!deleted)
            {
                return NotFound();
            }

            return NoContent();
        }
    }
}
