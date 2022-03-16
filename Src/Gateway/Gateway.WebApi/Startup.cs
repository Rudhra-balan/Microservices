using System.Text;
using Common.Lib.JwtTokenHandler;
using Gateway.WebApi.Model;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using MMLib.Ocelot.Provider.AppConfiguration;
using Ocelot.DependencyInjection;
using Ocelot.Middleware;

namespace Gateway.WebApi
{
    public class Startup
    {
        public Startup(IConfiguration config)
        {
            Configuration = config;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc(options => options.EnableEndpointRouting = false);
            services.AddOcelot().AddAppConfiguration();
            services.AddSwaggerForOcelot(Configuration);
            services.AddJwtTokenAuthentication(Configuration);

        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.UseDeveloperExceptionPage();
            app.UsePathBase("/gateway");
            app.UseHttpsRedirection();
            app.UseAuthentication();
            app.UseStaticFiles();
            app.UseMvc();
            app.UseSwaggerForOcelotUI(Configuration,
                    opt =>
                    {
                        opt.DownstreamSwaggerEndPointBasePath = "/gateway/swagger/docs";
                        opt.PathToSwaggerGenerator = "/swagger/docs";
                        opt.DefaultModelsExpandDepth(-1);
                    })
                .UseOcelot()
                .Wait();
           
        }
    }
}
