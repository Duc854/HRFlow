using Data.Abstractions;
using HRFlow.Business.Interfaces;
using Shared.Wrappers;

namespace Application.Services
{
    public class ApprovalService : IApprovalService
    {
        private readonly IUnitOfWork _unitOfWork;
        public ApprovalService(IUnitOfWork unitOfWork) => _unitOfWork = unitOfWork;

        // 1. Lấy danh sách đơn đang chờ (Pending)
        public async Task<ResponseDto<List<LeaveRequestApprovalDto>>> GetPendingRequestsAsync()
        {
            // Bác cần đảm bảo Query này có .Include(r => r.Employee)
            var requests = await _unitOfWork.LeaveRequests.GetPendingWithEmployeeAsync();

            var data = requests.Select(r => new LeaveRequestApprovalDto
            {
                Id = r.Id,
                EmployeeId = r.EmployeeId,
                // Chỗ này r.Employee sẽ không còn null nữa
                EmployeeName = r.Employee?.FullName ?? "Không xác định",
                LeaveType = r.LeaveType,
                FromDate = r.FromDate,
                ToDate = r.ToDate,
                Reason = r.Reason,
                Status = r.Status
            }).ToList();

            return ResponseDto<List<LeaveRequestApprovalDto>>.SuccessResult(data);
        }

        // 2. Lấy danh sách tên nhân viên nghỉ hôm nay
        public async Task<ResponseDto<List<string>>> GetEmployeesOnLeaveTodayAsync()
        {
            var today = DateTime.Today;
            var onLeave = await _unitOfWork.LeaveRequests.GetEmployeesOnLeaveByDateAsync(today);

            var names = onLeave
                .Where(r => r.Employee != null)
                .Select(r => r.Employee.FullName)
                .Distinct()
                .ToList();

            return ResponseDto<List<string>>.SuccessResult(names);
        }

        // 3. Xử lý Duyệt hoặc Từ chối
        public async Task<ResponseDto<bool>> ApproveOrRejectRequestAsync(int directorId, ApprovalActionDto dto)
        {
            var request = await _unitOfWork.LeaveRequests.GetByIdAsync(dto.RequestId);
            if (request == null) return ResponseDto<bool>.FailResult("Không tìm thấy đơn này.");

            request.Status = dto.IsApproved ? "Approved" : "Rejected";
            request.ApprovedById = directorId; // Lưu ID người duyệt
                                               // Giả sử bác có cột ResponseNote trong DB để lưu lời nhắn của Sếp
                                               // request.ResponseNote = dto.ResponseNote; 

            _unitOfWork.LeaveRequests.Update(request);
            var result = await _unitOfWork.CommitAsync();

            return result > 0 ? ResponseDto<bool>.SuccessResult(true) : ResponseDto<bool>.FailResult("Lỗi hệ thống khi cập nhật.");
        }
    }
}