using Microsoft.AspNetCore.SignalR;
using paper.Hubs;

namespace paper.Services
{
    public class GameEngineService : IDisposable
    {
        private IHubContext<GameHub> gameHubContext;
        private GameHubMediator gameHubMediator;
        private GameEngine gameEngine;

        public GameEngineService(IHubContext<GameHub> gameHubContext, GameHubMediator gameHubMediator)
        {
            this.gameHubContext = gameHubContext;
            this.gameHubMediator = gameHubMediator;
            this.gameHubMediator.ClientConnected += OnClientConnected;
            this.gameHubMediator.ClientDisconnected += OnClientDisconnected;
            this.gameHubMediator.MouseMoved += OnMouseMoved;
            
            gameEngine = new GameEngine(gameState => GameHub.SendGameStateUpdate(gameHubContext, gameState).Wait());
        }

        public void Start() {
            gameEngine.Start();
        }

        public void Stop() {
            gameEngine.Stop();
        }

        public void Dispose()
        {
            gameEngine.Stop();

            this.gameHubMediator.ClientConnected -= OnClientConnected;
            this.gameHubMediator.ClientDisconnected -= OnClientDisconnected;
            this.gameHubMediator.MouseMoved -= OnMouseMoved;
        }

        private void OnClientConnected(object? sender, ConnectionEventArgs e)
        {
            if (gameEngine.NumberOfPlayers == 0)
            {
                gameEngine.Start();
                Console.WriteLine($"Started Game Engine");
            }

            gameEngine.AddPlayer(e.ConnectionId);
            Console.WriteLine($"Connected {e.ConnectionId}");
        }

        private void OnClientDisconnected(object? sender, ConnectionEventArgs e)
        {
            gameEngine.RemovePlayer(e.ConnectionId);
            if (gameEngine.NumberOfPlayers == 0)
            {
                gameEngine.Stop();
                Console.WriteLine($"Stopped Game Engine");
            }

            Console.WriteLine($"Disconnected {e.ConnectionId}");
        }

        private void OnMouseMoved(object? sender, MouseEventArgs e)
        {
            gameEngine.UpdatePlayerVectorPoint(e.ConnectionId, e.Point);
        }
    }
}