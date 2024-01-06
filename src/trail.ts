import { Point } from "./point";

export class Trail {
    public points: Point[];

    public constructor(trail: Partial<Trail>) {
        if  (trail) {
            this.points = trail.points?.map(p => new Point(p));
        }
    }
}