using Microsoft.AspNetCore.SignalR;
using paper.Hubs;

namespace paper.Services
{
    public class GameEngineService : IDisposable
    {
        private IHubContext<GameHub> gameHubContext;
        private GameHubMediator gameHubMediator;
        private List<Player> players;
        private List<string> newPlayerColours;
        private int nextPlayerId;
        private Thread thread;
        private bool running;

        public GameEngineService(IHubContext<GameHub> gameHubContext, GameHubMediator gameHubMediator)
        {
            this.gameHubContext = gameHubContext;
            this.gameHubMediator = gameHubMediator;
            this.gameHubMediator.ClientConnected += OnClientConnected;
            this.gameHubMediator.ClientDisconnected += OnClientDisconnected;
            
            players = new List<Player>();
            newPlayerColours = new List<string>() { "red", "green", "blue", "cyan", "magenta", "yellow" };
            nextPlayerId = 1;

            thread = new Thread(GameLoop);
            running = false;
        }

        public void Start() {
            running = true;
            thread.Start();
        }

        public void Stop() {
            running = false;
            thread.Join();
        }

        private void GameLoop()
        {
            Player player1 = new Player() {Name = "Player 1", Colour = "red", X = 0, Y = 0};
            Player player2 = new Player() {Name = "Player 2", Colour = "green",X = 0, Y = 0};
            Player player3 = new Player() {Name = "Player 3", Colour = "blue",X = 0, Y = 0};
            while (running)
            {
                foreach (Player player in players)
                {
                    player.X = (player.X + 1) % 300;
                    player.Y = (player.Y + 1) % 300;
                }

                GameState gameState = new GameState() { Players = players };
                GameHub.SendGameStateUpdate(gameHubContext, gameState).Wait();
                Thread.Sleep(100);
            }
        }

        public void Dispose()
        {
            this.gameHubMediator.ClientConnected -= OnClientConnected;
            this.gameHubMediator.ClientDisconnected -= OnClientDisconnected;
        }

        private void OnClientConnected(object? sender, ConnectionEventArgs e)
        {
            string colour = newPlayerColours[nextPlayerId % newPlayerColours.Count];
            players.Add(new Player() {Id = e.ConnectionId, Name = $"Player {nextPlayerId}", Colour = colour, X = 0, Y = 0});
            nextPlayerId++;

            Console.WriteLine($"Connected {e.ConnectionId}");
        }

        private void OnClientDisconnected(object? sender, ConnectionEventArgs e)
        {
            int indexOfPlayer = players.FindIndex(p => p.Id == e.ConnectionId);
            if (indexOfPlayer > -1)
                players.RemoveAt(indexOfPlayer);

            Console.WriteLine($"Disconnected {e.ConnectionId}");
        }
    }
}