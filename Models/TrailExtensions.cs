namespace paper
{
    //see http://jeffreythompson.org/collision-detection/poly-poly.php
    public static class TrailExtensions
    {
        public static bool DoesLineCrossTrail(this Trail trail, Point lineStart, Point lineEnd)
        {
            if (trail.Points == null || trail.Points.Count == 0)
                return false;

            for(int i=1; i<trail.Points.Count; i++) {
                if ((!lineStart.Equals(trail.Points[i])) && DoLinesCross(lineStart, lineEnd, trail.Points[i-1], trail.Points[i]))
                    return true;
            }

            return false;
        }

        private static bool DoLinesCross(Point startPoint1, Point endPoint1, Point startPoint2, Point endPoint2)
        {
            double uA = (((endPoint2.X - startPoint2.X) * (startPoint1.Y - startPoint2.Y)) - 
                ((endPoint2.Y - startPoint2.Y) * (startPoint1.X - startPoint2.X))) / 
                (((endPoint2.Y - startPoint2.Y) * (endPoint1.X - startPoint1.X)) - 
                ((endPoint2.X - startPoint2.X) * (endPoint1.Y - startPoint1.Y)));
            
            double uB = (((endPoint1.X - startPoint1.X) * (startPoint1.Y - startPoint2.Y)) - 
                ((endPoint1.Y - startPoint1.Y) * (startPoint1.X - startPoint2.X))) / 
                (((endPoint2.Y - startPoint2.Y) * (endPoint1.X - startPoint1.X)) - 
                ((endPoint2.X - startPoint2.X) * (endPoint1.Y - startPoint1.Y)));

            return uA >= 0 && uA <=1 && uB >= 0 && uB <=1;
        }
    }
}