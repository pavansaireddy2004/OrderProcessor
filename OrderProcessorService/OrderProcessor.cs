using Microsoft.Extensions.Logging;
using System.Text.Json;
using System.Threading.Tasks;
using System.IO;

public class OrderProcessor
{
    private readonly ILogger<OrderProcessor> _logger;
    private readonly OrderRepository _repo;

    public OrderProcessor(ILogger<OrderProcessor> logger, OrderRepository repo)
    {
        _logger = logger;
        _repo = repo;
    }

    public async Task ProcessFile(string path)
    {
        string json = await File.ReadAllTextAsync(path);

        Order order;

        // Try to parse the JSON
        try
        {
            order = JsonSerializer.Deserialize<Order>(json);
        }
        catch
        {
            await _repo.SaveInvalid(path, json, "Corrupted JSON");
            _logger.LogWarning($"Invalid JSON file: {path}");
            return;
        }

        // Validation rules
        if (order.TotalAmount < 0)
        {
            await _repo.SaveInvalid(path, json, "TotalAmount < 0");
            return;
        }

        if (string.IsNullOrWhiteSpace(order.CustomerName))
        {
            await _repo.SaveInvalid(path, json, "CustomerName missing or empty");
            return;
        }

        // Business rule: High Value
        if (order.TotalAmount > 1000)
            order.IsHighValue = true;

        // Save valid order
        await _repo.SaveValid(order, path);

        _logger.LogInformation($"Order processed: {order.OrderId}");
    }
}
