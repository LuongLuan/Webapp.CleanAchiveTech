using Application.DTO;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.IdentityExtension
{
    public static class IdentityResultExtensions
    {
        public static ResultDto ToApplicationResult(this IdentityResult result)
        {
            return result.Succeeded
                ? ResultDto.Success(null)
                : ResultDto.Failure(result.Errors.Select(e => e.Description));
        }
    }
}
