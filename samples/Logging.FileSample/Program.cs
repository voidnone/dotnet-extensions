using Microsoft.AspNetCore.Mvc;

var builder = WebApplication.CreateBuilder(args);
builder.Logging.ClearProviders();
builder.WebHost.ConfigureKestrel(options => options.ListenAnyIP(8080));

//Use File logWriter
builder.Logging.AddFile(options =>
{
    options.Path = "custom_log_folder";
});

var app = builder.Build();

app.MapGet("/", ([FromServices] ILogger<Program> logger) =>
{
   logger.LogError($"Logging from file logging provider"); 
});

app.Run();
