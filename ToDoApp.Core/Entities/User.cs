using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ToDoApp.Core.Entities
{
    public class User
    {
        public int Id { get; set; }
        public string UserName { get; set; }
        public string Email { get; set; }
        public string PasswordHash { get; set; } = string.Empty;
        public DateTime RegisteredAt { get; set; }


        public ICollection<TaskItem> Tasks { get; set; } = new List<TaskItem>();
    }
}
