using Microsoft.AspNetCore.Mvc;

var builder = WebApplication.CreateBuilder(args);
builder.Logging.ClearProviders();

//Use File logWriter
builder.Logging.AddFile(options =>
{
    options.Path = "custom_log_folder";
});

var app = builder.Build();

app.MapGet("/", ([FromServices] ILogger<Program> logger) =>
{
    logger.LogError("Logging from file logging perovider");
});

app.Run();
