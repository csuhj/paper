namespace paper.Hubs
{
    public class MouseEventArgs : ConnectionEventArgs
    {
        public Point Point { get; set; }

        public MouseEventArgs(string connectionId, Point point)
            : base (connectionId)
        {
            Point = point;
        }
    }
}