using System.Collections.Generic;

namespace Backend.Models
{
    public class TaskPriorityLookup
    {
        public int Id { get; set; }
        public string TaskPriorityName { get; set; } = string.Empty; // Low, Medium, High, Critical
        public string TaskPriorityCssClass { get; set; } = string.Empty;

        public ICollection<TaskItem> Tasks { get; set; } = new List<TaskItem>();
    }
}
