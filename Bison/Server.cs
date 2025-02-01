using System;
using System.Text;
using System.Net;
using System.Net.WebSockets;



class Server
{
    static async Task Main(string[] args)
    {
        string uri = "http://localhost:5000/";
        await Start(uri);
    }

    static async Task Start(string uri)
    {
        HttpListener listener = new();
        listener.Prefixes.Add(uri);
        listener.Start();
        Console.WriteLine($"Server started at {uri}");

        while (true)
        {
            HttpListenerContext context = await listener.GetContextAsync();
            if (context.Request.IsWebSocketRequest)
            {
                HttpListenerWebSocketContext wsContext = await context.AcceptWebSocketAsync(null);
                Console.WriteLine("WebSocket Connection Established");
                await HandleConnection(wsContext.WebSocket);
            }
            else
            {
                context.Response.StatusCode = 400;
                context.Response.Close();
            }
        }
    }

    private static async Task HandleConnection(WebSocket webSocket)
    {
        byte[] buffer = new byte[1024];
        while (webSocket.State == WebSocketState.Open)
        {
            WebSocketReceiveResult result = await webSocket.ReceiveAsync(
                new ArraySegment<byte>(buffer), CancellationToken.None);

            string receivedMessage = Encoding.UTF8.GetString(buffer, 0, result.Count);                
            Console.WriteLine($"Received: {receivedMessage}");

            if (result.MessageType == WebSocketMessageType.Close)
            {
                await webSocket.CloseAsync(WebSocketCloseStatus.NormalClosure, "Closing", CancellationToken.None);    
                Console.WriteLine("WebSocket Connection Closed");
            }
            else 
            {
                string response = $"server: {receivedMessage}";
                byte[] responseBytes = Encoding.UTF8.GetBytes(response);
                await webSocket.SendAsync(new ArraySegment<byte>(responseBytes), WebSocketMessageType.Text, true, CancellationToken.None);
                Console.WriteLine($"Sent: {response}");

            }
        }
    }
}