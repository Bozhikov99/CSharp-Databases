using System;
using System.Collections.Generic;
using System.Text;

namespace TeisterMask.DataProcessor.ExportDto
{
    public class EmployeeOutputDTO
    {
        public string Username { get; set; }

        public ICollection<TaskOutputDTO> Tasks { get; set; }
    }
}
