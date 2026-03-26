using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Business.Dtos.AuthDtos
{
    public class LoginRequestDto
    {
        public string Username { get; set; }
        public string Password { get; set; }
        public string ClientIp { get; set; }
    }
}
