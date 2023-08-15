using Microsoft.EntityFrameworkCore;
using MongoDB.Driver;

namespace MVC_app_main
{
    public class Startup
    {
        public IConfiguration Configuration { get; }

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public void Configure(IApplicationBuilder app)
        {
            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseRouting();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
                endpoints.MapControllerRoute(name: "default", pattern: "{controller=Home}/{action=Index}/{sortBy?}/{sortOrder?}");
            });
        }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddSingleton<IMongoClient>(new MongoClient(Configuration.GetConnectionString("Main")));
            services.AddMvc();
        }
    }
}
