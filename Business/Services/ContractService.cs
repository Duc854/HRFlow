using Business.Abstractions;
using Business.Dtos.ContractDtos;
using Data.Abstractions;
using Data.Entities.Payroll;
using Microsoft.EntityFrameworkCore;
using Shared.Models;
using Shared.Wrappers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Business.Services
{
    public class ContractService : IContractService
    {
        private readonly IUnitOfWork _unitOfWork;
        public ContractService(IUnitOfWork unitOfWork) => _unitOfWork = unitOfWork;

        public async Task<ResponseDto<ContractDetailDto>> GetContractDetailAsync(UserIdentity identity, int id)
        {
            try
            {
                // 1. Lấy dữ liệu từ Repo
                var contract = await _unitOfWork.Contracts.GetByIdAsync(id);

                if (contract == null)
                    return ResponseDto<ContractDetailDto>.FailResult("404", "Không tìm thấy hợp đồng.");

                var isHighest = identity.IsAdmin || identity.IsDirector;
                var isManagerOfDept = identity.IsManager && contract.Employee.DepartmentId == identity.DepartmentId;
                var isOwnContract = identity.EmployeeId == contract.EmployeeId;

                if (!isHighest && !isManagerOfDept && !isOwnContract)
                {
                    return ResponseDto<ContractDetailDto>.FailResult("403", "Bạn không có quyền xem hợp đồng này.");
                }

                // 3. Mapping sang DTO
                var dto = new ContractDetailDto
                {
                    Id = contract.Id,
                    ContractNumber = contract.ContractNumber,
                    ContractType = contract.ContractType,
                    StartDate = contract.StartDate,
                    EndDate = contract.EndDate,
                    BasicSalary = contract.BasicSalary,
                    EmployeeId = contract.EmployeeId,
                    EmployeeName = contract.Employee.FullName,
                    DepartmentName = contract.Employee.Department?.Name ?? "N/A",
                    PositionName = contract.Employee.Position?.Name ?? "N/A"
                };

                return ResponseDto<ContractDetailDto>.SuccessResult(dto);
            }
            catch (Exception ex)
            {
                return ResponseDto<ContractDetailDto>.FailResult("500", "Lỗi: " + ex.Message);
            }
        }

        public async Task<ResponseDto<IEnumerable<ContractDetailDto>>> SearchContractsAsync(UserIdentity identity, ContractFilterDto filter)
        {
            try
            {
                var query = _unitOfWork.Contracts.GetContractsQuery();

                if (!identity.IsAdmin && !identity.IsDirector)
                {
                        return ResponseDto<IEnumerable<ContractDetailDto>>.FailResult("403", "Bạn không có quyền xem danh sách này.");
                }

                if (!string.IsNullOrWhiteSpace(filter.EmployeeSearch))
                {
                    query = query.Where(c => c.Employee.FullName.Contains(filter.EmployeeSearch)
                                          || c.Employee.Email.Contains(filter.EmployeeSearch));
                }

                if (filter.DepartmentId.HasValue)
                    query = query.Where(c => c.Employee.DepartmentId == filter.DepartmentId);

                if (filter.PositionId.HasValue)
                    query = query.Where(c => c.Employee.PositionId == filter.PositionId);

                if (filter.JoinedBefore.HasValue)
                    query = query.Where(c => c.Employee.JoinDate <= filter.JoinedBefore.Value);

                if (!string.IsNullOrEmpty(filter.ContractType))
                    query = query.Where(c => c.ContractType == filter.ContractType);

                var contracts = await query.ToListAsync();

                var dtoList = contracts.Select(c => new ContractDetailDto
                {
                    Id = c.Id,
                    ContractNumber = c.ContractNumber,
                    ContractType = c.ContractType,
                    StartDate = c.StartDate,
                    EndDate = c.EndDate,
                    BasicSalary = c.BasicSalary,
                    EmployeeId = c.EmployeeId,
                    EmployeeName = c.Employee.FullName,
                    DepartmentName = c.Employee.Department?.Name ?? "N/A",
                    PositionName = c.Employee.Position?.Name ?? "N/A"
                });

                return ResponseDto<IEnumerable<ContractDetailDto>>.SuccessResult(dtoList);
            }
            catch (Exception ex)
            {
                return ResponseDto<IEnumerable<ContractDetailDto>>.FailResult("500", "Lỗi search: " + ex.Message);
            }
        }

        public async Task<ResponseDto<int>> CreateContractAsync(UserIdentity identity, CreateContractDto dto)
        {
            if (!identity.IsAdmin && !identity.IsDirector)
            {
                return ResponseDto<int>.FailResult("403", "Bạn không có thẩm quyền thực hiện thao tác này.");
            }

            await _unitOfWork.BeginTransactionAsync();
            try
            {
                var employee = await _unitOfWork.Employees.GetByIdAsync(dto.EmployeeId);
                if (employee == null) return ResponseDto<int>.FailResult("404", "Không tìm thấy nhân viên.");

                // 1. Tìm hợp đồng đang Active để "End" nó đi
                var currentActiveContract = await _unitOfWork.Contracts.GetActiveByEmployeeIdAsync(dto.EmployeeId);

                if (currentActiveContract != null)
                {
                    // Kết thúc vào ngày trước khi hợp đồng mới bắt đầu
                    currentActiveContract.EndDate = dto.StartDate.AddDays(-1);

                    // Cập nhật Status sang trạng thái đã kết thúc
                    currentActiveContract.Status = "Terminated";

                    _unitOfWork.Contracts.Update(currentActiveContract);
                }

                // 2. Thêm hợp đồng mới (Extend hoặc New đều vào đây)
                var newContract = new Contract
                {
                    ContractNumber = dto.ContractNumber,
                    ContractType = dto.ContractType,
                    StartDate = dto.StartDate,
                    EndDate = dto.EndDate,
                    BasicSalary = dto.BasicSalary,
                    EmployeeId = dto.EmployeeId,
                    Status = "Active" // Hợp đồng mới luôn là Active
                };

                _unitOfWork.Contracts.Add(newContract);

                await _unitOfWork.CommitTransactionAsync();
                return ResponseDto<int>.SuccessResult(newContract.Id);
            }
            catch (Exception ex)
            {
                await _unitOfWork.RollbackTransactionAsync();
                return ResponseDto<int>.FailResult("500", "Lỗi: " + ex.Message);
            }
        }

        public async Task<ResponseDto<bool>> UpdateContractAsync(UserIdentity identity, int id, UpdateContractDto dto)
        {
            if (!identity.IsAdmin && !identity.IsDirector)
                return ResponseDto<bool>.FailResult("403", "Chỉ Admin/Director mới được sửa hợp đồng.");

            try
            {
                var contract = await _unitOfWork.Contracts.GetByIdAsync(id);
                if (contract == null) return ResponseDto<bool>.FailResult("404", "Không tìm thấy hợp đồng.");
                if(contract.Status == "Terminated" || contract.Status == "Expired")
                {
                    return ResponseDto<bool>.FailResult("400", "Không thể sửa hợp đồng cũ. Vui lòng ký hợp đồng mới nếu có thay đổi.");

                }
                // 1. Kiểm tra mốc thời gian
                var today = DateTime.Now;
                var isAfterLockDate = today.Day > 15;
                // Tính tổng số tháng để so sánh cho chuẩn cả năm và tháng
                var contractMonthValue = contract.StartDate.Year * 12 + contract.StartDate.Month;
                var currentMonthValue = today.Year * 12 + today.Month;


                // 2. Kiểm tra xem User có đang sửa Lương hoặc Ngày bắt đầu không
                bool isChangingSensitiveData = contract.BasicSalary != dto.BasicSalary || contract.StartDate != dto.StartDate;

                // 3. Nếu sửa dữ liệu tháng cũ sau ngày 15 -> CHẶN
                if (isAfterLockDate && contractMonthValue < currentMonthValue)
                {
                    if (isChangingSensitiveData)
                    {
                        return ResponseDto<bool>.FailResult("400", "Không thể sửa lương hoặc ngày bắt đầu của tháng trước sau ngày 15. Vui lòng ký hợp đồng mới nếu có thay đổi.");
                    }
                }

                // 4. Nếu hợp lệ thì mới cho Update toàn bộ
                contract.ContractNumber = dto.ContractNumber;
                contract.ContractType = dto.ContractType;
                contract.StartDate = dto.StartDate;
                contract.EndDate = dto.EndDate;
                contract.BasicSalary = dto.BasicSalary;
                contract.Status = dto.Status;

                await _unitOfWork.CommitAsync();
                return ResponseDto<bool>.SuccessResult(true);
            }
            catch (Exception ex)
            {
                return ResponseDto<bool>.FailResult("500", "Lỗi hệ thống: " + ex.Message);
            }
        }

        public async Task<ResponseDto<bool>> TerminateContractAsync(UserIdentity identity, int id, DateTime? terminateDate)
        {
            if (!identity.IsAdmin && !identity.IsDirector)
                return ResponseDto<bool>.FailResult("403", "Chỉ Admin/Director mới có quyền chấm dứt hợp đồng.");

            try
            {
                var contract = await _unitOfWork.Contracts.GetByIdAsync(id);
                if (contract == null) return ResponseDto<bool>.FailResult("404", "Không tìm thấy hợp đồng.");

                // 1. Check trạng thái: Đã đóng rồi thì thôi
                if (contract.Status == "Terminated" || contract.Status == "Expired")
                    return ResponseDto<bool>.FailResult("400", "Hợp đồng này đã kết thúc từ trước.");

                var today = DateTime.Now.Date; // Lấy ngày hiện tại (không tính giờ)
                var effectiveDate = terminateDate?.Date ?? today;

                // 2. CHỐT CHẶN QUÁ KHỨ: Không cho phép chọn ngày trước ngày hôm nay
                if (effectiveDate < today)
                {
                    return ResponseDto<bool>.FailResult("400", "Không thể chấm dứt hợp đồng. Ngày kết thúc phải từ hôm nay trở đi.");
                }

                // 3. Kiểm tra logic với StartDate (Không thể kết thúc trước khi bắt đầu)
                if (effectiveDate < contract.StartDate.Date)
                {
                    return ResponseDto<bool>.FailResult("400", "Ngày kết thúc không thể trước ngày bắt đầu hợp đồng.");
                }

                // 4. Thực hiện cập nhật
                contract.Status = "Terminated";
                contract.EndDate = effectiveDate;

                if (effectiveDate == today && contract.Employee != null)
                {
                    contract.Employee.ActiveContract = null;
                }

                await _unitOfWork.CommitAsync();
                return ResponseDto<bool>.SuccessResult(true);
            }
            catch (Exception ex)
            {
                return ResponseDto<bool>.FailResult("500", "Lỗi hệ thống: " + ex.Message);
            }
        }
    }
}
