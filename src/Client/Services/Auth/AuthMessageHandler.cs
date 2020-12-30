using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.WebAssembly.Authentication;

namespace OpenDokoBlazor.Client.Services.Auth
{
    public class AuthMessageHandler : DelegatingHandler
    {
        private readonly NavigationManager _navigationManager;
        private readonly IAuthService _authService;
        private Uri[] _authorizedUris;

        public AuthMessageHandler(NavigationManager navigationManager, IAuthService authService)
        {
            _navigationManager = navigationManager;
            _authService = authService;
            ConfigureHandler(new[] { navigationManager.BaseUri });
        }

        public void ConfigureHandler(
            IEnumerable<string> authorizedUrls,
            IEnumerable<string> scopes = null,
            string returnUrl = null)
        {
            if (_authorizedUris != null)
            {
                throw new InvalidOperationException("Handler already configured.");
            }

            if (authorizedUrls == null)
            {
                throw new ArgumentNullException(nameof(authorizedUrls));
            }

            var uris = authorizedUrls.Select(uri => new Uri(uri, UriKind.Absolute)).ToArray();
            if (uris.Length == 0)
            {
                throw new ArgumentException("At least one URL must be configured.", nameof(authorizedUrls));
            }

            _authorizedUris = uris;
        }

        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            var now = DateTimeOffset.Now;
            if (_authorizedUris == null)
            {
                throw new InvalidOperationException($"The '{nameof(AuthorizationMessageHandler)}' is not configured. " +
                                                    $"Call '{nameof(AuthorizationMessageHandler.ConfigureHandler)}' and provide a list of endpoint urls to attach the token to.");
            }

            string? token = "";
            if (_authorizedUris.Any(uri => uri.IsBaseOf(request.RequestUri!)))
            {
                token = await _authService.GetToken();
                if (token == null)
                    throw new AccessTokenNotAvailableException(_navigationManager, new AccessTokenResult(AccessTokenResultStatus.RequiresRedirect, new AccessToken(), "/login"), null);
                
            }
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
            
            return await base.SendAsync(request, cancellationToken);
        }
    }
}
