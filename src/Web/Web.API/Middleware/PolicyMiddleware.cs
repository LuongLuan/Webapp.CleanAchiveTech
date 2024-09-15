using Microsoft.AspNetCore.Http.Features;
using Web.API.Constants;
using Web.API.CustomAtrribute;

namespace Web.API.Middleware
{
    public class PolicyMiddleware
    {
        private readonly RequestDelegate _next;
        public PolicyMiddleware(RequestDelegate next)
        {
            _next = next;
        }
        public async Task InvokeAsync(HttpContext context)
        {
            // check for the attribute and skip everything else if it exists
            var endpoint = context.Features.Get<IEndpointFeature>()?.Endpoint;
            var policy = endpoint?.Metadata.GetMetadata<CustomPolicy>();
            if (policy != null)
            {
                if (policy.PolicyName == ConstantAPI.VIEW_PRODUCT)
                {
                    await _next(context);
                }
                return;
            }
            await _next(context);
        }
    }
}
