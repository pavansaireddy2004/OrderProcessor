using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

public class Worker : BackgroundService
{
    private readonly ILogger<Worker> _logger;
    private readonly OrderProcessor _processor;
    private FileSystemWatcher _watcher;

    public Worker(ILogger<Worker> logger, OrderProcessor processor)
    {
        _logger = logger;
        _processor = processor;
    }

    protected override Task ExecuteAsync(CancellationToken stoppingToken)
    {
        Directory.CreateDirectory("IncomingOrders");

        _watcher = new FileSystemWatcher("IncomingOrders", "*.json");
        
        _watcher.Created += OnNewFile;
        _watcher.EnableRaisingEvents = true;

        _logger.LogInformation("Service started, watching: IncomingOrders");

        return Task.CompletedTask;
    }

    private async void OnNewFile(object sender, FileSystemEventArgs e)
    {
        await Task.Delay(300);

        try
        {
            _logger.LogInformation($"New file detected: {e.Name}");
            await _processor.ProcessFile(e.FullPath);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error processing file");
        }
    }
}
