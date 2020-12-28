using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Blazored.LocalStorage;
using Blazored.SessionStorage;
using Blazorise;
using Blazorise.Bootstrap;
using Blazorise.Icons.FontAwesome;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.WebAssembly.Authentication;
using Microsoft.Extensions.Hosting;
using OpenDokoBlazor.Client.Auth;
using OpenDokoBlazor.Client.Services.Auth;
using Stl.DependencyInjection;
using Stl.Fusion;
using Stl.Fusion.Blazor;
using Stl.Fusion.Client;

namespace OpenDokoBlazor.Client
{
    public class Program
    {
        public const string ClientSideScope = nameof(ClientSideScope);
        public static Task Main(string[] args)
        {
            var builder = WebAssemblyHostBuilder.CreateDefault(args);
            ConfigureServices(builder.Services, builder);
            builder.Services
                .AddBlazorise(options =>
                {
                    options.ChangeTextOnKeyPress = true;
                })
                .AddBootstrapProviders()
                .AddFontAwesomeIcons();
            builder.RootComponents.Add<App>("#app");

            builder.Services.AddLocalization();
            builder.Services.AddScoped<AuthenticationStateProvider, AuthProvider>();
            builder.Services.AddScoped<ILoginService, LoginService>();
            builder.Services.AddBlazoredLocalStorage();
            builder.Services.AddBlazoredSessionStorage();
            builder.Services.AddAuthorizationCore();
            builder.Services.AddHttpClient("OpenDoko.ServerAPI")
                .ConfigureHttpClient(client => client.BaseAddress = new Uri(builder.HostEnvironment.BaseAddress));

            builder.Services.AddScoped(provider =>
            {
                var factory = provider.GetRequiredService<IHttpClientFactory>();
                return factory.CreateClient("OpenDoko.ServerAPI");
            });

            //builder.Services.AddOidcAuthentication(options =>
            //{
            //    options.ProviderOptions.ClientId = "balosar-blazor-client";
            //    options.ProviderOptions.Authority = "https://localhost:44310/";
            //    options.ProviderOptions.ResponseType = "code";

            //    // Note: response_mode=fragment is the best option for a SPA. Unfortunately, the Blazor WASM
            //    // authentication stack is impacted by a bug that prevents it from correctly extracting
            //    // authorization error responses (e.g error=access_denied responses) from the URL fragment.
            //    // For more information about this bug, visit https://github.com/dotnet/aspnetcore/issues/28344.
            //    //
            //    options.ProviderOptions.ResponseMode = "query";
            //    options.AuthenticationPaths.RemoteRegisterPath = "https://localhost:44310/Identity/Account/Register";
            //});

            var host = builder.Build();
            host.Services
                .UseBootstrapProviders()
                .UseFontAwesomeIcons();
            var runTask = host.RunAsync();
            Task.Run(async () => {
                // We "manually" start IHostedServices here, because Blazor host doesn't do this.
                var hostedServices = host.Services.GetRequiredService<IEnumerable<IHostedService>>();
                foreach (var hostedService in hostedServices)
                    await hostedService.StartAsync(default);
            });
            return runTask;
        }
        
        public static void ConfigureServices(IServiceCollection services, WebAssemblyHostBuilder builder)
        {
            builder.Logging.SetMinimumLevel(LogLevel.Warning);

            var baseUri = new Uri(builder.HostEnvironment.BaseAddress);
            var apiBaseUri = new Uri($"{baseUri}api/");

            var fusion = services.AddFusion();
            var fusionClient = fusion.AddRestEaseClient(
                (c, o) => {
                    o.BaseUri = baseUri;
                    o.MessageLogLevel = LogLevel.Information;
                }).ConfigureHttpClientFactory(
                (c, name, o) => {
                    var isFusionClient = (name ?? "").StartsWith("Stl.Fusion");
                    var clientBaseUri = isFusionClient ? baseUri : apiBaseUri;
                    o.HttpClientActions.Add(client => client.BaseAddress = clientBaseUri);
                });
            var fusionAuth = fusion.AddAuthentication().AddRestEaseClient().AddBlazor();
            
            // This method registers services marked with any of ServiceAttributeBase descendants, including:
            // [Service], [ComputeService], [RestEaseReplicaService], [LiveStateUpdater]
            services.AttributeScanner(ClientSideScope).AddServicesFrom(Assembly.GetExecutingAssembly());
            ConfigureSharedServices(services);
        }

        public static void ConfigureSharedServices(IServiceCollection services)
        {
            // Default delay for update delayers
            services.AddSingleton(c => new UpdateDelayer.Options()
            {
                Delay = TimeSpan.FromSeconds(0.1),
            });

            services.AttributeScanner().AddServicesFrom(Assembly.GetExecutingAssembly());
        }
    }
}
