using AutoMapper;
using AutoMapper.QueryableExtensions;
using FastFood.Data;
using FastFood.Models;
using FastFood.Services.DTO;
using FastFood.Services.DTO.Employee;
using FastFood.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FastFood.Services
{
    public class EmployeeService : IEmployeeService
    {
        private readonly FastFoodContext dbContext;
        private readonly IMapper mapper;
        public EmployeeService(FastFoodContext dbContext, IMapper mapper)
        {
            this.dbContext = dbContext;
            this.mapper = mapper;
        }
        public ICollection<ListAllEmployeesDto> All()
        => dbContext.Employees
            .ProjectTo<ListAllEmployeesDto>(mapper.ConfigurationProvider)
            .ToList();

        public void Register(RegisterEmployeeDto dto)
        {
            Employee employee = mapper.Map<Employee>(dto);
            dbContext.Employees.Add(employee);
            dbContext.SaveChanges();
        }
    }
}
