using AspNetCoreMiddleware.Extentions;
using AspNetCoreMiddleware.Services;
using AspNetCoreMiddleware.Settings;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;

namespace AirTaskerRateLimiter
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers();

            // Load configuration from appsettings.json
            services.AddOptions();

            // Load General settings for RateLimiting
            services.Configure<ApiKeyLimitSettings>(Configuration.GetSection("RateLimiter:General"));

            // Load Client Specific settings for RateLimiting
            services.Configure<ClientLimitSettings>(Configuration.GetSection("RateLimiter"));

            // Storage for RateLimiter rules and current counters
            services.AddMemoryCache();

            // Add general middleware that is mandate for all the clients
            services.AddSingleton<ApiKeyRateLimittingService>();

            // Add client specific middleware to check /path for specific client and his limits
            services.AddSingleton<ClientLimittingService>();

            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo
                {
                    Title = "AirTasker API",
                    Version = "v1"
                });

                c.OperationFilter<DefaultHeaderFilter>();
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseSwagger();

            // Enable middleware to serve swagger-ui (HTML, JS, CSS, etc.),
            // specifying the Swagger JSON endpoint.
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "AirTasker API V1");

                // To serve SwaggerUI at application's root page, set the RoutePrefix property to an empty string.
                c.RoutePrefix = string.Empty;
            });

            // Inject middleware to get requests and check rate limiter rules
            app.UseClientRateLimiting();

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}