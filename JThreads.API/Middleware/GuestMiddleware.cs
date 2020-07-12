using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace JThreads.API.Middleware
{
    public class GuestMiddleware
    {

        // check if has guest session token

        // if so, add guest to context

        // if not, generate token and add to cookies

        private readonly RequestDelegate _next;

        public GuestMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext httpContext)
        {        
            if (!httpContext.User.Identity.IsAuthenticated)
            {
                if (httpContext.Request.Cookies.TryGetValue("sessionId", out var existingGuid))
                {
                    httpContext.Items.Add("sessionId", existingGuid);
                } 
                else
                { 
                    var guid = Guid.NewGuid().ToString();
                    httpContext.Items.Add("sessionId", guid);
                    var cookieOptions = new CookieOptions()
                    {
                        Expires = DateTimeOffset.Now.AddDays(365),
                        SameSite = SameSiteMode.Strict
                    };
                    httpContext.Response.Cookies.Append("sessionId", guid, cookieOptions);
                }
            }
            await _next(httpContext);
        }
    }
}
