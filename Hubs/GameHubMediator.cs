namespace paper.Hubs
{
    public class GameHubMediator
    {
        public event EventHandler<ConnectionEventArgs> ClientConnected;
        public event EventHandler<ConnectionEventArgs> ClientDisconnected;
       public event EventHandler<MouseEventArgs> MouseMoved;

        public void FireClientConnected(string connectionId)
        {
            ClientConnected?.Invoke(this, new ConnectionEventArgs(connectionId));
        }

        public void FireClientDisconnected(string connectionId)
        {
            ClientDisconnected?.Invoke(this, new ConnectionEventArgs(connectionId));
        }

        public void FireMouseMoved(string connectionId, Point point)
        {
            MouseMoved?.Invoke(this, new MouseEventArgs(connectionId, point));
        }
    }
}