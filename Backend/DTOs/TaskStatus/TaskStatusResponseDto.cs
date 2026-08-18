namespace Backend.DTOs.TaskStatus
{
    public class TaskStatusResponseDto
    {
        public int Id { get; set; }
        public string TaskStatusName { get; set; } = string.Empty;
        public string TaskStatusCssClass { get; set; } = string.Empty;
    }

    public class CreateTaskStatusDto
    {
        public string TaskStatusName { get; set; } = string.Empty;

        public string TaskStatusCssClass { get; set; } = string.Empty; 
    }

    public class UpdateTaskStatusLookupDto
    {
        public string TaskStatusName { get; set; } = string.Empty;
        public string TaskStatusCssClass { get; set; } = string.Empty;
    }
}
