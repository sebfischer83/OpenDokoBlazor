using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Blazored.LocalStorage;
using Blazored.SessionStorage;
using Blazorise;
using Blazorise.Bootstrap;
using Blazorise.Icons.FontAwesome;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Components.WebAssembly.Authentication;
using Microsoft.Extensions.Hosting;
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
            builder.Services.AddBlazoredLocalStorage();
            builder.Services.AddBlazoredSessionStorage();
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
            builder.Services.AddScoped<IAuthService, AuthService>();

            builder.Services.AddAuthorizationCore();
            builder.Services.AddScoped<AuthMessageHandler>();
            builder.Services.AddHttpClient("OpenDoko.ServerAPI")
                .ConfigureHttpClient(client => client.BaseAddress = new Uri(builder.HostEnvironment.BaseAddress)).AddHttpMessageHandler<AuthMessageHandler>();

            builder.Services.AddHttpClient("OpenDoko.Authorize").ConfigureHttpClient(client =>
                client.BaseAddress = new Uri(builder.HostEnvironment.BaseAddress));


            var baseUri = new Uri(builder.HostEnvironment.BaseAddress);
            var apiBaseUri = new Uri($"{baseUri}api/");
            builder.Services.AddHttpClient("OpenDoko.FusionApi")
                .ConfigureHttpClient(client => client.BaseAddress = apiBaseUri).AddHttpMessageHandler<AuthMessageHandler>();

            builder.Services.AddScoped(provider =>
            {
                var factory = provider.GetRequiredService<IHttpClientFactory>();
                return factory.CreateClient("OpenDoko.ServerAPI");
            });

            var host = builder.Build();
            host.Services
                .UseBootstrapProviders()
                .UseFontAwesomeIcons();
            var runTask = host.RunAsync();
            Task.Run(async () =>
            {
                // We "manually" start IHostedServices here, because Blazor host doesn't do this.
                var hostedServices = host.Services.GetRequiredService<IEnumerable<IHostedService>>();
                foreach (var hostedService in hostedServices)
                    await hostedService.StartAsync(default);
            });
            return runTask;
        }

        public static void ConfigureServices(IServiceCollection services, WebAssemblyHostBuilder builder)
        {
            builder.Logging.SetMinimumLevel(LogLevel.Error);

            var baseUri = new Uri(builder.HostEnvironment.BaseAddress);
            var apiBaseUri = new Uri($"{baseUri}api/");
            services.AddScoped<FusionTokenService>();
            
            var fusion = services.AddFusion();
            var fusionClient = fusion.AddRestEaseClient(
                (c, o) =>
                {
                    o.BaseUri = baseUri;
                }).ConfigureHttpClientFactory(
                (c, name, o) =>
                {
                    var isFusionClient = (name ?? "").StartsWith("Stl.Fusion");
                    var clientBaseUri = isFusionClient ? baseUri : apiBaseUri;
                    o.HttpClientActions.Add(client =>
                    {
                        client.BaseAddress = clientBaseUri;
                        var serviceScopeFactory = c.GetService<IServiceScopeFactory>();
                        using (var scope = serviceScopeFactory.CreateScope())
                        {
                            var token = scope.ServiceProvider.GetService<FusionTokenService>();
                            client.DefaultRequestHeaders.Authorization =
                                new AuthenticationHeaderValue("Bearer", token.GetToken());
                        }
                    });
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
