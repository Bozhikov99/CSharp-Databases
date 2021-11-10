using FastFood.Services.DTO;
using FastFood.Services.DTO.Employee;
using System;
using System.Collections.Generic;
using System.Text;

namespace FastFood.Services.Interfaces
{
    public interface IEmployeeService
    {
        void Register(RegisterEmployeeDto dto);

        ICollection<ListAllEmployeesDto> All();
    }
}
