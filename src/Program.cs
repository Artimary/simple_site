using Infrastructure.Data;
using Infrastructure.Repositories;
using Domain.Interfaces;
using Application.Services;
using Application.Interfaces;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Logging.AddConsole();
builder.Logging.AddDebug();

builder.Services.AddControllersWithViews();

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"))
          .EnableSensitiveDataLogging()
          .LogTo(Console.WriteLine, LogLevel.Information));

builder.Services.AddDbContextFactory<AppDbContext>(options => 
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("DefaultConnection"),
        sqlOptions => 
        {
            sqlOptions.EnableRetryOnFailure(
                maxRetryCount: 5, 
                maxRetryDelay: TimeSpan.FromSeconds(30), 
                errorNumbersToAdd: null
            );
            sqlOptions.UseQuerySplittingBehavior(QuerySplittingBehavior.SplitQuery);
        }
    ), 
    ServiceLifetime.Scoped);

builder.Services.AddScoped<IMouseRepository, MouseRepository>();
builder.Services.AddScoped<IMouseService, MouseService>();

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    var logger = services.GetRequiredService<ILogger<Program>>();
    
    try
    {
        var context = services.GetRequiredService<AppDbContext>();
        await context.Database.CanConnectAsync();
        logger.LogInformation("Database connection established");
    }
    catch (Exception ex)
    {
        logger.LogError(ex, "Database connection failed");
        throw;
    }
}

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseStaticFiles();
app.UseRouting();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
