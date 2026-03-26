using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.Models
{
    public class UserIdentity
    {
        public int UserId { get; set; }
        public int? EmployeeId { get; set; }
        public int? DepartmentId { get; set; }
        public string Username { get; set; } = string.Empty;
        public List<string> Roles { get; set; } = new(); // Dùng List thay vì string đơn

        // Helper check nhanh để tầng Business đỡ phải viết .Contains
        public bool IsAdmin => Roles.Contains("Admin");
        public bool IsDirector => Roles.Contains("Director");
        public bool IsManager => Roles.Contains("Manager");
    }
}
