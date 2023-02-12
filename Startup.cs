namespace MVC_app_main
{
    public class Startup
    {
        public void Configure(IApplicationBuilder app)
        {
            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseRouting();
        }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc();
        }
    }
}
