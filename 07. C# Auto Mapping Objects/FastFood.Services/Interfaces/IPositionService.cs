using FastFood.Services.DTO.Position;
using System;
using System.Collections.Generic;
using System.Text;

namespace FastFood.Services.Interfaces
{
    public interface IPositionService
    {
        ICollection<EmployeeRegisterPositionsAvailable> GetPositionsAvailable();
    }
}
