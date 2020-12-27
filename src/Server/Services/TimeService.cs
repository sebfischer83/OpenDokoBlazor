using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using OpenDokoBlazor.Shared.Services;
using Stl.Fusion;

namespace OpenDokoBlazor.Server.Services
{
    [ComputeService(typeof(ITimeService))]
    public class TimeService : ITimeService
    {
        [ComputeMethod(AutoInvalidateTime = 1, KeepAliveTime = 1)]
        public virtual async Task<DateTime> GetTimeAsync(CancellationToken cancellationToken = default)
        {
            await Task.Delay(250, cancellationToken).ConfigureAwait(false);
            return DateTime.Now;
        }
    }
}
