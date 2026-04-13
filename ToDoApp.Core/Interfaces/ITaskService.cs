using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ToDoApp.Core.Entities;
using ToDoApp.Core.Interfaces;
using ToDoApp.Core.DTO;


namespace ToDoApp.Core.Interfaces
{
    public interface ITaskService
    {
        Task<List<TaskResponseDto>> GetUserTasksAsync(int userId);

        Task<TaskResponseDto?> GetTaskByIdAsync(int taskId, int userId);

        Task<TaskResponseDto> CreateTaskAsync(int userId, TaskCreateDto dto);

        Task<TaskResponseDto?> UpdateTaskAsync(int taskId, int userId, TaskUpdateDto dto);

        Task<bool> DeleteTaskAsync(int taskId, int userId);
    }
}
