using Shared.Wrappers;

namespace HRFlow.Business.Interfaces
{
    public interface IApprovalService
    {
        Task<ResponseDto<List<LeaveRequestApprovalDto>>> GetPendingRequestsAsync();
        Task<ResponseDto<List<string>>> GetEmployeesOnLeaveTodayAsync();
        Task<ResponseDto<bool>> ApproveOrRejectRequestAsync(int directorId, ApprovalActionDto dto);
    }
}