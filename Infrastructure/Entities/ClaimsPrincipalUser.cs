using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Entities
{
    public class ClaimsPrincipalUser
    {
        public Guid Id { get; set; }
        public string Email { get; set; } = "";
        public List<string> Roles { get; set; } = new();
        public string? UserName { get; set; }
    }
}
