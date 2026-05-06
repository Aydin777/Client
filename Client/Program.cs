using System.Net.Sockets;
using System.Text;

Console.OutputEncoding = Encoding.UTF8;
TcpClient client = new TcpClient();
await client.ConnectAsync("10.1.18.16", 27001);

NetworkStream stream = client.GetStream();
StreamReader reader = new StreamReader(stream, Encoding.UTF8);
StreamWriter writer = new StreamWriter(stream, Encoding.UTF8) { AutoFlush = true };

Console.Write("Username daxil et: ");
string username = Console.ReadLine();

await writer.WriteLineAsync($"LOGIN:{username}");

string response = await reader.ReadLineAsync();
Console.WriteLine(response);

_ = Task.Run(async () =>
{
    while (true)
    {
        try
        {
            string msg = await reader.ReadLineAsync();
            if (msg != null)
                Console.WriteLine($"\n[Incoming]: {msg}");
        }
        catch
        {
            break;
        }
    }
});

while (true)
{
    Console.WriteLine("\n1. Show Users");
    Console.WriteLine("2. Go Chat");
    Console.Write("Seçim: ");
    string choice = Console.ReadLine();

    if (choice == "1")
    {
        await writer.WriteLineAsync("SHOW_USERS");
    }
    else if (choice == "2")
    {
        Console.Write("Kiminle chat: ");
        string target = Console.ReadLine()?.Trim().ToLower();

        Console.WriteLine("1.Text  2.File  3.Voice");
        string type = Console.ReadLine();
        if (type == "1")
        {
            Console.Write("Mesaj: ");
            string msg = Console.ReadLine();

            await writer.WriteLineAsync($"MSG_TEXT:{target}:{msg}");
        }
        else if (type == "2")
        {
            Console.Write("File path: ");
            string path = Console.ReadLine();

            if (File.Exists(path))
            {
                byte[] fileBytes = File.ReadAllBytes(path);
                string base64 = Convert.ToBase64String(fileBytes);

                await writer.WriteLineAsync($"MSG_FILE:{target}:{Path.GetFileName(path)}:{base64}");
            }
            else
            {
                Console.WriteLine("File tapılmadı");
            }
        }
        else if (type == "3")
        {
            Console.WriteLine("Voice göndərmək üçün hazır fayl istifadə et (wav/mp3)");

            Console.Write("File path: ");
            string path = Console.ReadLine();

            if (File.Exists(path))
            {
                byte[] fileBytes = File.ReadAllBytes(path);
                string base64 = Convert.ToBase64String(fileBytes);

                await writer.WriteLineAsync($"MSG_VOICE:{target}:{Path.GetFileName(path)}:{base64}");
            }
        }
    }
}