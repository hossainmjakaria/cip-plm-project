using PLM.Library.Services;
using PLM.Library.Utility;
using PLM.Web.Configurations;

var builder = WebApplication.CreateBuilder(args);


// Add services to the container.
var appSettings = builder.Configuration.GetSection("AppSettings").Get<AppSettings>();
builder.Services.AddSingleton(appSettings!);

builder.Services.ResolveDependencies();

// Add services to the container.
builder.Services.AddControllersWithViews();

var app = builder.Build();

// Seed parking lot data
using (var scope = app.Services.CreateScope())
{
    var seeder = scope.ServiceProvider.GetRequiredService<IParkingSeederService>();
    await seeder.SeedAsync();
}

// Configure the HTTP request pipeline.


if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");


app.Run();
