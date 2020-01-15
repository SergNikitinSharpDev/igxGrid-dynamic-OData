using System.Linq;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using TodoApi.Models;
using Microsoft.AspNet.OData.Extensions;
using Microsoft.AspNet.OData.Builder;
using PostgreODataAPI.Routing;
using PostgreODataAPI.DynamicOData;

namespace TodoApi
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
            services.AddSingleton(Configuration);
            services.AddScoped<IDataService, PostgreDataService>();
            services.AddScoped<ISchemaReader, PostgreSchemaReader>();
            services.AddTransient(typeof(IEdmModelBuilder), typeof(PostgreEdmModelBuilder));
            // Add framework services.
            services.AddDbContext<Models.AppContext>(options =>
            options.UseNpgsql(Configuration.GetConnectionString("pbk")));
            services.AddMvc(o => o.EnableEndpointRouting = false).SetCompatibilityVersion(CompatibilityVersion.Version_2_2);
            services.AddCors(o => o.AddPolicy("MyPolicy", builder =>
            {
                builder.AllowAnyOrigin()
                       .AllowAnyMethod()
                       .AllowAnyHeader();
            }));
            services.AddOData();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseCors("MyPolicy");

            //ODataConventionModelBuilder builder = new ODataConventionModelBuilder(app.ApplicationServices);
            //builder.EntitySet<ProjectConstructionEntity>(nameof(ProjectConstructionEntity));
            app.UseMvc(routeBuilder =>
            {
                routeBuilder.EnableDependencyInjection();
                routeBuilder.MaxTop(100).Expand().Select().Filter().Count().OrderBy().SkipToken();
                //routeBuilder.MapODataServiceRoute("ODataRoute", "odt", builder.GetEdmModel());
                routeBuilder.CustomMapODataServiceRoute("msodata", "msodata/{dataSource}", Configuration);
            });
        }
    }
}
