using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive;
using System.Security.Claims;
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
using OpenDokoBlazor.Shared.Cards;
using OpenDokoBlazor.Shared.Player;
using OpenDokoBlazor.Shared.Services;
using OpenDokoBlazor.Shared.ViewModels.Card;
using OpenDokoBlazor.Shared.ViewModels.Tables;
using OpenIddict.Abstractions;
using Stl.Async;
using Stl.CommandR;
using Stl.Fusion;
using Stl.Fusion.Bridge;
using Stl.Fusion.Operations;

namespace OpenDokoBlazor.Server.Services
{
    [ComputeService(typeof(IGameService))]
    public class GameService : DbServiceBase<OpenDokoContext>, IGameService
    {
        private ILogger<GameService> _log;
        private IPublisher _publisher;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly TableManager _tableManager;

        public GameService(
            IPublisher publisher,
            IServiceProvider services,
            IHttpContextAccessor httpContextAccessor,
            TableManager tableManager,
            ILogger<GameService> log = null) : base(services)
        {
            _log = log ??= NullLogger<GameService>.Instance;
            _publisher = publisher;
            _httpContextAccessor = httpContextAccessor;
            _tableManager = tableManager;
        }

        public Task RemovePlayerFromTable(Guid tableId, CancellationToken cancellationToken = default)
        {
            var table = _tableManager[tableId];

            var userId = _httpContextAccessor.HttpContext?.User.GetClaim(OpenIddictConstants.Claims.Subject);
            if (string.IsNullOrWhiteSpace(userId))
                throw new Exception(nameof(userId));

            if (!_tableManager.IsPlayerAlreadyInGame(new Guid(userId)))
                throw new Exception("Player is not on this table");

            table.RemovePlayer(Guid.Parse(userId));

            using (Computed.Invalidate())
                GetTablesAsync(CancellationToken.None).Ignore();

            return Task.CompletedTask;
        }

        public virtual Task<TableViewModel[]> GetTablesAsync(CancellationToken cancellationToken = default)
        {
            var res = _tableManager.Select(table =>
                    new TableViewModel(table.Id, table.Order,
                        table.GetPlayers().Select(player => player.ToViewModel()).ToList(), table.IsSoloTable))
                .ToArray();
            var task = Task.FromResult(res);
            return task;
        }

        public virtual Task<DeckViewModel> GetUserDeck(Guid tableId, CancellationToken cancellationToken = default)
        {
            var table = _tableManager[tableId];
            var userId = _httpContextAccessor.HttpContext?.User.GetClaim(OpenIddictConstants.Claims.Subject);
            if (string.IsNullOrWhiteSpace(userId))
                throw new Exception(nameof(userId));
            var game = table.CurrentGame;
            var cards = game.GetCardsForPlayer(new Guid(userId)).Cards;
            var model = new DeckViewModel() { Cards = cards.Select(card => card.ToViewModel())
                .Reverse().ToList()
            };
            return Task.FromResult(model);
        }

        public virtual Task<List<Guid>> GetPlacableCards(Guid tableId, CancellationToken cancellationToken = default)
        {
            var table = _tableManager[tableId];
            var userId = _httpContextAccessor.HttpContext?.User.GetClaim(OpenIddictConstants.Claims.Subject);
            if (string.IsNullOrWhiteSpace(userId))
                throw new Exception(nameof(userId));
            var game = table.CurrentGame;
            var res = game?.GetPlacableCards(new Guid(userId));

            return Task.FromResult(res);
        }

        public Task<bool> GetGameStartedState(Guid tableId, CancellationToken cancellationToken = default)
        {
            var table = _tableManager[tableId];

            return Task.FromResult(table.IsGameRunning);
        }

        public virtual Task AddPlayerToTable(Guid tableId, CancellationToken cancellationToken = default)
        {
            var table = _tableManager[tableId];
            var userId = _httpContextAccessor.HttpContext?.User.GetClaim(OpenIddictConstants.Claims.Subject);
            var userName = _httpContextAccessor.HttpContext?.User.GetClaim(OpenIddictConstants.Claims.Username);
            if (string.IsNullOrWhiteSpace(userId))
                throw new Exception(nameof(userId));
            if (string.IsNullOrWhiteSpace(userName))
                throw new Exception(nameof(userName));
            if (_tableManager.IsPlayerAlreadyInGame(new Guid(userId)))
                throw new Exception("Player is already in game");
            
            table.AddPlayer(Guid.Parse(userId), userName);

            using (Computed.Invalidate())
            {
                GetTablesAsync(CancellationToken.None).Ignore();
                GetGameStartedState(tableId, CancellationToken.None).Ignore();
            }

            return Task.CompletedTask;
        }
    }
}
