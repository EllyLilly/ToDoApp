namespace ToDoApi.Model
{
    public class TaskItem
    {
        public int Id { get; set; }
        public string TaskName { get; set; } = string.Empty;
        public bool IsCompleted { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    }
}
