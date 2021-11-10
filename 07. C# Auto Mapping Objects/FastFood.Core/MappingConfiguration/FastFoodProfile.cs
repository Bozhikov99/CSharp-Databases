namespace FastFood.Core.MappingConfiguration
{
    using AutoMapper;
    using FastFood.Core.ViewModels.Categories;
    using FastFood.Core.ViewModels.Employees;
    using FastFood.Models;
    using FastFood.Services.DTO;
    using FastFood.Services.DTO.Categories;
    using FastFood.Services.DTO.Employee;
    using FastFood.Services.DTO.Position;
    using System.Security.Cryptography.X509Certificates;
    using ViewModels.Positions;

    public class FastFoodProfile : Profile
    {
        public FastFoodProfile()
        {
            //Positions
            this.CreateMap<CreatePositionInputModel, Position>()
                .ForMember(x => x.Name, y => y.MapFrom(s => s.PositionName));

            this.CreateMap<Position, PositionsAllViewModel>()
                .ForMember(x => x.Name, y => y.MapFrom(s => s.Name));

            CreateMap<Position, EmployeeRegisterPositionsAvailable>()
                .ForMember(x => x.PositionId,
                    y => y.MapFrom(s => s.Id))
                .ForMember(x => x.PositionName, y => y.MapFrom(s => s.Name));


            //Categories
            CreateMap<CreateCategoryInputModel, CreateCategoryDTO>();

            CreateMap<ListAllCategoriesDTO, CategoryAllViewModel>()
                .ForMember(x => x.Name, y => y.MapFrom(s => s.CategoryName));

            CreateMap<CreateCategoryDTO, Category>()
                .ForMember(x => x.Name, y => y.MapFrom(s => s.CategoryName));

            CreateMap<Category, ListAllCategoriesDTO>()
                .ForMember(x => x.CategoryName, y => y.MapFrom(s => s.Name));

            //Employees
            CreateMap<EmployeeRegisterPositionsAvailable, RegisterEmployeeViewModel>();

            CreateMap<RegisterEmployeeInputModel, RegisterEmployeeDto>();

            CreateMap<RegisterEmployeeDto, Employee>();

            CreateMap<ListAllEmployeesDto, EmployeesAllViewModel>();

            CreateMap<Employee, ListAllEmployeesDto>()
                .ForMember(x => x.Position,
                y => y.MapFrom(s => s.Position.Name));
        }
    }
}
