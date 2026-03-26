using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Business.Dtos.AuthDtos
{
    public class UserRegisterResponseDto
    {
        public string? Message { get; set; }
        public string? Username { get; set; }
        public string? Email { get; set; }
    }
}
