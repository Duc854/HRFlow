using Data.Entities.Common;
using Data.Entities.HR;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data.Entities.Payroll
{
    public class EmployeePayroll : BaseEntity
    {
        public int Month { get; set; }
        public int Year { get; set; }

        public int TotalWorkDays { get; set; } 
        public int TotalLeaveDays { get; set; } 

        [Column(TypeName = "decimal(18,2)")]
        public decimal TotalSalary { get; set; } 

        [Column(TypeName = "decimal(18,2)")]
        public decimal Tax { get; set; } 

        [Column(TypeName = "decimal(18,2)")]
        public decimal Insurance { get; set; } 

        [Column(TypeName = "decimal(18,2)")]
        public decimal NetSalary { get; set; }

        public bool IsPaid { get; set; } = false;

        public int EmployeeId { get; set; }
        public virtual Employee Employee { get; set; } = null!;
    }
}
