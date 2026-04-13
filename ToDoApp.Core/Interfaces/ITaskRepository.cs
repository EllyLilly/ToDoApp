using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ToDoApp.Core.Entities;

namespace ToDoApp.Core.Interfaces
{
    public interface ITaskRepository
    {
        Task<List<TaskItem>> GetUserTasksAsync(int userId);
        Task<TaskItem?> GetTaskByIdAndUserAsync(int taskId, int userId);
        Task AddAsync(TaskItem task);

        Task Update(TaskItem task);
        Task Delete(TaskItem task);
        Task SaveChangesAsync();
    }
}
