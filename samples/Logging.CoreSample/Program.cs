using Logging.CoreSample;
using Microsoft.AspNetCore.Mvc;

var builder = WebApplication.CreateBuilder(args);
builder.Logging.ClearProviders();

//Use custom logWriter
builder.Logging.AddImplementation<CustomLogWriter>();

var app = builder.Build();

app.MapGet("/", ([FromServices] ILogger<Program> logger) =>
{
    logger.LogError("Logging from CustomLogWriter");
});

app.Run();
