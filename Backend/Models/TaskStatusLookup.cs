using System.Collections.Generic;

namespace Backend.Models
{
    public class TaskStatusLookup
    {
        public int Id { get; set; }
        public string TaskStatusName { get; set; } = string.Empty; // Pending, In Progress, Completed
        public string TaskStatusCssClass { get; set; } = string.Empty;
    
        public ICollection<TaskItem> Tasks { get; set; } = new List<TaskItem>();
    }
}
