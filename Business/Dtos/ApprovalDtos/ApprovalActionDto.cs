public class ApprovalActionDto
{
    public int RequestId { get; set; }
    public bool IsApproved { get; set; } // True = Duyệt, False = Từ chối
    public string? ResponseNote { get; set; } // Lý do Sếp phản hồi
}