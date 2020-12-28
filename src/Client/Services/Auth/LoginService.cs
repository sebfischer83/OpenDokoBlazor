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
    public interface ILoginService
    {
        Task<bool> Login(string email, string password, bool remember);
        bool Logout();
    }

    public class LoginService : ILoginService
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly ILocalStorageService _localStorageService;
        private readonly ISessionStorageService _sessionStorageService;
        private readonly AuthenticationStateProvider _authenticationStateProvider;
        private HttpClient _httpClient;

        public LoginService(IServiceProvider serviceProvider,
            ILocalStorageService localStorageService, ISessionStorageService sessionStorageService, AuthenticationStateProvider authenticationStateProvider)
        {
            _serviceProvider = serviceProvider;
            _localStorageService = localStorageService;
            _sessionStorageService = sessionStorageService;
            _authenticationStateProvider = authenticationStateProvider;
            _httpClient = (HttpClient)serviceProvider.GetService<HttpClient>();
        }

        public async Task<bool> Login(string email, string password, bool remember)
        {
            List<KeyValuePair<string, string>> list = new();
            list.Add(new KeyValuePair<string, string>("grant_type", "password"));
            list.Add(new KeyValuePair<string, string>("username", email));
            list.Add(new KeyValuePair<string, string>("password", password));
            var result = await _httpClient.PostAsync("connect/token", new FormUrlEncodedContent(list));
            if (result.IsSuccessStatusCode)
            {
                await using var sr = await result.Content.ReadAsStreamAsync();
                var parsedResult = await JsonSerializer.DeserializeAsync<SignInResult>(sr);
                Console.WriteLine(parsedResult.AccessToken);
            }

            return false;
        }

        public bool Logout()
        {
            throw new NotImplementedException();
        }
    }

    public class SignInResult
    {
        [JsonPropertyName("access_token")]
        public string AccessToken { get; set; }
        [JsonPropertyName("token_type")]
        public string TokenType { get; set; }
        [JsonPropertyName("expires_in")]
        public int ExpiresIn { get; set; }
    }
}
