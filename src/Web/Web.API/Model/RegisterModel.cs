using Microsoft.VisualBasic;

namespace Web.API.Model
{
    public class RegisterModel
    {
        public string Email { get; set; } = "";
        public string Password { get; set; } = "";
        public string ConfirmPassword { get; set; } = "";
    }
}
