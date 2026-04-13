using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ToDoApp.Core.Interfaces;
using ToDoApp.Infrastructure.Data;
using ToDoApp.Core.Entities;


namespace ToDoApp.Infrastructure.Repositories
{
    public class TaskRepository : ITaskRepository
    {

        private readonly ToDoDbContext _db;

        public TaskRepository(ToDoDbContext db) { _db = db; }

        public async Task<List<TaskItem>> GetUserTasksAsync(int userId)
        {
            var userIdTasks = _db.TaskItems.Where(u => u.UserId == userId);


            List<TaskItem> tasks = await userIdTasks.ToListAsync();
            return tasks;
        }

        public async Task<TaskItem?> GetTaskByIdAndUserAsync(int taskId, int userId)
        {
            var oneTask = await _db.TaskItems.FirstOrDefaultAsync(t => t.Id == taskId && t.UserId == userId);

            return (oneTask);
        }

        public Task AddAsync(TaskItem task)
        {
            _db.TaskItems.Add(task);
            return Task.CompletedTask;
        }

        public Task Update(TaskItem task)
        {
            _db.TaskItems.Update(task);
            return Task.CompletedTask;
        }

        public Task Delete(TaskItem task)
        {
            _db.TaskItems.Remove(task);
            return Task.CompletedTask;
        }

        public async Task SaveChangesAsync()
        {
            await _db.SaveChangesAsync();
            
        }
    }
}
