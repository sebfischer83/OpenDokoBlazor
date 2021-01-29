using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using OpenDokoBlazor.Server.Classes;
using OpenDokoBlazor.Shared.Services;
using OpenDokoBlazor.Shared.ViewModels.Card;
using OpenDokoBlazor.Shared.ViewModels.Tables;
using OpenIddict.Validation.AspNetCore;
using Stl.Fusion.Server;

namespace OpenDokoBlazor.Server.Controllers.Table
{
    [Route("api/[controller]")]
    [ApiController, JsonifyErrors]
    [Authorize(AuthenticationSchemes = OpenIddictValidationAspNetCoreDefaults.AuthenticationScheme)]
    public class GameController : ControllerBase, IGameService
    {
        private readonly IGameService _gameService;

        public GameController(IGameService gameService)
        {
            _gameService = gameService;
        }

        [HttpPost("addPlayer"), Publish]
        public Task AddPlayerToTable(Guid tableId, CancellationToken cancellationToken = default)
        {
            return _gameService.AddPlayerToTable(tableId, cancellationToken);
        }

        [HttpPost("removePlayer"), Publish]
        public Task RemovePlayerFromTable(Guid tableId, CancellationToken cancellationToken = default)
        {
            return _gameService.RemovePlayerFromTable(tableId, cancellationToken);
        }

        [HttpGet("getTables"), Publish]
        public Task<TableViewModel[]> GetTablesAsync(CancellationToken cancellationToken = default)
        {
            return _gameService.GetTablesAsync(cancellationToken);
        }

        [HttpGet("getDeck"), Publish]
        public Task<DeckViewModel> GetUserDeck(Guid tableId, CancellationToken cancellationToken = default)
        {
            return _gameService.GetUserDeck(tableId, cancellationToken);
        }

        [HttpGet("getPlacableCards"), Publish]
        public Task<List<Guid>> GetPlacableCards(Guid tableId, CancellationToken cancellationToken = default)
        {
            return _gameService.GetPlacableCards(tableId, cancellationToken);
        }

        [HttpGet("getGameStartedState"), Publish]
        public Task<bool> GetGameStartedState(Guid tableId, CancellationToken cancellationToken = default)
        {
            return _gameService.GetGameStartedState(tableId, cancellationToken);
        }
    }
}
