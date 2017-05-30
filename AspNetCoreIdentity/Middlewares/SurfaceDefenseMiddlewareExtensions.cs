using System;
using Microsoft.AspNetCore.Builder;

namespace AspNetCoreIdentity.Middlewares
{
    public static class SurfaceDefenseMiddlewareExtensions
    {
        public static IApplicationBuilder UseSurfaceDefense(this IApplicationBuilder builder, TimeSpan timeSlice, int maxHitsAllowedPerTimeSlice)
        {
            return builder.UseMiddleware<SurfaceDefenseMiddleware>(timeSlice, maxHitsAllowedPerTimeSlice);
        }
    }
}
