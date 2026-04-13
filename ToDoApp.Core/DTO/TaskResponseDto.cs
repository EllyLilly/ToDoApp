namespace ToDoApp.Core.DTO
{
    public class TaskResponseDto
    {
        public int Id { get; set; }
        public string TaskName { get; set; }

        public bool IsCompleted { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
