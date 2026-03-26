using CrmWebApi.Extensions;
using CrmWebApi.Middleware;
using Scalar.AspNetCore;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

builder.Host.UseSerilog((context, config) =>
{
    config
        .WriteTo.Console(
            theme: Serilog.Sinks.SystemConsole.Themes.AnsiConsoleTheme.Code,
            outputTemplate: "[{Timestamp:HH:mm:ss} {Level:u3}] {Message:lj}{NewLine}{Exception}")
        .MinimumLevel.Information();

    if (!context.HostingEnvironment.IsProduction())
        config.WriteTo.Debug();
});

builder.Services.AddMemoryCache();
builder.Services.AddControllers();
builder.Services.AddServices();
builder.Services.AddOpenApi();
builder.Services.AddDatabase(builder.Configuration);
builder.Services.AddRepositories();
builder.Services.AddJwt(builder.Configuration);
builder.Services.AddAuthorization();
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend", policy =>
        policy.AllowAnyOrigin()
              .AllowAnyHeader()
              .AllowAnyMethod());
});

var app = builder.Build();

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
