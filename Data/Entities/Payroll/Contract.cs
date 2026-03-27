using Data.Entities.Common;
using Data.Entities.HR;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data.Entities.Payroll
{
    public class Contract : BaseEntity
    {
        [Required, StringLength(50)]
        public string ContractNumber { get; set; } = string.Empty;

        [Required, StringLength(50)]
        public string ContractType { get; set; } = string.Empty;

        [DataType(DataType.Date)]
        public DateTime StartDate { get; set; }

        [DataType(DataType.Date)]
        public DateTime? EndDate { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal BasicSalary { get; set; }

        [Required, StringLength(20)]
        public string Status { get; set; } = "Active";

        public int EmployeeId { get; set; }
        public virtual Employee Employee { get; set; } = null!;
    }
}
