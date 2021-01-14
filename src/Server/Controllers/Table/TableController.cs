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
using OpenDokoBlazor.Shared.ViewModels.Tables;
using OpenIddict.Validation.AspNetCore;
using Stl.Fusion.Server;

namespace OpenDokoBlazor.Server.Controllers.Table
{
    [Route("api/[controller]")]
    [ApiController, JsonifyErrors]
    [Authorize(AuthenticationSchemes = OpenIddictValidationAspNetCoreDefaults.AuthenticationScheme)]
    public class TableController : ControllerBase, ITableService
    {
        private readonly ITableService _tableService;

        public TableController(ITableService tableService)
        {
            _tableService = tableService;
        }

        [HttpPost("addPlayer"), Publish]
        public Task AddPlayerToTable(Guid tableId, CancellationToken cancellationToken = default)
        {
            return _tableService.AddPlayerToTable(tableId, cancellationToken);
        }

        [HttpGet("getTables"), Publish]
        public Task<TableViewModel[]> GetTablesAsync(CancellationToken cancellationToken = default)
        {
            return _tableService.GetTablesAsync(cancellationToken);
        }
    }
}
