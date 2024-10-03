using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTO
{
    public class AppsettingDto
    {
        public const string EnvSectionName = "Appsettings";
        public JwtTokenConfigDto JwtTokenConfigDto { get; set; }
    }
}
