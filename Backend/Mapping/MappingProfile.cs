using AutoMapper;
using Backend.DTOs.Department;
using Backend.Models;

namespace Backend.Mapping
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            // Models -> DTO (usefull for get,getById, http methods)
            CreateMap<Department, DepartmentResponseDto>()
                .ForMember(
                    dto => dto.UserCount,
                    department => department.MapFrom(department => department.Users != null ? department.Users.Count : 0)
                );

            // DTOs -> Model (usefor full for create,update http methods)
            CreateMap<CreateDepartmentDto, Department>();
        }
    }
}
