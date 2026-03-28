namespace Business.Dtos.PayrollDtos
{
    public class PayrollDto
    {
        public int Id { get; set; }
        public int EmployeeId { get; set; }
        public string EmployeeName { get; set; } = string.Empty;
        public string EmployeeCode { get; set; } = string.Empty;
        public int Month { get; set; }
        public int Year { get; set; }
        public decimal BasicSalary { get; set; }
        public int TotalWorkDays { get; set; }
        public decimal TotalSalary { get; set; }
        public decimal Tax { get; set; }
        public decimal Insurance { get; set; }
        public decimal NetSalary { get; set; }
        public bool IsPaid { get; set; }
    }
}