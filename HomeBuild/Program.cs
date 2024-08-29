using HomeBuild.Data;
using HomeBuild.Models;
using HomeBuild.Repositories;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllersWithViews();
builder.Services.AddDbContext<ApplicationDbContext>(options =>
options.UseSqlServer(
builder.Configuration.GetConnectionString("DefaultConnection"),
o => {
	o.EnableRetryOnFailure();
	o.UseQuerySplittingBehavior(QuerySplittingBehavior.SplitQuery);
	o.UseRelationalNulls();
	o.CommandTimeout(60);
	o.MaxBatchSize(1000);
	o.EnableRetryOnFailure(5, TimeSpan.FromSeconds(10), null);
}));
builder.Services.AddIdentity <ApplicationUser, IdentityRole>()
.AddEntityFrameworkStores<ApplicationDbContext>();

builder.Services.AddScoped<StocksRepository>();
builder.Services.AddScoped<ApplicationDbContext>();
builder.Services.AddScoped<ShoppingCartRepository>();

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
	app.UseExceptionHandler("/Home/Error");
	app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseAuthentication();
app.UseRouting();
app.UseAuthorization();

app.MapControllerRoute(
name: "default",
pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
