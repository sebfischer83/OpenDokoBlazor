using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using OpenDokoBlazor.Server.Classes;
using OpenDokoBlazor.Server.Data;
using OpenDokoBlazor.Server.Data.Models;
using OpenDokoBlazor.Server.Helper;
using OpenDokoBlazor.Shared.Player;
using OpenDokoBlazor.Shared.Services;
using OpenDokoBlazor.Shared.ViewModels.Tables;
using Stl.Async;
using Stl.Fusion;
using Stl.Fusion.Bridge;

namespace OpenDokoBlazor.Server.Services
{
    [ComputeService(typeof(ITableService))]
    public class TableService : DbServiceBase<OpenDokoContext>, ITableService
    {
        private ILogger<TableService> _log;
        private IPublisher _publisher;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly TableManager _tableManager;

        public TableService(
            IPublisher publisher,
            IServiceProvider services,
            IHttpContextAccessor httpContextAccessor,
            TableManager tableManager,
            ILogger<TableService> log = null) : base(services)
        {
            _log = log ??= NullLogger<TableService>.Instance;
            _publisher = publisher;
            _httpContextAccessor = httpContextAccessor;
            _tableManager = tableManager;
        }

        public virtual Task<TableViewModel[]> GetTablesAsync(CancellationToken cancellationToken = default)
        {
            var res = _tableManager.Select(table =>
                    new TableViewModel(table.Id, table.Order,
                        table.GetPlayers().Select(player => player.Name).ToList()))
                .ToArray();
            var task = Task.FromResult(res);
            return task;
        }

        public virtual async Task AddPlayerToTable(Guid tableId, CancellationToken cancellationToken = default)
        {
            var table = _tableManager[tableId];
            using var scope = Services.CreateScope();
            using var userManager = scope.ServiceProvider.GetService<UserManager<OpenDokoUser>>();

            var user = await userManager.GetUserAsync(_httpContextAccessor.HttpContext.User);
            var player = new Player(Guid.Parse(user.Id), user.UserName);
            table.AddPlayer(player);

            using (Computed.Invalidate())
                GetTablesAsync(CancellationToken.None).Ignore();
        }
    }
}
