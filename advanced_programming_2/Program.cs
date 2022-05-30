using advanced_programming_2.Hubs;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Repository;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddDbContext<advanced_programming_2Context>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("advanced_programming_2Context") ?? throw new InvalidOperationException("Connection string 'advanced_programming_2Context' not found.")));

// Add services to the container.
builder.Services.AddControllersWithViews();

builder.Services.AddSignalR();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
}
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=ratings}/{action=Search}/{id?}");

app.UseEndpoints(endpoints =>
{
    endpoints.MapHub<myHub>("/myHub");
});
app.Run();
