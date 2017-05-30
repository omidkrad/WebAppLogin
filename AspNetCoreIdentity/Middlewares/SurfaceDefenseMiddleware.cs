using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace AspNetCoreIdentity.Middlewares
{
    using HitDictionary = Dictionary<IPAddress, List<DateTime>>;

    public class SurfaceDefenseMiddleware
    {
        private readonly TimeSpan _timeSlice;
        private readonly int _maxHitsAllowedPerTimeSlice;

        // the Lazy structure provides thread-safety for accessing the HitDictionary
        private static Lazy<HitDictionary> _hitTracker = new Lazy<HitDictionary>(() => new HitDictionary());
        private readonly RequestDelegate _next;

        public SurfaceDefenseMiddleware(RequestDelegate next, TimeSpan timeSlice, int maxHitsAllowedPerTimeSlice)
        {
            _next = next;
            _timeSlice = timeSlice;
            _maxHitsAllowedPerTimeSlice = maxHitsAllowedPerTimeSlice;
        }

        public async Task Invoke(HttpContext context)
        {
            // mock a remote IP for test
            context.Connection.RemoteIpAddress = IPAddress.Parse("25.7.51.129");

            var remoteIpAddress = context.Connection.RemoteIpAddress;
            if (!(remoteIpAddress == null
               || remoteIpAddress.Equals(IPAddress.Loopback)
               || remoteIpAddress.Equals(IPAddress.IPv6Loopback)))
            {
                // continue protecting if request is coming from a remote IP

                var dic = _hitTracker.Value;
                List<DateTime> hitList;
                if (!dic.TryGetValue(remoteIpAddress, out hitList))
                {
                    // track hits for the new IP address
                    hitList = new List<DateTime>(_maxHitsAllowedPerTimeSlice);
                    dic[remoteIpAddress] = hitList;
                }
                else if (hitList.Count >= _maxHitsAllowedPerTimeSlice)
                {
                    // remove first item to make a circular buffer
                    hitList.RemoveAt(0);
                }

                hitList.Add(DateTime.Now);
                var numHitsInLast15Min = hitList.Count(d => (DateTime.Now - d) < _timeSlice);
                if (numHitsInLast15Min >= _maxHitsAllowedPerTimeSlice)
                {
                    await context.Response.WriteAsync("Connection blocked!");
                    return;
                }
            }

            await _next(context);
        }

    }
}
