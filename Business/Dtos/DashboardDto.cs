using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Business.Dtos.DashboardDto
{
    public class DashboardStatsDto
    {
        public int TotalEmployees { get; set; }
        public int NewEmployeesThisMonth { get; set; }
        public int TotalDepartments { get; set; }

        // Dữ liệu cho biểu đồ tròn (Nhân viên theo phòng ban)
        public List<ChartDataDto> EmployeeByDepartment { get; set; } = new();

        // Danh sách 5-10 nhân viên sắp hết hạn hợp đồng (Thông báo)
        public List<ExpiringContractDto> ExpiringContracts { get; set; } = new();
    }

    public class ChartDataDto
    {
        public string Label { get; set; } = string.Empty;
        public int Value { get; set; }
    }

    public class ExpiringContractDto
    {
        public int EmployeeId { get; set; }
        public string FullName { get; set; } = string.Empty;
        public DateTime ExpiryDate { get; set; }
        public int DaysLeft { get; set; }
    }
}
