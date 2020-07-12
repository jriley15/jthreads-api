using JThreads.API.Middleware;
using Microsoft.AspNetCore.Builder;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace JThreads.API.Extensions
{
    public static class CustomMiddlewareExtensions
    {
        public static IApplicationBuilder UseGuestMiddleware(
         this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<GuestMiddleware>();
        }
    }
}
