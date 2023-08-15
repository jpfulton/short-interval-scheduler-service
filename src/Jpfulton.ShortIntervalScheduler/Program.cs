using Jpfulton.ShortIntervalScheduler;
using Microsoft.Extensions.Logging.Configuration;
using Microsoft.Extensions.Logging.EventLog;

HostApplicationBuilder builder = Host.CreateApplicationBuilder(args);
builder.Services.AddWindowsService();

LoggerProviderOptions.RegisterProviderOptions<
    EventLogSettings, EventLogLoggerProvider>(builder.Services);

ILogger logger = builder.Services.BuildServiceProvider().GetRequiredService<ILogger<Program>>();
Options? workerOptions = null;
try
{
    workerOptions = Options.GetOptionsFromArgs(args);
}
catch (Exception e)
{
    logger.LogError(e, "Unable to build service options. Shutting down...");
    Environment.Exit(1);
}

builder.Services.AddSingleton(workerOptions);
builder.Services.AddHostedService<Worker>();

// See: https://github.com/dotnet/runtime/issues/47303
builder.Logging.AddConfiguration(
    builder.Configuration.GetSection("Logging"));

logger.LogInformation("Configuring service...");
IHost host = builder.Build();

logger.LogInformation("Starting service loop...");
host.Run();
