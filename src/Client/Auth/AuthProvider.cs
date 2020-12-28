using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components.Authorization;
using OpenDokoBlazor.Client.Pages.Auth;
using Stl.Fusion;
using Stl.Fusion.Blazor;

namespace OpenDokoBlazor.Client.Auth
{
    public class AuthProvider : AuthenticationStateProvider
    {
        private ClaimsPrincipal _claimsPrincipal = new ClaimsPrincipal(new ClaimsIdentity());
        public override async Task<AuthenticationState> GetAuthenticationStateAsync()
        {
            await Task.FromResult(0);
            return new AuthenticationState(_claimsPrincipal);
        }

        public void MarkUserAsLoggedIn(UserData data)
        {
            //ClaimsIdentity identity;
            //_dispatcher.Dispatch(new LoginAction() { UserName = data.UserName, Password = data.Password, ServerUrl = data.ServerUrl });
            //identity = new ClaimsIdentity(new[] { new Claim(ClaimTypes.Name, data.UserName), new Claim(ClaimTypes.Email, data.UserName) }, "traccarlogin");
            //var user = new ClaimsPrincipal(identity);
            //var state = Task.FromResult(new AuthenticationState(user));
            //NotifyAuthenticationStateChanged(state);
        }

        public void MarkUserAsLoggedOut()
        {
            var anonymousUser = new ClaimsPrincipal(new ClaimsIdentity());
            _claimsPrincipal = anonymousUser;
            NotifyAuthenticationStateChanged(GetAuthenticationStateAsync());
        }
    }
}
