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
    }

    public class AccountResponseDto
    {
        public int Id { get; set; }
        public string Username { get; set; }
        public string EmployeeName { get; set; }
        public bool IsActive { get; set; }
    }
}
