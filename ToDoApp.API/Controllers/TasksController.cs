using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using ToDoApp.Core.Entities;
using ToDoApp.Infrastructure.Data;

namespace ToDoApp.API.Controllers
{
    [ApiController]
    [Route("api/tasks")]
    public class TasksController : ControllerBase
    {

        private readonly ToDoDbContext _db;

        public TasksController(ToDoDbContext db) { _db = db; }

        //GET: api/tasks
        [HttpGet]
        public async Task<IActionResult> GetTasks()
        {
            var tasks = await _db.TaskItems.ToListAsync();
            return Ok(tasks);
        }

        //GET: api/tasks/{id}
        [HttpGet("{id}")]
        public async Task<IActionResult> GetOneTask(int id)
        {
            var oneTask = await _db.TaskItems.FindAsync(id);

            if (oneTask == null)
            {
                return NotFound();
            }

            return Ok(oneTask);
        }

        //POST: /api/tasks
        [HttpPost]
        public async Task<ActionResult<TaskItem>> CreateTask([FromBody] TaskItem task)
        {
            task.CreatedAt = DateTime.UtcNow;
            _db.TaskItems.Add(task);
            await _db.SaveChangesAsync();


            return CreatedAtAction(nameof(GetOneTask), new { id = task.Id }, task);
        }

        //PUT: /api/tasks/5
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateTask(int id, [FromBody] TaskItem updatedTask)
        {
            if (id != updatedTask.Id)
            {
                return BadRequest();
            }

            var existingTask = await _db.TaskItems.FirstOrDefaultAsync(t => t.Id == id);

            if (existingTask == null)
            {
                return NotFound();
            }

            existingTask.TaskName = updatedTask.TaskName;
            existingTask.IsCompleted = updatedTask.IsCompleted;
            await _db.SaveChangesAsync();

            return Ok();

        }

        //DELETE: /api/tasks/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteTask(int id)
        {
            var task = await _db.TaskItems.FindAsync(id);
            if (task == null)
            {
                return NotFound();
            }

            _db.TaskItems.Remove(task);

            await _db.SaveChangesAsync();
            return NoContent();
        }
    }
}
