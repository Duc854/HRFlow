using Data.Entities.Common;
using Data.Entities.HR;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data.Entities.Attendance
{
    public class LeaveRequest : BaseEntity
    {
        [Required, StringLength(50)]
        public string LeaveType { get; set; } = string.Empty;

        [DataType(DataType.Date)]
        public DateTime FromDate { get; set; }

        [DataType(DataType.Date)]
        public DateTime ToDate { get; set; }

        public double TotalDays { get; set; }

        [StringLength(500)]
        public string? Reason { get; set; }

        [Required, StringLength(20)]
        public string Status { get; set; } = "Pending";

        public int? ApprovedById { get; set; }
        public virtual Employee? ApprovedBy { get; set; }

        public int EmployeeId { get; set; }
        public virtual Employee Employee { get; set; } = null!;
    }
}
