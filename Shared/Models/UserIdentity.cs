using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.Models
{
    public class UserIdentity
    {
        public required string UserId { get; init; }
        public required string Username { get; init; }
        public required string Role { get; init; }
    }
}
