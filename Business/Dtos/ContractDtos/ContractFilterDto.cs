using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Business.Dtos.ContractDtos
{
    public class ContractFilterDto
    {
        public string? EmployeeSearch { get; set; }
        public int? DepartmentId { get; set; }
        public int? PositionId { get; set; }
        public DateTime? JoinedBefore { get; set; }
        public string? ContractType { get; set; }
    }
}
