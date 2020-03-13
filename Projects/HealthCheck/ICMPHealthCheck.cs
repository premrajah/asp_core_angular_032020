using Microsoft.Extensions.Diagnostics.HealthChecks; //
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.NetworkInformation; //
using System.Threading;
using System.Threading.Tasks;

namespace HealthCheck
{
    public class ICMPHealthCheck : IHealthCheck
    {
        private string _Host { get; set; }
        private int _Timeout { get; set; }

        public ICMPHealthCheck(string host, int timeout)
        {
            _Host = host;
            _Timeout = timeout;
        }

        public async Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
        {
            try
            {
                using(var ping = new Ping())
                {
                    var reply = await ping.SendPingAsync(_Host);

                    switch (reply.Status)
                    {
                        case IPStatus.Success:
                            var msg = String.Format($"IMCP to {_Host} took {reply.RoundtripTime} ms");

                            return (reply.RoundtripTime > _Timeout) ? HealthCheckResult.Degraded(msg) : HealthCheckResult.Healthy(msg);
                        default:
                            var error = String.Format($"IMCP to {_Host} failed: {reply.Status}");
                            return HealthCheckResult.Unhealthy(error);
                    }
                };
            }
            catch (Exception ex)
            {
                var error = String.Format($"IMCP to {_Host} failed {ex.Message}");
                return HealthCheckResult.Unhealthy(error);
            }
        }
    }
}
