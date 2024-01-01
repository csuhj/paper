export class Point {
    public x: number;
    public y: number;

    public constructor(point: Partial<Point>) {
        if  (point) {
            this.x = point.x;
            this.y = point.y;
        }
    }

    public equals(otherPoint: Point) {
        return otherPoint &&
            otherPoint.x === this.x &&
            otherPoint.y === this.y;
    }

    public static calculatePointOnUnitVector(pointA, pointB) {
        var vector = {
            x: pointB.x - pointA.x,
            y: pointB.y - pointA.y
        };
    
        var magnitude = Point.calculateVectorMagnitude(vector);
    
        return new Point({
            x: vector.x / magnitude,
            y: vector.y / magnitude
        });
    }
    
    public static calculateVectorMagnitude(vector) {
        return Math.sqrt((vector.x * vector.x) + (vector.y * vector.y));
    }
}