using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Business.Dtos.EmployeeDtos
{
    public class UpdateEmployeeDto
    {
        public string FullName { get; set; } = string.Empty;
        public string? Phone { get; set; }
        public DateTime DoB { get; set; }
        public int DepartmentId { get; set; }
        public int PositionId { get; set; }
        public string Status { get; set; } = "Active";
    }
}
