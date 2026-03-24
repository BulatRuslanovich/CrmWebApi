using CrmWebApi.Data;
using CrmWebApi.Extensions;
using CrmWebApi.Middleware;
using Microsoft.EntityFrameworkCore;
using Scalar.AspNetCore;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

builder.Host.UseSerilog((context, config) =>
{
    config
        .WriteTo.Console(
            theme: Serilog.Sinks.SystemConsole.Themes.AnsiConsoleTheme.Code,
            outputTemplate: "[{Timestamp:HH:mm:ss} {Level:u3}] {Message:lj}{NewLine}{Exception}")
        .MinimumLevel.Information()
        .MinimumLevel.Override("CrmWebApi.Data", context.HostingEnvironment.IsProduction()
            ? Serilog.Events.LogEventLevel.Warning
            : Serilog.Events.LogEventLevel.Information)
        .MinimumLevel.Override("CrmWebApi.Middleware", Serilog.Events.LogEventLevel.Warning)
        .MinimumLevel.Override("Microsoft.AspNetCore", Serilog.Events.LogEventLevel.Warning)
        .MinimumLevel.Override("Microsoft.EntityFrameworkCore.Database.Command", context.HostingEnvironment.IsProduction()
            ? Serilog.Events.LogEventLevel.Warning
            : Serilog.Events.LogEventLevel.Information);

    if (!context.HostingEnvironment.IsProduction())
        config.WriteTo.Debug();
});

builder.Services.AddControllers();
builder.Services.AddServices();
builder.Services.AddOpenApi();
builder.Services.AddDatabase(builder.Configuration);
builder.Services.AddRepositories();
builder.Services.AddJwt(builder.Configuration);
builder.Services.AddAuthorization();
var corsOrigins = builder.Configuration["Cors:AllowedOrigins"]
    ?.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
    ?? ["http://localhost:8081"];

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend", policy =>
        policy.WithOrigins(corsOrigins)
              .AllowAnyHeader()
              .AllowAnyMethod()
              .AllowCredentials());
});

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    db.Database.Migrate();
}

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference(options =>
    {
        options.Title = "CRM Web Api";
        options.AddPreferredSecuritySchemes("Bearer")
               .AddHttpAuthentication("Bearer", b => b.Token = string.Empty);
    });
}

app.UseMiddleware<ExceptionMiddleware>();
app.UseCors("AllowFrontend");
if (!app.Environment.IsDevelopment())
    app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

app.Run();
