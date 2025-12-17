using System.Net;
using System.Net.Sockets;
using System.Text.Json;

var random = new Random();

const int portNumber = 5000;
const int flushInterval = 1000; // milliseconds

var stocks = new List<(string name, string ticker)>
{
    ("Microsoft", "MSFT"),
    ("Apple", "AAPL"),
    ("Google", "GOOGL"),
    ("Amazon", "AMZN"),
    ("Tesla", "TSLA")
};


_ = Task.Run(() => RunServer());
await RunClient();

string CreateHttpResponse(string json) => 
    $"HTTP/1.1 200 OK\r\nContent-Type: application/json\r\nContent-Length: {json.Length}\r\n\r\n{json}";


void RunServer()
{
    var listener = new TcpListener(IPAddress.Loopback, portNumber);
    listener.Start();
    Console.WriteLine($"[SERVER] Listening on http://localhost:{portNumber}/");

    while (true)
    {
        var client = listener.AcceptTcpClient();
        _ = Task.Run(() => HandleClient(client));
    }
}

void HandleClient(TcpClient tcpClient)
{
    using (tcpClient)
    using (var writer = new StreamWriter(tcpClient.GetStream()))
    {
        var stockPrices = stocks.Select(s => new
        {
            name = s.name,
            ticker = s.ticker,
            price = (random.NextDouble() * 200 + 50).ToString("F2")
        }).ToList();

        var json = JsonSerializer.Serialize(stockPrices);
        var response = CreateHttpResponse(json);
        
        writer.Write(response);
        writer.Flush();
    }
}

async Task RunClient()
{
    await Task.Delay(flushInterval);
    var client = new HttpClient();

    while (true)
    {
        try
        {
            var json = await client.GetStringAsync($"http://localhost:{portNumber}/");
            var stocks = JsonSerializer.Deserialize<List<JsonElement>>(json);
            
            Console.Clear();
            Console.WriteLine("=== Stock Prices ===\n");
            foreach (var stock in stocks ?? new())
            {
                var name = stock.GetProperty("name").GetString();
                var ticker = stock.GetProperty("ticker").GetString();
                var price = stock.GetProperty("price").GetString();
                Console.WriteLine($"$ {name} ({ticker}) ${price}");
            }
            Console.WriteLine($"\nLast updated: {DateTime.Now:HH:mm:ss}");
            
            await Task.Delay(flushInterval);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[CLIENT] Error: {ex.Message}");
            await Task.Delay(flushInterval);
        }
    }
}
