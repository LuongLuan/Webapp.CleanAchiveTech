
using Domain.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Microsoft.Net.Http.Headers;
using System.IdentityModel.Tokens.Jwt;
using System.Net;
using Web.API.DTO;
using Infrastructure.Interface;
using Infrastructure.DTO;
using Infrastructure.IdentityExtension;
using Web.API.Validation;
using Infrastructure.Common;

namespace Web.API.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly JwtTokenConfigDto _jwtTokenConfig;
        private readonly IHttpContextAccessor _httpContextAccessor;
        public AuthController(
            IOptions<JwtTokenConfigDto> jwtTokenConfig,
            IHttpContextAccessor httpContextAccessor,
            IUserService userService)
        {
            _jwtTokenConfig = jwtTokenConfig.Value;
            _httpContextAccessor = httpContextAccessor;
            _userService = userService;
        }


        [AllowAnonymous]
        [HttpPost]
        public async Task<IActionResult> Login(UserInputDto loginInput)
        {
            try
            {
                if (!loginInput.Email.IsMailValid())
                    return new JsonResult(new { code = HttpStatusCode.BadRequest });

                ApplicationUser? user = await _userService.GetUserByEmailAsync(loginInput.Email);
                if (user == null )
                {
                    return new JsonResult(new { code = HttpStatusCode.Unauthorized });
                }

                var author = await _userService.AuthenticationAsync(user, loginInput.Password, _jwtTokenConfig);
                if (author.Succeeded)
                {
                    return
                        new JsonResult
                            (new
                                {
                                    code = HttpStatusCode.OK,
                                    data = author.Data
                                }
                            );

                }
                else
                {
                    return new JsonResult(new { code = HttpStatusCode.Unauthorized, data = author.Errors });
                }
            }
            catch (Exception ex)
            {
                return new JsonResult(new { code = HttpStatusCode.Unauthorized });
            }

        }

        [AllowAnonymous]
        [HttpPost]
        public async Task<IActionResult> RefreshToken(RefreshTokenDto refreshTokenInput)
        {
            var result = await _userService.RefreshTokenAsync(refreshTokenInput.RefreshToken, _jwtTokenConfig);

            if (result.Succeeded)
            {
                return
                    new JsonResult
                    (new APIResultDto
                    {
                        Code = HttpStatusCode.OK,
                        Data = result.Data
                    }
                    );

            }
            else
            {
                return new JsonResult(new { code = HttpStatusCode.Unauthorized, data = result.Errors });
            }

        }
        [Authorize]
        [HttpGet]
        public async Task<IActionResult> GetCurrentUser()
        {
            var u = HttpContext.User;
            ApplicationUser? user = await User.ToAppUser(_userService.GetUserAsync!);
            //ApplicationUser? user = await _identityUserService.GetUserAsync(u);

            if (user == null)
            {
                return new JsonResult(new APIResultDto() { Code = HttpStatusCode.Unauthorized });
            }

            return new JsonResult(new APIResultDto() { Code = HttpStatusCode.OK, Data = user });
        }

        [AllowAnonymous]
        [HttpPost]
        public async Task<IActionResult> Register(RegisterDto registerInput)
        {
            var userEmail = await _userService.GetUserByEmailAsync(registerInput.Email);
            if (userEmail != null)
            {
                return new JsonResult(new APIResultDto() 
                {
                    Code = HttpStatusCode.Unauthorized,
                    Data = "Email address already exists"
                });
            }
            if (registerInput.Password != registerInput.ConfirmPassword)
            {
                return new JsonResult(new APIResultDto()
                {
                    Code = HttpStatusCode.BadRequest,
                    Data = "Invalid confirm password!"
                });
            }
            else
            {
                ApplicationUser user = new ApplicationUser()
                {
                    Email = registerInput.Email,
                    UserName = registerInput.Email,

                };
                var res = await _userService.CreateUserAsync(registerInput.Email, registerInput.Password);
                if (!res.Succeeded)
                {
                    return new JsonResult(new APIResultDto() 
                    {
                        Code = HttpStatusCode.OK,
                        Data = res
                    });
                }
                return new JsonResult(new APIResultDto()
                {
                    Code = HttpStatusCode.BadRequest,
                    Data = res.Errors
                });
            }

        }
    }
}
