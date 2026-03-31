using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ToDoApp.Core.Entities
{
    public class TaskItem
    {
        public int Id { get; set; }
        public string TaskName { get; set; } = string.Empty;
        public bool IsCompleted { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
