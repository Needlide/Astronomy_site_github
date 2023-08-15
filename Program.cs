using MongoDB.Driver;
using MVC_app_main;

var builder = WebApplication.CreateBuilder(args);

string? connectionString = builder.Configuration.GetConnectionString("Main");

if (string.IsNullOrEmpty(connectionString))
{
    throw new InvalidOperationException("Missing MongoDB connection string in the configuration.");
}

builder.Services.AddSingleton<IMongoClient>(new MongoClient(connectionString));

builder.Services.AddSingleton(sp =>
{
    var mongoClient = sp.GetRequiredService<IMongoClient>();
    return new AstroDBContext(mongoClient);
});

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