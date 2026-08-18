namespace Backend.DTOs.UserType
{
    public class UserTypeResponseDto
    {
        public int Id { get; set; }
        public string UserTypeName { get; set; } = string.Empty;
        public string? Description { get; set; }
    }
}
