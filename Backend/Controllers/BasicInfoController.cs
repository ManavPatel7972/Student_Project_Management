using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;

namespace Backend.Controllers
{
   
   
    // getting the current user's ID from JWT claims.
   
    public class BasicInfoController : ControllerBase
    {

        #region GetCurrentUserId
        protected int? GetCurrentUserId()
        {
            var claim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(claim))
            {
                return null;
            }

            var userID = Convert.ToInt32(claim);
            return userID;
        }

        #endregion


        #region CheckStudentRoleOrNot
        protected bool IsStudent()
        {
            return User.IsInRole("Student");
            
        }
        #endregion


        #region CheckFacultyRoleOrNot
        protected bool IsFaculty() 
        {
            return User.IsInRole("Faculty");
        }
        #endregion


        #region CheckAdminRoleOrNot
        protected bool IsAdmin()
        {
           return User.IsInRole("Admin");

        }
        #endregion


        #region CanViewAllRecord
        protected bool CanViewAllRecords()
        {
            return IsStudent() || IsFaculty();  
        }
        #endregion
    }
}
