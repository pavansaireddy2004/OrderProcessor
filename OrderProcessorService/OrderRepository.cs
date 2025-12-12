using Microsoft.Data.Sqlite;
using System.IO;
using System.Threading.Tasks;

public class OrderRepository
{
    private const string DbPath = "orders.db";

    public OrderRepository()
    {
        // Create DB and tables if not exist
        if (!File.Exists(DbPath))
        {
            using var conn = new SqliteConnection($"Data Source={DbPath}");
            conn.Open();

            string validTableSql = @"
            CREATE TABLE ValidOrders (
                OrderId TEXT PRIMARY KEY,
                CustomerName TEXT,
                OrderDate TEXT,
                TotalAmount REAL,
                IsHighValue INTEGER,
                FileName TEXT
            );";

            string invalidTableSql = @"
            CREATE TABLE InvalidOrders (
                Id INTEGER PRIMARY KEY AUTOINCREMENT,
                FileName TEXT,
                RawContent TEXT,
                Reason TEXT
            );";

            new SqliteCommand(validTableSql, conn).ExecuteNonQuery();
            new SqliteCommand(invalidTableSql, conn).ExecuteNonQuery();
        }
    }

    // Save a valid order
    public async Task SaveValid(Order order, string file)
    {
        using var conn = new SqliteConnection($"Data Source={DbPath}");
        await conn.OpenAsync();

        string sql = @"
        INSERT OR IGNORE INTO ValidOrders
        (OrderId, CustomerName, OrderDate, TotalAmount, IsHighValue, FileName)
        VALUES ($id, $name, $date, $amount, $hv, $file);";

        var cmd = conn.CreateCommand();
        cmd.CommandText = sql;

        cmd.Parameters.AddWithValue("$id", order.OrderId);
        cmd.Parameters.AddWithValue("$name", order.CustomerName);
        cmd.Parameters.AddWithValue("$date", order.OrderDate);
        cmd.Parameters.AddWithValue("$amount", order.TotalAmount);
        cmd.Parameters.AddWithValue("$hv", order.IsHighValue ? 1 : 0);
        cmd.Parameters.AddWithValue("$file", Path.GetFileName(file));

        await cmd.ExecuteNonQueryAsync();
    }

    // Save an invalid order
    public async Task SaveInvalid(string file, string json, string reason)
    {
        using var conn = new SqliteConnection($"Data Source={DbPath}");
        await conn.OpenAsync();

        string sql = @"
        INSERT INTO InvalidOrders (FileName, RawContent, Reason)
        VALUES ($file, $raw, $reason);";

        var cmd = conn.CreateCommand();
        cmd.CommandText = sql;

        cmd.Parameters.AddWithValue("$file", Path.GetFileName(file));
        cmd.Parameters.AddWithValue("$raw", json);
        cmd.Parameters.AddWithValue("$reason", reason);

        await cmd.ExecuteNonQueryAsync();
    }
}
