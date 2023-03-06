using Microsoft.EntityFrameworkCore;
using MVC_app_main;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddDbContext<AstroDBContext>(options => options.UseSqlServer("Server=(localdb)\\MSSQLLocalDB;Database=AstroDB;Trusted_Connection=True;"));
builder.Services.AddControllersWithViews();
var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseAuthorization();
app.MapControllers();
app.Run();