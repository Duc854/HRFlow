using System;
using System.ComponentModel.DataAnnotations;

namespace Business.Dtos.AttendanceDtos
{
    public class CreateLeaveRequestDto
    {
        [Required(ErrorMessage = "Vui lòng chọn loại nghỉ phép")]
        public string LeaveType { get; set; } = string.Empty;

        [Required]
        public DateTime FromDate { get; set; }

        [Required]
        public DateTime ToDate { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập lý do")]
        public string Reason { get; set; } = string.Empty;
    }
}