using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Business.Dtos.EmployeeDtos
{
    public class EmployeeListDto
    {
        public int Id { get; set; }
        public string FullName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public DateTime? DoB { get; set; }
        public string PositionName { get; set; } = string.Empty;
        public string Status { get; set; } = "Active";
        public DateTime JoinDate { get; set; }
        public string? Phone { get; set; }
        public string? Address { get; set; }

        //Manager không xem được
        public string DepartmentName { get; set; } = string.Empty;
        public decimal? BaseSalary { get; set; }

        public bool HasAccount { get; set; }
        public string? Username { get; set; }

    }
}
