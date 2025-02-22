using System;
using System.Diagnostics;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

class Client 
{
    static async Task Main(string[] args) 
    {

        string uri = "ws://localhost:5000/";
        await Start(uri);
    }

    private static async Task Start(string uri)
    {

        using (ClientWebSocket webSocket = new()) 
        {
            try 
            {
                await webSocket.ConnectAsync(new Uri(uri), CancellationToken.None); 
                Console.WriteLine($"Connected to {uri}");
                int counter = 0;

                // Stopwatch to measure how much time server spends to handle the messages.
                // Stopwatch stopwatch = Stopwatch.StartNew();

                while (webSocket.State == WebSocketState.Open && counter < 1000)
                {
                    // Send a message to a server
                    string message = GenerateMessage(++counter);

                    // string message = Console.ReadLine();
                    if (message == "exit")
                    {
                        break;
                    }
                    byte[] messageBytes = Encoding.UTF8.GetBytes(message);
                    await webSocket.SendAsync(new ArraySegment<byte>(messageBytes), WebSocketMessageType.Text, true, CancellationToken.None);

                    // Receive message from a server
                    Task receiveTask = ReceiveMessages(webSocket);
                }

                // stopwatch.Stop();
                // Console.WriteLine(stopwatch.ElapsedMilliseconds);

                await webSocket.CloseAsync(WebSocketCloseStatus.NormalClosure, "Client closing", CancellationToken.None);
            }

            catch (Exception ex)
            {
                Console.WriteLine($"Exception: {ex.Message}");
            }
        }
    }

    private static string GenerateMessage(int counter)
    {
        // message sample:  tag_id, timestamp,    x,    y,    z,battery_level,battery_voltage,status
        //                 TAG_001,1672531200,0.000,0.000,0.000,          100,           4.20,active
        string message = "TAG_001," + counter * 1000 + "0.000,0.000,0.000,100,4.20,active";
        return message;
    }

    private static async Task ReceiveMessages(ClientWebSocket webSocket)
    {
        byte[] buffer = new byte[1024];
        while (webSocket.State == WebSocketState.Open)
        {
            WebSocketReceiveResult res = await webSocket.ReceiveAsync(
                new ArraySegment<byte>(buffer), CancellationToken.None
            );
            string receivedMessage = Encoding.UTF8.GetString(buffer, 0, res.Count);
            Console.WriteLine($"Received message: {receivedMessage}");
        }
    }
}