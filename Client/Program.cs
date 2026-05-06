using System.Net.Sockets;
using System.Text;

TcpClient client = new TcpClient();
await client.ConnectAsync("192.168.0.100", 27001);

var stream = client.GetStream();
var reader = new StreamReader(stream, Encoding.UTF8);
var writer = new StreamWriter(stream, Encoding.UTF8) { AutoFlush = true };

Console.Write("Username: ");
string username = Console.ReadLine();
await writer.WriteLineAsync($"LOGIN:{username}");

Console.WriteLine(await reader.ReadLineAsync());

_ = Task.Run(async () =>
{
    while (true)
    {
        try
        {
            var msg = await reader.ReadLineAsync();
            if (msg != null)
                Console.WriteLine($"\n{msg}");
        }
        catch { break; }
    }
});

while (true)
{
    Console.WriteLine("\n1. Users");
    Console.WriteLine("2. Send");
    Console.WriteLine("3. Unread");
    Console.Write("Seçim: ");

    var c = Console.ReadLine();

    if (c == "1")
    {
        await writer.WriteLineAsync("SHOW_USERS");
    }
    else if (c == "2")
    {
        Console.Write("Kime: ");
        var to = Console.ReadLine();

        Console.Write("Mesaj: ");
        var msg = Console.ReadLine();

        await writer.WriteLineAsync($"MSG_TEXT:{to}:{msg}");
    }
    else if (c == "3")
    {
        await writer.WriteLineAsync("SHOW_UNREAD");
    }
}