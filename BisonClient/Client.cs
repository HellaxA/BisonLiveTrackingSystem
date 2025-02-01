using System;
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
                Task receiveTask = ReceiveMessages(webSocket);

                while (webSocket.State == WebSocketState.Open)
                {
                    string message = Console.ReadLine();
                    if (message == "exit")
                    {
                        break;
                    }
                    byte[] messageBytes = Encoding.UTF8.GetBytes(message);
                    await webSocket.SendAsync(new ArraySegment<byte>(messageBytes), WebSocketMessageType.Text, true, CancellationToken.None);
                }
                await webSocket.CloseAsync(WebSocketCloseStatus.NormalClosure, "Client closing", CancellationToken.None);
            }

            catch (Exception ex)
            {
                Console.WriteLine($"Exception: {ex.Message}");
            }
        }
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