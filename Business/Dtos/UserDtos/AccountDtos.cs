using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Business.Dtos.UserDtos.AccountDtos
{
    public class CreateAccountRequestDto
    {
        public int EmployeeId { get; set; }
        public string Username { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public List<int> RoleIds { get; set; } = new List<int>();
    }

    public class AccountResponseDto
    {
        public int Id { get; set; }
        public string Username { get; set; }
        public string EmployeeName { get; set; }
        public List<string> Roles { get; set; } = new List<string>();
        public bool IsActive { get; set; }
    }
}
