namespace FastFood.Core.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using AutoMapper;
    using Data;
    using FastFood.Models;
    using FastFood.Services.DTO;
    using FastFood.Services.DTO.Employee;
    using FastFood.Services.DTO.Position;
    using FastFood.Services.Interfaces;
    using Microsoft.AspNetCore.Mvc;
    using ViewModels.Employees;

    public class EmployeesController : Controller
    {
        private readonly IPositionService positionService;
        private readonly IMapper mapper;
        private readonly IEmployeeService employeeService;

        public EmployeesController(IMapper mapper, IPositionService positionService, IEmployeeService employeeService)
        {
            this.mapper = mapper;
            this.positionService = positionService;
            this.employeeService = employeeService;
        }

        public IActionResult Register()
        {
            ICollection<EmployeeRegisterPositionsAvailable> positionsDto =
                this.positionService.GetPositionsAvailable();

            List<RegisterEmployeeViewModel> regViewModel = mapper
                .Map<ICollection<EmployeeRegisterPositionsAvailable>,
                     ICollection<RegisterEmployeeViewModel>>(positionsDto)
                .ToList();

            return View(regViewModel);
        }

        [HttpPost]
        public IActionResult Register(RegisterEmployeeInputModel model)
        {
            if (!ModelState.IsValid)
            {
                return RedirectToAction("Register");
            }

            RegisterEmployeeDto employeeDto = mapper.Map<RegisterEmployeeDto>(model);
            employeeService.Register(employeeDto);

            return RedirectToAction("All");
        }

        public IActionResult All()
        {
            ICollection<ListAllEmployeesDto> employeesDtos = employeeService.All();

            List<EmployeesAllViewModel> employeesViewModels = mapper
                .Map<ICollection<ListAllEmployeesDto>,
                ICollection<EmployeesAllViewModel>>(employeesDtos)
                .ToList();

            return View(employeesViewModels);
        }
    }
}
