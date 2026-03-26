using Shared.Models;
using System.Security.Claims;

namespace API.Extensions
{
    public static class HttpContextExtensions
    {
        public static UserIdentity? GetUserIdentity(this HttpContext httpContext)
        {
            // 1. Kiểm tra xem User đã qua middleware Authentication chưa
            if (httpContext.User.Identity?.IsAuthenticated != true)
                return null;

            var user = httpContext.User;

            // 2. Trích xuất thông tin cơ bản
            int.TryParse(user.FindFirst(ClaimTypes.NameIdentifier)?.Value, out int userId);

            // Lấy EmployeeId và DepartmentId (đã nhét vào Token lúc Generate)
            int? employeeId = int.TryParse(user.FindFirst("EmployeeId")?.Value, out int eId) ? eId : null;
            int? departmentId = int.TryParse(user.FindFirst("DepartmentId")?.Value, out int dId) ? dId : null;

            var username = user.FindFirst(ClaimTypes.Name)?.Value ?? string.Empty;

            // 3. Xử lý đa quyền hạn (n-n Role)
            var roles = user.FindAll(ClaimTypes.Role)
                            .Select(c => c.Value)
                            .ToList();

            // 4. Trả về Object UserIdentity sạch sẽ cho Business dùng
            return new UserIdentity
            {
                UserId = userId,
                EmployeeId = employeeId,
                DepartmentId = departmentId,
                Username = username,
                Roles = roles
            };
        }
    }
}
