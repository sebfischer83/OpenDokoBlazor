using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using OpenDokoBlazor.Shared.Services;
using OpenDokoBlazor.Shared.ViewModels.Tables;
using RestEase;
using Stl.Fusion.Client;

namespace OpenDokoBlazor.Client.Services.Table
{
    [RestEaseReplicaService(typeof(ITableService), Scope = Program.ClientSideScope)]
    [BasePath("table")]
    public interface ITableClient
    {
        [Post("addPlayer")]
        Task AddPlayerToTable(Guid tableId, CancellationToken cancellationToken = default);

        [Get("getTables")] 
        Task<TableViewModel[]> GetTablesAsync(CancellationToken cancellationToken = default);
    }
    
}
