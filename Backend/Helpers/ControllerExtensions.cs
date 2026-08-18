using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;

namespace Backend.Helpers
{
    public static class ControllerExtensions
    {
        public static int? GetCurrentUserId(this ControllerBase controller)
        {
            
            Console.WriteLine("ClaimTypes NameIdentifier === "+ ClaimTypes.NameIdentifier);

            var claim = controller.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            return int.TryParse(claim, out var id) ? id : null;
        }

        public static bool IsStudent(this ControllerBase controller) =>
            controller.User.IsInRole("Student");

        public static bool IsFaculty(this ControllerBase controller) =>
            controller.User.IsInRole("Faculty");

        public static bool IsAdmin(this ControllerBase controller) =>
            controller.User.IsInRole("Admin");

        public static bool CanViewAllRecords(this ControllerBase controller) =>
            controller.IsAdmin() || controller.IsFaculty();
    }
}
