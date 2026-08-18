using System.Collections.Generic;

namespace Backend.Models
{
    public class UserType
    {
        public int Id { get; set; }
        public string UserTypeName { get; set; } = string.Empty; // Admin, Student, Faculty
        public string? Description { get; set; }

        public ICollection<User> Users { get; set; } = new List<User>();
    }
}
