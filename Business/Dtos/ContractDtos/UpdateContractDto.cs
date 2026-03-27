using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Business.Dtos.ContractDtos
{
    public class UpdateContractDto
    {
        public string ContractNumber { get; set; } = string.Empty;
        public string ContractType { get; set; } = string.Empty;

        public decimal BasicSalary { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime? EndDate { get; set; }

    }
}
