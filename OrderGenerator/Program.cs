using System.Text.Json;

string folder = "../OrderProcessorService/IncomingOrders/";
Directory.CreateDirectory(folder);

Console.WriteLine("Order Generator Tool");
Console.WriteLine("Usage: dotnet run -- <count> <mode>");
Console.WriteLine("Modes: valid, invalid, corrupt");

if (args.Length < 2)
{
    Console.WriteLine("Example: dotnet run -- 10 valid");
    return;
}

int count = int.Parse(args[0]);
string mode = args[1].ToLower();

var rand = new Random();

for (int i = 0; i < count; i++)
{
    string path = Path.Combine(folder, $"order_{Guid.NewGuid()}.json");

    string json = mode switch
    {
        "valid" => JsonSerializer.Serialize(new
        {
            OrderId = Guid.NewGuid().ToString(),
            CustomerName = "Customer " + rand.Next(1000),
            OrderDate = DateTime.UtcNow.ToString("o"),
            TotalAmount = rand.Next(10, 2000)
        }),

        "invalid" => JsonSerializer.Serialize(new
        {
            OrderId = Guid.NewGuid().ToString(),
            CustomerName = "",
            OrderDate = DateTime.UtcNow.ToString("o"),
            TotalAmount = -10
        }),

        "corrupt" => "{ this is not valid json !!! }",

        _ => throw new Exception("Unknown mode")
    };

    File.WriteAllText(path, json);
}

Console.WriteLine($"Generated {count} {mode} order files.");
