namespace Backend.DTOs.Department
{
    public class CreateDepartmentDto
    {
        public string Name { get; set; } = string.Empty;
    }

    public class UpdateDepartmentDto
    {
        public string Name { get; set; } = string.Empty;
    }

    public class DepartmentResponseDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public int UserCount { get; set; }
    }
}
