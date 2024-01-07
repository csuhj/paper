using Microsoft.AspNetCore.SignalR;
using paper.Hubs;

namespace paper.Services
{
    public class GameEngine
    {
        private List<Player> players;
        private Dictionary<string, Point> playerIdToDirectionVectorPointMap;
        private Dictionary<string, Trail> playerIdToTrailMap;
        private List<string> newPlayerColours;
        private int nextPlayerId;
        private Thread thread;
        private bool running = false;
        private Action<GameState> updateGameStateCallback;
        private const int worldWidth = 500;
        private const int worldHeight = 500;
        private readonly TimeSpan TrailRecordingTimeSpan = TimeSpan.FromSeconds(1);

        public int NumberOfPlayers => players.Count;

        public GameEngine(Action<GameState> updateGameStateCallback)
        {            
            players = new List<Player>();
            playerIdToDirectionVectorPointMap = new Dictionary<string, Point>();
            playerIdToTrailMap = new Dictionary<string, Trail>();
            newPlayerColours = new List<string>() { "red", "green", "blue", "cyan", "magenta", "yellow" };
            nextPlayerId = 1;

            running = false;

            this.updateGameStateCallback = updateGameStateCallback;
        }

        public void Start() {
            if (running)
                return;

            running = true;
            thread = new Thread(GameLoop)
            {
                Name = "GameLoop"
            };
            thread.Start();
        }

        public void Stop() {
            if (!running || thread == null)
                return;

            running = false;
            thread.Join();
        }

        public void AddPlayer(string playerId)
        {
            string colour = newPlayerColours[nextPlayerId % newPlayerColours.Count];
            players.Add(new Player() {Id = playerId, Name = $"Player {nextPlayerId}", Colour = colour, X = 0, Y = 0});
            playerIdToDirectionVectorPointMap.Add(playerId, new Point() {X = 0, Y = 0});
            playerIdToTrailMap.Add(playerId, new Trail() { Points = new List<Point>() });
            nextPlayerId++;
        }

        public void RemovePlayer(string playerId)
        {
            int indexOfPlayer = players.FindIndex(p => p.Id == playerId);
            if (indexOfPlayer < 0)
                return;

            players.RemoveAt(indexOfPlayer);
            playerIdToDirectionVectorPointMap.Remove(playerId);
            playerIdToTrailMap.Remove(playerId);
        }

        public void UpdatePlayerVectorPoint(string playerId, Point point)
        {
            playerIdToDirectionVectorPointMap[playerId] = point;
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

                        Point playerPoint = new Point() { X = player.X, Y = player.Y };

                        if (trail.Points.Count > 0)
                        {
                            foreach (Trail otherTrail in playerIdToTrailMap.Values) {
                                if (otherTrail.DoesLineCrossTrail(trail.Points[trail.Points.Count - 1], playerPoint)) {
                                    player.IsDead = true;
                                    break;
                                }
                            }
                        }

                        trail.Points.Add(playerPoint);
                    }
                    lastTrailPointRecorded = DateTime.Now;
                }

                GameState gameState = new GameState() { Players = players, PlayerIdToTrailMap = playerIdToTrailMap };
                updateGameStateCallback(gameState);
                Thread.Sleep(1000/25);
            }
        }
    }
}