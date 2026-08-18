namespace Backend.DTOs.TaskPriority
{
    public class TaskPriorityResponseDto
    {
        public int Id { get; set; }
        public string TaskPriorityName { get; set; } = string.Empty;
        public string TaskPriorityCssClass { get; set; } = string.Empty;
    }
    public class CreateTaskPriorityDto
    {
        public string TaskPriorityName { get; set; } = string.Empty;
        public string TaskPriorityCssClass { get; set; } = string.Empty;
    }

    public class UpdateTaskPriorityLookupDto
    {
        public string TaskPriorityName { get; set; } = string.Empty;
        public string TaskPriorityCssClass { get; set; } = string.Empty;
    }
}