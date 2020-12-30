using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components.Authorization;
using OpenDokoBlazor.Client.Helper;

namespace OpenDokoBlazor.Client.Services.Auth
{
    public class AuthProvider : AuthenticationStateProvider
    {
        private ClaimsPrincipal _claimsPrincipal = new ClaimsPrincipal(new ClaimsIdentity());
        public override async Task<AuthenticationState> GetAuthenticationStateAsync()
        {
            await Task.FromResult(0);
            return new AuthenticationState(_claimsPrincipal);
        }

        public void MarkUserAsLoggedIn(string userName, string password, string token)
        {
            var identity = new ClaimsIdentity(new[] { new Claim(ClaimTypes.Name, userName), new Claim(ClaimTypes.Email, userName) }, "opendoko");
            _claimsPrincipal = new ClaimsPrincipal(identity);
            NotifyAuthenticationStateChanged(GetAuthenticationStateAsync());
        }

        public void MarkUserAsLoggedOut()
        {
            var anonymousUser = new ClaimsPrincipal(new ClaimsIdentity());
            _claimsPrincipal = anonymousUser;
            NotifyAuthenticationStateChanged(GetAuthenticationStateAsync());
        }
    }
}
