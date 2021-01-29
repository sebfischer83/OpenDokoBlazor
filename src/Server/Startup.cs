using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.ResponseCompression;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using HealthChecks.UI.Client;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.OpenApi.Models;
using OpenDokoBlazor.Server.Classes;
using OpenDokoBlazor.Server.Data;
using OpenDokoBlazor.Server.Data.Models;
using OpenDokoBlazor.Server.Services;
using OpenIddict.Abstractions;
using Serilog;
using Serilog.AspNetCore;
using Stl.DependencyInjection;
using Stl.Fusion;
using Stl.Fusion.Authentication;
using Stl.Fusion.Blazor;
using Stl.Fusion.Client;
using Stl.Fusion.EntityFramework;
using Stl.Fusion.Server;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace OpenDokoBlazor.Server
{
    public class Startup
    {
        public IWebHostEnvironment Env;

        public Startup(IConfiguration configuration, IWebHostEnvironment environment)
        {
            Env = environment;
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            services.Configure<RequestLoggingOptions>(o =>
            {
                o.EnrichDiagnosticContext = (diagnosticContext, httpContext) =>
                {
                    diagnosticContext.Set("RemoteIpAddress", httpContext?.Connection?.RemoteIpAddress?.MapToIPv4());
                };
            });
            services.AddLogging(logging => {
                if (Env.IsDevelopment())
                    logging.AddFilter("Microsoft.EntityFrameworkCore.Database.Command", LogLevel.Information);
            });

            services.AddDbContext<OpenDokoContext>((provider, builder) =>
            {
                var configuration = provider.GetService<IConfiguration>();
                builder.UseSqlServer(configuration.GetConnectionString("connectionString"));
                builder.UseOpenIddict();
            });

            services.AddDbContext<InMemoryContext>((provider, builder) =>
            {
                var configuration = provider.GetService<IConfiguration>();
                builder.UseInMemoryDatabase("inmemorydb");
            });
            
            services.AddSingleton(new PresenceService.Options() { UpdatePeriod = TimeSpan.FromMinutes(1) });
            services.AddFusion();
            var fusion = services.AddFusion();
            var fusionServer = fusion.AddWebSocketServer();
            var fusionClient = fusion.AddRestEaseClient();
            var fusionAuth = fusion.AddAuthentication().AddServer();
            services.AttributeScanner().AddServicesFrom(Assembly.GetExecutingAssembly());

            services.AttributeScanner()
                .AddServicesFrom(typeof(TimeService).Assembly)
                .AddServicesFrom(Assembly.GetExecutingAssembly());

            AddAuth(services);
            services.AddHealthChecksUI().AddInMemoryStorage();
            services.AddHealthChecks().AddSqlServer(Configuration.GetConnectionString("connectionString"), "SELECT 1",
                "sql", HealthStatus.Unhealthy, new string[] { "db", "sql", "sqlserver" });
            services.AddMemoryCache();
            services.AddHttpContextAccessor();
            services.AddMvc(options => options.EnableEndpointRouting = false);
            services.AddAntiforgery();
            services.AddControllersWithViews();
            services.AddRazorPages();
            services.Configure<OpenDokoOptions>(Configuration.GetSection(OpenDokoOptions.Identifier));
            services.AddSingleton<TableManager>();
            fusionAuth.AddBlazor(o => { });
            services.AddHostedService<TableManagerStartupService>();
            AddSwagger(services);
        }

        private void AddAuth(IServiceCollection services)
        {
            services.AddIdentity<OpenDokoUser, IdentityRole>()
                .AddEntityFrameworkStores<OpenDokoContext>()
                .AddDefaultTokenProviders();

            services.ConfigureApplicationCookie(options =>
            {
                options.Events.OnRedirectToAccessDenied = context =>
                {
                    context.Response.StatusCode = 401;
                    return Task.CompletedTask;
                };
                options.Events.OnRedirectToLogin = context =>
                {
                    context.Response.StatusCode = 401;
                    return Task.CompletedTask;
                };
            });

            services.Configure<IdentityOptions>(options =>
            {
                options.ClaimsIdentity.UserNameClaimType = OpenIddictConstants.Claims.Name;
                options.ClaimsIdentity.UserIdClaimType = OpenIddictConstants.Claims.Subject;
                options.ClaimsIdentity.RoleClaimType = OpenIddictConstants.Claims.Role;
            });
            services.AddAuthentication();

            services.AddOpenIddict(builder =>
            {
                builder.AddCore(coreBuilder => coreBuilder.UseEntityFrameworkCore().UseDbContext<OpenDokoContext>());
                builder.AddServer(options =>
                {
                    // Enable the token endpoint.
                    options.SetTokenEndpointUris("/connect/token");
                    options.SetUserinfoEndpointUris("/Account/UserInfo");

                    // Enable the password flow.
                    options.AllowPasswordFlow().AllowRefreshTokenFlow();
                    options.AcceptAnonymousClients();

                    // Register the signing and encryption credentials.
                    options.AddDevelopmentEncryptionCertificate()
                        .AddDevelopmentSigningCertificate();
                    options.RegisterScopes(OpenIddictConstants.Scopes.Email, OpenIddictConstants.Scopes.Roles, OpenIddictConstants.Scopes.Profile);

                    // Register the ASP.NET Core host and configure the ASP.NET Core-specific options.
                    options.UseAspNetCore()
                        .EnableAuthorizationEndpointPassthrough()
                        .EnableLogoutEndpointPassthrough()
                        .EnableStatusCodePagesIntegration()
                        .EnableTokenEndpointPassthrough()
                        .DisableTransportSecurityRequirement(); // During development, you can disable the HTTPS requirement.
                });
                builder.AddValidation(options =>
                {
                    // Import the configuration from the local OpenIddict server instance.
                    options.UseLocalServer();
                    
                    // Register the ASP.NET Core host.
                    options.UseAspNetCore();
                });
            });

        }

        private static void AddSwagger(IServiceCollection services)
        {
            services.AddApiVersioning(options =>
            {
                // Specify the default API Version as 1.0
                options.DefaultApiVersion = new ApiVersion(1, 0);
                // If the client hasn't specified the API version in the request, use the default API version number 
                options.AssumeDefaultVersionWhenUnspecified = true;
                options.ReportApiVersions = true;
            });
            services.AddTransient<IConfigureOptions<SwaggerGenOptions>, ConfigureSwaggerOptions>();
            services.AddVersionedApiExplorer(
                options =>
                {
                    // add the versioned api explorer, which also adds IApiVersionDescriptionProvider service
                    // note: the specified format code will format the version as "'v'major[.minor][-status]"
                    options.GroupNameFormat = "'v'VVV";

                    // note: this option is only necessary when versioning by url segment. the SubstitutionFormat
                    // can also be used to control the format of the API version in route templates
                    options.SubstituteApiVersionInUrl = true;
                });
            services.AddSwaggerGen(
                options =>
                {
                    var apiinfo = new OpenApiInfo
                    {
                        Title = "theta-CandidateAPI",
                        Version = "v1",
                        Description = "Candidate API for thetalentbot",
                        Contact = new OpenApiContact
                        { Name = "thetalentbot", Url = new Uri("https://thetalentbot.com/developers/contact") },
                        License = new OpenApiLicense()
                        {
                            Name = "Commercial",
                            Url = new Uri("https://thetalentbot.com/developers/license")
                        }
                    };

                    OpenApiSecurityScheme securityDefinition = new OpenApiSecurityScheme()
                    {
                        Name = "Bearer",
                        BearerFormat = "JWT",
                        Scheme = "bearer",
                        Description = "Specify the authorization token.",
                        In = ParameterLocation.Header,
                        Type = SecuritySchemeType.Http,
                    };
                    OpenApiSecurityRequirement securityRequirements = new OpenApiSecurityRequirement()
                    {
                        //{securityScheme, new string[] { }},
                    };

                    options.AddSecurityDefinition("jwt_auth", securityDefinition);
                    // Make sure swagger UI requires a Bearer token to be specified
                    options.AddSecurityRequirement(securityRequirements);
                });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, IApiVersionDescriptionProvider provider)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseWebAssemblyDebugging();
            }
            else
            {
                app.UseExceptionHandler("/Error");
                app.UseHsts();
            }

            //app.UseSerilogRequestLogging();
            app.UseHttpsRedirection();

            app.UseWebSockets(new WebSocketOptions()
            {
                KeepAliveInterval = TimeSpan.FromSeconds(30),
            });
            app.UseFusionSession();

            app.UseBlazorFrameworkFiles();
            app.UseStaticFiles();
            app.UseSwagger();
            app.UseSwaggerUI(
                options =>
                {
                    // build a swagger endpoint for each discovered API version
                    foreach (var description in provider.ApiVersionDescriptions)
                    {
                        options.SwaggerEndpoint($"/swagger/{description.GroupName}/swagger.json", description.GroupName.ToUpperInvariant());
                    }
                });
            app.UseRouting();
            app.UseAuthentication();
            app.UseAuthorization();
            app.UseMvcWithDefaultRoute();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapHealthChecks("healthz", new HealthCheckOptions()
                {
                    Predicate = _ => true,
                    ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
                });
                endpoints.MapHealthChecksUI();
                endpoints.MapFusionWebSocketServer();
                endpoints.MapRazorPages();
                endpoints.MapControllers();
                endpoints.MapFallbackToFile("index.html");
            });
        }
    }

    public class ConfigureSwaggerOptions : IConfigureOptions<SwaggerGenOptions>
    {
        readonly IApiVersionDescriptionProvider provider;

        public ConfigureSwaggerOptions(IApiVersionDescriptionProvider provider) =>
            this.provider = provider;

        public void Configure(SwaggerGenOptions options)
        {
            foreach (var description in provider.ApiVersionDescriptions)
            {
                options.SwaggerDoc(
                    description.GroupName,
                    new Info()
                    {
                        Title = $"Sample API {description.ApiVersion}",
                        Version = description.ApiVersion.ToString(),
                    });
            }
        }
    }

    public class Info : OpenApiInfo
    {
    }
}
