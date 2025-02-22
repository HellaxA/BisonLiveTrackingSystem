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
        await Listen(listener);

        // In case I want to shut down the server correctly I need to use a custom Stop method (like in the article).
        // Task.Run(() => Listen(listener).ConfigureAwait(false));
    }

    private static async Task Listen(HttpListener listener)
    {
        while (true)
        {
            HttpListenerContext context = await listener.GetContextAsync();
            if (context.Request.IsWebSocketRequest)
            {
                HttpListenerWebSocketContext wsContext = await context.AcceptWebSocketAsync(null);
                Console.WriteLine("WebSocket Connection Established");
                //TODO remove _ = or test if I even need it
                _ = Task.Run(() => HandleConnection(wsContext.WebSocket).ConfigureAwait(false));
                
                //This is why I couldn't get multiple clients at the same time
                //await HandleConnection(wsContext.WebSocket);
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
        //TODO check what happens if I send a message more than 1024 bytes.
        byte[] buffer = new byte[1024];
        while (webSocket.State == WebSocketState.Open)
        {
            try
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
                    string response = $"Server: {receivedMessage}";
                    byte[] responseBytes = Encoding.UTF8.GetBytes(response);
                   // await webSocket.SendAsync(new ArraySegment<byte>(responseBytes), WebSocketMessageType.Text, true, CancellationToken.None);
                    Console.WriteLine($"Sent: {response}");

                }
            }
            catch (WebSocketException ex)
            {
                Console.WriteLine($"Remote client must have closed the connection unexpectedly: {ex}");
            }
        }
    }
}