using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using OpenDokoBlazor.Shared.Services;
using OpenDokoBlazor.Shared.ViewModels.Card;
using OpenDokoBlazor.Shared.ViewModels.Tables;
using RestEase;
using Stl.Fusion.Client;

namespace OpenDokoBlazor.Client.Services.Game
{
    [RestEaseReplicaService(typeof(IGameService), Scope = Program.ClientSideScope)]
    [BasePath("game")]
    public interface IGameClient
    {
        [Post("addPlayer")]
        Task AddPlayerToTable(Guid tableId, CancellationToken cancellationToken = default);

        [Post("removePlayer")]
        Task RemovePlayerFromTable(Guid tableId, CancellationToken cancellationToken = default);

        [Get("getTables")] 
        Task<TableViewModel[]> GetTablesAsync(CancellationToken cancellationToken = default);

        [Get("getDeck")]
        Task<DeckViewModel> GetUserDeck(Guid tableId, CancellationToken cancellationToken = default);

        [Get("getPlacableCards")]
        Task<List<Guid>> GetPlacableCards(Guid tableId, CancellationToken cancellationToken = default);

        [Get("getGameStartedState")]
        Task<bool> GetGameStartedState(Guid tableId, CancellationToken cancellationToken = default);
    }
    
}
