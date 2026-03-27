using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Business.Dtos.EmployeeDtos
{
    public class CreateEmployeeDto
    {
        public string FullName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Gender { get; set; } = string.Empty;
        public string? Address { get; set; }
        public string? Phone { get; set; }
        public DateTime DoB { get; set; }
        public int DepartmentId { get; set; }
        public int PositionId { get; set; }
        public DateTime JoinDate { get; set; } = DateTime.Now;
    }
}
