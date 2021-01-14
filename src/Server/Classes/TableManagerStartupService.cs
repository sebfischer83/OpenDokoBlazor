using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;

namespace OpenDokoBlazor.Server.Classes
{
    public class TableManagerStartupService : IHostedService
    {
        private readonly TableManager _tableManager;

        public TableManagerStartupService(TableManager tableManager)
        {
            _tableManager = tableManager;
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            _tableManager.Init();
            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }
    }
}
