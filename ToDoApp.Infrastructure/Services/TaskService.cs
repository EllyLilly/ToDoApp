using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ToDoApp.Core.DTO;
using ToDoApp.Core.Entities;
using ToDoApp.Core.Interfaces;

namespace ToDoApp.Infrastructure.Services
{
    public class TaskService : ITaskService
    {

        private readonly ITaskRepository _taskRepository;
        private readonly ILogger<TaskService> _logger;

        public TaskService(ILogger<TaskService> logger, ITaskRepository taskRepository)
        {
            _logger = logger;
            _taskRepository = taskRepository;
        }

        public async Task<List<TaskResponseDto>> GetUserTasksAsync(int userId)
        {
            var userIdTasks = await _taskRepository.GetUserTasksAsync(userId);


            var result = new List<TaskResponseDto>();

            foreach (var userIdTask in userIdTasks)
            {
                var dto = new TaskResponseDto();

                dto.Id = userIdTask.Id;
                dto.TaskName = userIdTask.TaskName;
                dto.IsCompleted = userIdTask.IsCompleted;
                dto.CreatedAt = userIdTask.CreatedAt;

                result.Add(dto);

            }

            return result;

        }

        public async Task<TaskResponseDto?> GetTaskByIdAsync(int taskId, int userId)
        {
            var oneTask = await _taskRepository.GetTaskByIdAndUserAsync(taskId, userId);

            if (oneTask == null) {return null;}

            var dto = new TaskResponseDto();

            dto.Id = oneTask.Id;
            dto.TaskName = oneTask.TaskName;
            dto.IsCompleted = oneTask.IsCompleted;
            dto.CreatedAt = oneTask.CreatedAt;

            return dto;
        }

        public async Task<TaskResponseDto> CreateTaskAsync(int userId, TaskCreateDto dto)
        {
            var newTask = new TaskItem();

            newTask.TaskName = dto.TaskName;
            newTask.IsCompleted = dto.IsCompleted;
            newTask.UserId = userId;
            newTask.CreatedAt = DateTime.UtcNow;

            var result = _taskRepository.AddAsync(newTask);

            await _taskRepository.SaveChangesAsync();

            _logger.LogInformation("User {UserId} created task {TaskId}: {TaskName}",
    userId, newTask.Id, newTask.TaskName);

            var dtoResult = new TaskResponseDto();

            dtoResult.Id = newTask.Id;
            dtoResult.TaskName = newTask.TaskName;
            dtoResult.IsCompleted = newTask.IsCompleted;
            dtoResult.CreatedAt = newTask.CreatedAt;


            return dtoResult;

        }

        public async Task<bool> DeleteTaskAsync(int taskId, int userId)
        {
            var oneTask = await _taskRepository.GetTaskByIdAndUserAsync(taskId, userId);

            if (oneTask == null) { return false; }

            _taskRepository.Delete(oneTask);

            _logger.LogInformation("User {UserId} deleted task {TaskId}", userId, taskId);

            await _taskRepository.SaveChangesAsync();

            return true;
        }

        public async Task<TaskResponseDto?> UpdateTaskAsync(int taskId, int userId, TaskUpdateDto dto)
        {
            var existingTask = await _taskRepository.GetTaskByIdAndUserAsync(taskId, userId);

            if (existingTask == null)
            {
                return null;
            } else
            {
                existingTask.TaskName = dto.TaskName;
                existingTask.IsCompleted = dto.IsCompleted;
            }

            await _taskRepository.SaveChangesAsync();

            var updatedTask = new TaskResponseDto();

            updatedTask.Id = existingTask.Id;
            updatedTask.TaskName = existingTask.TaskName;
            updatedTask.IsCompleted = existingTask.IsCompleted;
            updatedTask.CreatedAt = existingTask.CreatedAt;

            return updatedTask;

        }

    }
}
