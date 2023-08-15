using System.Diagnostics;

namespace Jpfulton.ShortIntervalScheduler;

public class Worker : BackgroundService
{
    private readonly ILogger<Worker> _logger;
    private readonly Options _options;

    public Worker(ILogger<Worker> logger, Options options)
    {
        _logger = logger;
        _options = options;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation($"Using delay between command runs: {_options.DelayInSeconds}");
        _logger.LogInformation($"Using command: '{_options.Command}'.");

        var cmdArray = _options.Command.Split(" ");
        var cmd = cmdArray[0];

        var cmdArgs = string.Empty;
        if (cmdArray.Length > 1)
        {
            // args exist
            var argList = cmdArray.ToList();
            argList.RemoveAt(0); // remove the command leaving args behind

            cmdArgs = string.Join("", argList);
        }

        try
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                using (var p = new Process
                {
                    StartInfo = new ProcessStartInfo
                    {
                        FileName = cmd,
                        Arguments = cmdArgs,
                        RedirectStandardInput = false,
                        RedirectStandardError = false,
                        UseShellExecute = true,
                        RedirectStandardOutput = false,
                        CreateNoWindow = false
                    }
                })
                {

                    // start external process and wait on exit
                    var processStarted = p.Start();
                    if (!processStarted) {
                        _logger.LogError($"Failed to start command: {cmd}");
                        throw new Exception("Failed to start command.");
                    }
                    await p.WaitForExitAsync(stoppingToken);
                }

                // delay between runs
                await Task.Delay(TimeSpan.FromSeconds(_options.DelayInSeconds), stoppingToken);
            }
        }
        catch (TaskCanceledException)
        {
            // When the stopping token is canceled, for example, a call made from services.msc,
            // we shouldn't exit with a non-zero exit code. In other words, this is expected...
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "{Message}", ex.Message);

            // Terminates this process and returns an exit code to the operating system.
            // This is required to avoid the 'BackgroundServiceExceptionBehavior', which
            // performs one of two scenarios:
            // 1. When set to "Ignore": will do nothing at all, errors cause zombie services.
            // 2. When set to "StopHost": will cleanly stop the host, and log errors.
            //
            // In order for the Windows Service Management system to leverage configured
            // recovery options, we need to terminate the process with a non-zero exit code.
            Environment.Exit(1);
        }
    }
}
