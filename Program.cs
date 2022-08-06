using Server;

var builder = WebApplication.CreateBuilder(args);

var httpPort = 5000;
var httpsPort = 5001;

if (args is not null && args.Length > 0 && int.TryParse(args[0], out int port))
{
    httpPort = port;
    httpsPort = port + 1;

    builder.WebHost.ConfigureKestrel(options =>
    {
        options.ListenAnyIP(httpPort);
        options.ListenAnyIP(httpsPort, configure => configure.UseHttps());
    });
}

builder.Host.ConfigureLogging(logging =>
{
    logging.ClearProviders();
    logging.SetMinimumLevel(LogLevel.Information);
    logging.AddFilter("System", LogLevel.Error);
    logging.AddFilter("Microsoft", LogLevel.Error);
    logging.AddFilter("Microsoft.AspNetCore", LogLevel.Error);
    logging.AddFilter("Microsoft.Hosting.Lifetime", LogLevel.Error);
    logging.AddSimpleConsole(options =>
        {
            options.IncludeScopes = false;
            options.SingleLine = false;
            options.TimestampFormat = "hh:mm:ss ";
        });
});

var app = builder.Build();

app.UseHttpsRedirection();

app.Use((context, next) =>
{
    context.Request.EnableBuffering();
    return next();
});

app.UseMiddleware<LogMiddleware>();

var emptyFunction = () => { };

app.MapGet("/", emptyFunction);
app.MapPost("/", emptyFunction);
app.MapPut("/", emptyFunction);
app.MapDelete("/", emptyFunction);

Console.WriteLine($"Server listening on http://localhost:{httpPort} and https://localhost:{httpsPort} ...\n");

app.Run();