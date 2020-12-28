using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Blazorise;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.Extensions.Localization;
using OpenDokoBlazor.Client.Services.Auth;
using Stl.Fusion.Authentication;

namespace OpenDokoBlazor.Client.Pages.Auth
{
    public partial class LoginPage
    {
        [Inject]
        private IStringLocalizer<LoginPage> Loc { get; set; }

        [Inject]
        private ILoginService LoginService { get; set; }

        [Inject]
        private NavigationManager NavigationManager { get; set; }

        [Inject]
        private AuthenticationStateProvider AuthenticationStateProvider { get; set; }

        private Validations _validations;

        private readonly UserData UserData = new UserData();
        private Alert _alert;

        void HideAlert()
        {
            _alert.Hide();
        }

        protected override async Task OnInitializedAsync()
        {
            await base.OnInitializedAsync();
            var auth = await AuthenticationStateProvider.GetAuthenticationStateAsync();
            if (auth.User.Identity.IsAuthenticated)
                NavigationManager.NavigateTo("/");
        }

        async Task SubmitAsync()
        {
            if (_validations.ValidateAll())
            {
                _validations.ClearAll();
                await LoginService.Login(UserData.UserName, UserData.Password, true);
                //if (await LoginService.Login(UserData))
                //{
                //    NavigationManager.NavigateTo("/");
                //}
                //else
                //{
                //    alert.Show();
                //}
            }
        }
    }
}
