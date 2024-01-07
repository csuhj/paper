namespace paper
{
    public class Point
    {
        public double X {get; set;}
        public double Y {get; set;}

        public override bool Equals(object? obj)
        {
            return obj is Point point &&
              X == point.X &&
              Y == point.Y;
        }

        public override int GetHashCode()
        {
            return (int)(X * Y);
        }
    }
}
