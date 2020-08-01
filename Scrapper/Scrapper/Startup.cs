using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Scrapper.Options;
using Scrapper.Scrappers;

namespace Scrapper
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers();
            services.AddHttpClient();
            services.Configure<SearchEngineConfig>(nameof(Google), Configuration.GetSection("SearchEngines:Google"));
            services.Configure<SearchEngineConfig>(nameof(Bing), Configuration.GetSection("SearchEngines:Bing"));

            services.AddScoped<ScrapperFactory>();
            services.AddScoped<Google>()
                .AddScoped<IScrapper, Google>(s => s.GetService<Google>());
            services.AddScoped<Bing>()
                        .AddScoped<IScrapper, Bing>(s => s.GetService<Bing>());
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
