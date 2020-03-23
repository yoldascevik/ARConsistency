using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ARConsistency;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace TestApi
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
            services.AddControllers().AddApiResponseConsistency(x =>
                    {
                        var config = new ResponseOptions();
                        Configuration.GetSection("ApiConsistency").Bind(config);

                        x.ApiVersion = config.ApiVersion;
                        x.ShowApiVersion = config.ShowApiVersion;
                        x.EnableExceptionLogging = config.EnableExceptionLogging;
                        x.IgnoreNullValue = config.IgnoreNullValue;
                        x.IsDebug = config.IsDebug;
                        x.ShowStatusCode = config.ShowStatusCode;
                        x.UseCamelCaseNaming = config.UseCamelCaseNaming;
                    });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseApiResponseConsistency();
            app.UseRouting();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
