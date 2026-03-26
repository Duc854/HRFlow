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
    public class TimeLog : BaseEntity
    {
        [DataType(DataType.Date)]
        public DateTime Date { get; set; }

        public TimeSpan CheckIn { get; set; }
        public TimeSpan? CheckOut { get; set; }

        [Required, StringLength(20)]
        public string Status { get; set; } = "Present";

        public int EmployeeId { get; set; }
        public virtual Employee Employee { get; set; } = null!;
    }
}
