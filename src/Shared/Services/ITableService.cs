using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using OpenDokoBlazor.Shared.ViewModels.Tables;
using Stl.Fusion;

namespace OpenDokoBlazor.Shared.Services
{
    public interface ITableService
    {
        Task AddPlayerToTable(Guid tableId, CancellationToken cancellationToken = default);

        [ComputeMethod(KeepAliveTime = 1)]
        Task<TableViewModel[]> GetTablesAsync(CancellationToken cancellationToken = default);
    }
}
