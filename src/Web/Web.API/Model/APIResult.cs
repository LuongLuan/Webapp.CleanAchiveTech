using System.Net;

namespace Web.API.Model
{
    public class APIResult
    {
        public HttpStatusCode Code { get; set; }
        public object? Data { get; set; } 
    }
}
