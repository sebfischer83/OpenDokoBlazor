using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Excubo.Blazor.Canvas;
using Microsoft.AspNetCore.Components;
using OpenDokoBlazor.Shared.ViewModels.Tables;

namespace OpenDokoBlazor.Client.Components.Table
{
    public partial class TableElement
    {
        [Parameter]
        public TableViewModel? Model { get; set; }

        [Parameter] 
        public EventCallback<Guid> OnParticipatEventCallback { get; set; }

        private Canvas? _helperCanvas;

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender)
            {
                await RenderTable();
            }
        }

        private async Task RenderTable()
        {
            if (_helperCanvas == null)
                return;
            await using var ctx1 = await _helperCanvas.GetContext2DAsync();
            await ctx1.ClearRectAsync(0, 0, 300, 300);
            //await ctx1.FillStyleAsync("gray");
            //await ctx1.FillRectAsync(0, 0, 300, 300);
            await ctx1.FontAsync("18px solid");
            int posY = 18;
            if (Model != null)
            {
                for (int i = 0; i < 4; i++)
                {
                    var player = Model.PlayerList.FirstOrDefault(model => model.Order == i);
                    if (player != null)
                        await ctx1.FillTextAsync(player.Name, 0, posY);
                    else
                        await ctx1.FillTextAsync("leer", 0, posY);
                    posY += 20;
                }
            }
            else
            {
                for (int i = 0; i < 4; i++)
                {
                    await ctx1.FillTextAsync("leer", 0, posY);
                    posY += 20;
                }
            }
        }

        protected override async Task OnParametersSetAsync()
        {
            await RenderTable();
        }
    }
}
