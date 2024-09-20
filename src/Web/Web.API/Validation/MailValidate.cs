using System.Net.Mail;

namespace Web.API.Validation
{
    public static class MailValidate
    {
        public static bool IsMailValid( this string emailaddress)
        {
            try
            {
                MailAddress m = new MailAddress(emailaddress);

                return true;
            }
            catch (FormatException)
            {
                return false;
            }
        }
    }
}
