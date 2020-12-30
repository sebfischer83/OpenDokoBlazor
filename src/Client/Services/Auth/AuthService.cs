#nullable enable
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using Blazored.LocalStorage;
using Blazored.SessionStorage;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.Extensions.DependencyInjection;
using Stl.Fusion;
using Stl.Fusion.Blazor;

namespace OpenDokoBlazor.Client.Services.Auth
{
    public interface IAuthService
    {
        Task<bool> Login(string email, string password, bool remember);
        bool Logout();

        Task<string?> GetToken();
    }

    public class AuthService : IAuthService
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly ILocalStorageService _localStorageService;
        private readonly ISessionStorageService _sessionStorageService;
        private readonly AuthenticationStateProvider _authenticationStateProvider;
        private readonly HttpClient _httpClient;
        private const string StorageKey = "token";
        
        public AuthService(IServiceProvider serviceProvider,
            ILocalStorageService localStorageService, ISessionStorageService sessionStorageService, AuthenticationStateProvider authenticationStateProvider,
            IHttpClientFactory httpClientFactory)
        {
            _serviceProvider = serviceProvider;
            _localStorageService = localStorageService;
            _sessionStorageService = sessionStorageService;
            _authenticationStateProvider = authenticationStateProvider;
            _httpClient = (HttpClient)httpClientFactory.CreateClient("OpenDoko.Authorize");
        }

        
        
        public async Task<bool> Login(string email, string password, bool remember)
        {
            List<KeyValuePair<string?, string?>> list = new();
            list.Add(new KeyValuePair<string?, string?>("grant_type", "password"));
            list.Add(new KeyValuePair<string?, string?>("username", email));
            list.Add(new KeyValuePair<string?, string?>("password", password));
            list.Add(new KeyValuePair<string?, string?>("scope", "openid offline_access email"));
            var result = await _httpClient.PostAsync("connect/token", new FormUrlEncodedContent(list));
            if (result.IsSuccessStatusCode)
            {
                await using var sr = await result.Content.ReadAsStreamAsync();
                var parsedResult = await JsonSerializer.DeserializeAsync<AuthResult>(sr);
                if (parsedResult == null)
                    return false;
                var token = new AuthStorage(parsedResult.RefreshToken,
                    DateTime.Now.AddSeconds(parsedResult.ExpiresIn),
                    parsedResult.AccessToken);
                await _localStorageService.SetItemAsync(StorageKey, JsonSerializer.Serialize(token));

                ((AuthProvider)_authenticationStateProvider).MarkUserAsLoggedIn(email, password, parsedResult.AccessToken);
                
                return true;
            }

            return false;
        }

        public bool Logout()
        {
            throw new NotImplementedException();
        }

        private async Task<string?> RefreshToken(AuthStorage authStorage)
        {
            List<KeyValuePair<string?, string?>> list = new();
            list.Add(new KeyValuePair<string?, string?>("grant_type", "refresh_token"));
            list.Add(new KeyValuePair<string?, string?>("refresh_token", authStorage.RefreshToken));
            var result = await _httpClient.PostAsync("connect/token", new FormUrlEncodedContent(list));
            if (result.IsSuccessStatusCode)
            {
                await using var sr = await result.Content.ReadAsStreamAsync();
                var parsedResult = await JsonSerializer.DeserializeAsync<AuthResult>(sr);
                if (parsedResult == null)
                    return null;
                var token = new AuthStorage(parsedResult.RefreshToken,
                    DateTime.Now.AddSeconds(parsedResult.ExpiresIn),
                    parsedResult.AccessToken);
                await _localStorageService.SetItemAsync(StorageKey, token);
                
                return token.AccessToken;
            }

            return null;
        }
        
        public async Task<string?> GetToken()
        {
            if (await _localStorageService.ContainKeyAsync(StorageKey))
            {
                var token = await _localStorageService.GetItemAsync<AuthStorage>(StorageKey);
                if (token.Expires <= DateTime.Now.AddMinutes(-5))
                {
                    return await RefreshToken(token);
                }

                return token.AccessToken;
            }

            return null;
        }
    }

    public class AuthStorage
    {
        public AuthStorage(string refreshToken, DateTime expires, string accessToken)
        {
            RefreshToken = refreshToken;
            Expires = expires;
            AccessToken = accessToken;
        }

        public string AccessToken { get; }

        public DateTime Expires { get; }
        
        public string RefreshToken { get; }
    }
    
    public class AuthResult
    {
        [JsonPropertyName("access_token")]
        public string AccessToken { get; set; }
        [JsonPropertyName("token_type")]
        public string TokenType { get; set; }
        [JsonPropertyName("expires_in")]
        public int ExpiresIn { get; set; }

        [JsonPropertyName("refresh_token")]
        public string RefreshToken { get; set; }
    }
}
