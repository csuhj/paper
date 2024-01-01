using Microsoft.AspNetCore.SignalR;

namespace paper.Hubs
{
    public class GameHub : Hub
    {
        private  GameHubMediator gameHubMediator;
        
        public GameHub(GameHubMediator gameHubMediator)
        {
            this.gameHubMediator = gameHubMediator;
        }

        public async Task SendMessage(string user, string message)
        {
            await Clients.All.SendAsync("ReceiveMessage", user, message);
        }

        public async Task NewMessage(long username, string message) =>
            await Clients.All.SendAsync("messageReceived", username, message);


        public override Task OnConnectedAsync()
        {
            gameHubMediator.FireClientConnected(Context.ConnectionId);
            return base.OnConnectedAsync();
        }

        public override Task OnDisconnectedAsync(Exception? exception)
        {
            gameHubMediator.FireClientDisconnected(Context.ConnectionId);
            return base.OnDisconnectedAsync(exception);
        }

        public static async Task SendGameStateUpdate(IHubContext<GameHub> hubContext, GameState gameState)
        {
            await hubContext.Clients.All.SendAsync("gameStateUpdate", gameState);
        }
    }
}