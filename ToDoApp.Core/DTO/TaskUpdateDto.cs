namespace ToDoApp.Core.DTO
{
    public class TaskUpdateDto
    {
        public int Id { get; set; }
        public string TaskName { get; set; }
        public bool IsCompleted { get; set; }
    }
}
