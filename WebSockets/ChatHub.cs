using Microsoft.AspNetCore.SignalR;

namespace TeamsApplicationServer.WebSockets
{
    public class ChatHub : Hub
    {
        private static readonly Dictionary<string, string> UserConnectionMap = new();

        public override async Task OnConnectedAsync()
        {
            string userName = Context.User?.Identity?.Name ?? Context.ConnectionId;

            Console.WriteLine($"User {userName} connected with connection ID {Context.ConnectionId}");  
            UserConnectionMap[userName] = Context.ConnectionId;

            await base.OnConnectedAsync();
        }

        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            string userName = Context.User?.Identity?.Name ?? Context.ConnectionId;

            UserConnectionMap.Remove(userName);

            await base.OnDisconnectedAsync(exception);
        }

        public async Task SendMessage(string user, string message)
        {
            Console.WriteLine(Context.User?.Identity?.Name ?? Context.ConnectionId);
            Console.WriteLine(user);
            Console.WriteLine(message);

            await Clients.All.SendAsync("ReceiveMessage", user, message);
        }
        public async Task SendPrivateMessage(string toUser, string message)
        {
            if (UserConnectionMap.TryGetValue(toUser, out var connectionId))
            {
                await Clients.Client(connectionId).SendAsync("ReceiveMessage", message);
            }
        }
    }
 }
