using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Blazored.LocalStorage;

namespace OpenDokoBlazor.Client.Services.Auth
{
    public class FusionTokenService
    {
        private readonly ISyncLocalStorageService _localStorageService;

        public FusionTokenService(ISyncLocalStorageService localStorageService)
        {
            _localStorageService = localStorageService;
        }

        public string GetToken()
        {
            if (_localStorageService.ContainKey(AuthService.StorageKey))
            {
                var token = _localStorageService.GetItem<AuthStorage>(AuthService.StorageKey);

                return token.AccessToken;
            }

            return string.Empty;
        }
    }
}
