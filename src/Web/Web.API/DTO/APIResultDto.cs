using System.Net;

namespace Web.API.DTO
{
    public class APIResultDto
    {
        public HttpStatusCode Code { get; set; }
        public object? Data { get; set; } 
    }
}
