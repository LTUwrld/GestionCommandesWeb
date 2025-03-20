using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using GestionCommandesWeb.Data;
var builder = WebApplication.CreateBuilder(args);
builder.Services.AddDbContext<GestionCommandesWebContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("GestionCommandesWebContext") ?? throw new InvalidOperationException("Connection string 'GestionCommandesWebContext' not found.")));

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddSession();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseRouting();

app.UseSession();

app.UseAuthorization();

app.MapStaticAssets();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();


app.Run();
