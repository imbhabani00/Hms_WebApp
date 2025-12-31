using Hms.WebApp.Extensions;
using Hms.WebApp.Helper;
using Hms.WebApp.Middleware;
using Microsoft.AspNetCore.Authentication.Cookies;
using Serilog;
using Serilog.Events;
using Serilog.Sinks.MSSqlServer;

var builder = WebApplication.CreateBuilder(args);


// Configure Serilog - wrap in try-catch to prevent crashes
try
{
    var connectionString = builder.Configuration.GetConnectionString("HmsConnectionString");

    var columnOptions = new ColumnOptions();
    columnOptions.Store.Remove(StandardColumn.Properties);
    columnOptions.Store.Add(StandardColumn.LogEvent);
    columnOptions.TimeStamp.ConvertToUtc = true;

    Log.Logger = new LoggerConfiguration()
        .ReadFrom.Configuration(builder.Configuration)
        .WriteTo.Console()
        .WriteTo.MSSqlServer(
            connectionString: connectionString,
            sinkOptions: new MSSqlServerSinkOptions
            {
                TableName = "tbl_Logs",
                SchemaName = "dbo",
                AutoCreateSqlTable = true
            },
            restrictedToMinimumLevel: LogEventLevel.Error,
            columnOptions: columnOptions
        )
        .MinimumLevel.Information()
        .Enrich.FromLogContext()
        .CreateLogger();

    Log.Information("Starting HMS Web Application");
}
catch (Exception ex)
{
    Console.WriteLine($"Serilog configuration failed: {ex.Message}");
    // Create a basic logger as fallback
    Log.Logger = new LoggerConfiguration()
        .WriteTo.Console()
        .CreateLogger();
}

// Add services to the container
builder.Services.AddControllersWithViews();
builder.Services.AddHttpContextAccessor();  // Required for CookieHelper
builder.Services.AddHttpClient();           // Required for API calls

// Add Session - MUST be configured before building the app
builder.Services.AddSession(options =>
{
    var cookieExpirationMinutes = builder.Configuration.GetValue<int>("CookieExpiration", 480);
    options.IdleTimeout = TimeSpan.FromMinutes(cookieExpirationMinutes);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
    options.Cookie.SameSite = SameSiteMode.Lax;
});

// Add Authentication
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/Account/Login";
        options.LogoutPath = "/Account/Logout";
        options.AccessDeniedPath = "/Account/AccessDenied";
        options.ExpireTimeSpan = TimeSpan.FromMinutes(60);
        options.SlidingExpiration = true;
    });

// Register application services
builder.Services.AddWebAppServices();

// Global configuration assignment
UrlEncryptionExtension.Configuration = builder.Configuration;

var app = builder.Build();

// Configure the HTTP request pipeline
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();  // Shows detailed errors
}
else
{
    app.UseExceptionHandler("/Account/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();

// Enable hot reload for CSS, HTML, and Razor pages
if (app.Environment.IsDevelopment())
{
    app.UseStaticFiles(new StaticFileOptions()
    {
        OnPrepareResponse = ctx =>
        {
            ctx.Context.Response.Headers.Add("Cache-Control", "public, max-age=0");
        }
    });
}
else
{
    app.UseStaticFiles();
}

app.UseRouting();

// CRITICAL: Session MUST come BEFORE Authentication
app.UseSession();

// Custom JWT Middleware (after Session, before Authentication)
app.UseMiddleware<JwtMiddleware>();
app.UseMiddleware<RefreshTokenMiddleware>();

// Authentication & Authorization
app.UseAuthentication();
app.UseAuthorization();

// Map controller routes
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Account}/{action=Login}/{id?}");

try
{
    app.Run();
}
catch (Exception ex)
{
    Log.Fatal(ex, "Application terminated unexpectedly");
    Console.WriteLine($"Fatal error: {ex.Message}");
    throw;
}
finally
{
    Log.CloseAndFlush();
}