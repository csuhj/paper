namespace paper
{
    public class GameState
    {
        public List<Player> Players {get; set;}
        public Dictionary<string, Trail> PlayerIdToTrailMap {get; set;}
    }
}
