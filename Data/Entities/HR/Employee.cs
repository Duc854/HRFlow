using Data.Entities.Attendance;
using Data.Entities.Common;
using Data.Entities.Identity;
using Data.Entities.Payroll;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data.Entities.HR
{
    public class Employee : BaseEntity
    {
        [Required, StringLength(150)]
        public string FullName { get; set; } = string.Empty;
        [Required, EmailAddress, StringLength(100)]
        public string Email { get; set; } = string.Empty;

        [DataType(DataType.Date)]
        public DateTime DoB { get; set; }

        [Required, StringLength(10)]
        public string Gender { get; set; } = string.Empty;

        [StringLength(250)]
        public string? Address { get; set; }

        [StringLength(20), Phone]
        public string? Phone { get; set; }

        [DataType(DataType.Date)]
        public DateTime JoinDate { get; set; }

        [Required, StringLength(20)]
        public string Status { get; set; } = "Active";
        public int DepartmentId { get; set; }
        public virtual Department Department { get; set; } = null!;

        public int PositionId { get; set; }
        public virtual Position Position { get; set; } = null!;

        public virtual User? User { get; set; }
        public virtual Contract? ActiveContract { get; set; }

        public virtual ICollection<TimeLog> TimeLogs { get; set; } = new List<TimeLog>();
        public virtual ICollection<LeaveRequest> LeaveRequests { get; set; } = new List<LeaveRequest>();
        public virtual ICollection<EmployeePayroll> Payrolls { get; set; } = new List<EmployeePayroll>();
    }
}
