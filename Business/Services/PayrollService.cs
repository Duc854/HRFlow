using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Business.Dtos.PayrollDtos;
using Data.Abstractions;
using Data.Entities.Payroll;
using HRFlow.Business.Interfaces;
using Shared.Wrappers;

namespace HRFlow.Business.Services
{
    public interface IPayrollService
    {
        Task<ResponseDto<List<PayrollDto>>> CalculateMonthlyPayrollAsync(int month, int year);
        Task<ResponseDto<List<PayrollDto>>> GetPayrollPreviewAsync(int month, int year);
        Task<ResponseDto<EmployeePayroll>> GetEmployeePayslipAsync(int employeeId, int month, int year);
        Task<ResponseDto<bool>> ConfirmPayrollAsync(int month, int year);
    }

    public class PayrollService : IPayrollService
    {
        private readonly IUnitOfWork _unitOfWork;
        public PayrollService(IUnitOfWork unitOfWork) { _unitOfWork = unitOfWork; }

        public async Task<ResponseDto<List<PayrollDto>>> CalculateMonthlyPayrollAsync(int month, int year)
        {
            // 1. Lấy tất cả nhân viên đang làm việc
            var employees = await _unitOfWork.Employees.FindAsync(e => !e.IsDeleted);

            // 2. Xóa bản nháp cũ của tháng này (nếu có tính lại)
            var oldDrafts = await _unitOfWork.Payrolls.FindAsync(p => p.Month == month && p.Year == year && !p.IsPaid);
            foreach (var draft in oldDrafts)
            {
                _unitOfWork.Payrolls.Remove(draft);
            }

            var calculatedPayrolls = new List<EmployeePayroll>();
            int standardWorkDays = 22; // Giả định 1 tháng có 22 ngày công chuẩn

            foreach (var emp in employees)
            {
                // Lấy Hợp đồng Active
                var contract = await _unitOfWork.Contracts.GetActiveByEmployeeIdAsync(emp.Id);
                if (contract == null) continue; // Bỏ qua nếu không có hợp đồng

                // Đếm số ngày đi làm thực tế trong tháng (có CheckIn và CheckOut)
                var timeLogs = await _unitOfWork.TimeLogs.GetLogsByMonthAsync(emp.Id, month, year);
                int workDays = timeLogs.Count(t => t.CheckIn != null && t.CheckOut != null);

                // THUẬT TOÁN TÍNH LƯƠNG CƠ BẢN
                decimal dailyRate = contract.BasicSalary / standardWorkDays;
                decimal grossSalary = dailyRate * workDays;

                // Thuế & Bảo hiểm (Giả định đơn giản: BH 10.5%, Thuế 5% nếu Gross > 11tr)
                decimal insurance = grossSalary * 0.105m;
                decimal tax = grossSalary > 11000000 ? (grossSalary - 11000000) * 0.05m : 0;
                decimal netSalary = grossSalary - insurance - tax;

                var newPayroll = new EmployeePayroll
                {
                    EmployeeId = emp.Id,
                    Month = month,
                    Year = year,
                    TotalWorkDays = workDays,
                    TotalSalary = Math.Round(grossSalary, 2),
                    Insurance = Math.Round(insurance, 2),
                    Tax = Math.Round(tax, 2),
                    NetSalary = Math.Round(netSalary, 2),
                    IsPaid = false // Đánh dấu là Bản nháp
                };

                await _unitOfWork.Payrolls.AddAsync(newPayroll);
                calculatedPayrolls.Add(newPayroll);
            }

            await _unitOfWork.CommitAsync();

            // Trả về danh sách vừa tính để FE xem trước
            return await GetPayrollPreviewAsync(month, year);
        }

        public async Task<ResponseDto<List<PayrollDto>>> GetPayrollPreviewAsync(int month, int year)
        {
            var payrolls = await _unitOfWork.Payrolls.FindAsync(p => p.Month == month && p.Year == year);

            var data = new List<PayrollDto>();
            foreach (var p in payrolls)
            {
                var emp = await _unitOfWork.Employees.GetByIdAsync(p.EmployeeId);
                var contract = await _unitOfWork.Contracts.GetActiveByEmployeeIdAsync(p.EmployeeId);

                data.Add(new PayrollDto
                {
                    Id = p.Id,
                    EmployeeId = p.EmployeeId,
                    EmployeeName = emp?.FullName ?? "N/A",
                    EmployeeCode = $"EMP{p.EmployeeId:D5}",
                    Month = p.Month,
                    Year = p.Year,
                    BasicSalary = contract?.BasicSalary ?? 0,
                    TotalWorkDays = p.TotalWorkDays,
                    TotalSalary = p.TotalSalary,
                    Tax = p.Tax,
                    Insurance = p.Insurance,
                    NetSalary = p.NetSalary,
                    IsPaid = p.IsPaid
                });
            }

            return ResponseDto<List<PayrollDto>>.SuccessResult(data.OrderBy(d => d.EmployeeId).ToList());
        }

        public async Task<ResponseDto<EmployeePayroll>> GetEmployeePayslipAsync(int employeeId, int month, int year)
        {
            var payslips = await _unitOfWork.Payrolls.FindAsync(p =>
                p.EmployeeId == employeeId &&
                p.Month == month &&
                p.Year == year &&
                p.IsPaid == true);

            var result = payslips.FirstOrDefault();

            if (result == null)
            {
                return ResponseDto<EmployeePayroll>.FailResult("Chưa có phiếu lương chính thức cho tháng này.");
            }

            return ResponseDto<EmployeePayroll>.SuccessResult(result);
        }

        public async Task<ResponseDto<bool>> ConfirmPayrollAsync(int month, int year)
        {
            // 1. Tìm tất cả các bản ghi lương chưa chốt của tháng đó
            var payrolls = await _unitOfWork.Payrolls.FindAsync(p =>
                p.Month == month &&
                p.Year == year &&
                p.IsPaid == false);

            if (!payrolls.Any())
            {
                return ResponseDto<bool>.FailResult("Không tìm thấy dữ liệu lương tạm tính hoặc bảng lương đã được chốt trước đó.");
            }

            // 2. Cập nhật trạng thái IsPaid = true
            foreach (var item in payrolls)
            {
                item.IsPaid = true;
                _unitOfWork.Payrolls.Update(item);
            }

            // 3. Lưu vào Database
            var result = await _unitOfWork.CommitAsync();

            return result > 0
                ? ResponseDto<bool>.SuccessResult(true)
                : ResponseDto<bool>.FailResult("Lỗi hệ thống khi chốt lương.");
        }
    }
}