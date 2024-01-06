using Microsoft.AspNetCore.SignalR;
using paper.Hubs;

namespace paper.Services
{
    public class GameEngineService : IDisposable
    {
        private IHubContext<GameHub> gameHubContext;
        private GameHubMediator gameHubMediator;
        private List<Player> players;
        private Dictionary<string, Point> playerIdToDirectionVectorPointMap;
        private Dictionary<string, Trail> playerIdToTrailMap;
        private List<string> newPlayerColours;
        private int nextPlayerId;
        private Thread thread;
        private bool running;
        private const int worldWidth = 500;
        private const int worldHeight = 500;
        private readonly TimeSpan TrailRecordingTimeSpan = TimeSpan.FromSeconds(1);

        public GameEngineService(IHubContext<GameHub> gameHubContext, GameHubMediator gameHubMediator)
        {
            this.gameHubContext = gameHubContext;
            this.gameHubMediator = gameHubMediator;
            this.gameHubMediator.ClientConnected += OnClientConnected;
            this.gameHubMediator.ClientDisconnected += OnClientDisconnected;
            this.gameHubMediator.MouseMoved += OnMouseMoved;
            
            players = new List<Player>();
            playerIdToDirectionVectorPointMap = new Dictionary<string, Point>();
            playerIdToTrailMap = new Dictionary<string, Trail>();
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
            DateTime lastTrailPointRecorded = DateTime.MinValue;
            while (running)
            {
                foreach (Player player in players)
                {
                    if (player.IsDead)
                        continue;

                    if (!playerIdToDirectionVectorPointMap.TryGetValue(player.Id, out Point point))
                        continue;

                    int speed = 3;

                    int newX = player.X + (int)Math.Round(point.X * speed);
                    int newY = player.Y + (int)Math.Round(point.Y * speed);
                    if (newX > 0 && newX < worldWidth)
                        player.X = newX;
                    
                    if (newY > 0 && newY < worldHeight)
                        player.Y = newY;
                }

                if (lastTrailPointRecorded + TrailRecordingTimeSpan < DateTime.Now)
                {
                    foreach (Player player in players)
                    {
                        if (player.IsDead)
                            continue;

                        if (!playerIdToTrailMap.TryGetValue(player.Id, out Trail trail))
                            continue;

                        trail.Points.Add(new Point() { X = player.X, Y = player.Y });
                    }
                    lastTrailPointRecorded = DateTime.Now;
                }

                GameState gameState = new GameState() { Players = players, PlayerIdToTrailMap = playerIdToTrailMap };
                GameHub.SendGameStateUpdate(gameHubContext, gameState).Wait();
                Thread.Sleep(1000/25);
            }
        }

        public void Dispose()
        {
            this.gameHubMediator.ClientConnected -= OnClientConnected;
            this.gameHubMediator.ClientDisconnected -= OnClientDisconnected;
            this.gameHubMediator.MouseMoved -= OnMouseMoved;
        }

        private void OnClientConnected(object? sender, ConnectionEventArgs e)
        {
            string colour = newPlayerColours[nextPlayerId % newPlayerColours.Count];
            players.Add(new Player() {Id = e.ConnectionId, Name = $"Player {nextPlayerId}", Colour = colour, X = 0, Y = 0});
            playerIdToDirectionVectorPointMap.Add(e.ConnectionId, new Point() {X = 0, Y = 0});
            playerIdToTrailMap.Add(e.ConnectionId, new Trail() { Points = new List<Point>() });
            nextPlayerId++;

            Console.WriteLine($"Connected {e.ConnectionId}");
        }

        private void OnClientDisconnected(object? sender, ConnectionEventArgs e)
        {
            int indexOfPlayer = players.FindIndex(p => p.Id == e.ConnectionId);
            if (indexOfPlayer < 0)
                return;

            players.RemoveAt(indexOfPlayer);
            playerIdToDirectionVectorPointMap.Remove(e.ConnectionId);
            Console.WriteLine($"Disconnected {e.ConnectionId}");
        }

        private void OnMouseMoved(object? sender, MouseEventArgs e)
        {
            playerIdToDirectionVectorPointMap[e.ConnectionId] = e.Point;
        }
    }
}