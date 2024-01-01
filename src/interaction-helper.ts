import { Point } from "./point";

export class InteractionHelper {

    private latestMousePos: Point;

    public constructor(private canvas: HTMLCanvasElement) {
        this.latestMousePos = new Point({x: 0, y:0})
        this.registerInteractionListeners();
    }

    public findMouseDirectionVectorPoint() {
        const canvasCentre = new Point({
            x: this.canvas.width / 2,
            y: this.canvas.height / 2
        });

        return Point.calculatePointOnUnitVector(canvasCentre, this.latestMousePos);
    }

    private registerInteractionListeners() {
        this.canvas.addEventListener('mousemove', evt => {
            this.latestMousePos = this.getMousePos(evt);
        }, false);
    }

    private getMousePos(evt) {
        var rect = this.canvas.getBoundingClientRect();
        return new Point({
            x: evt.clientX - rect.left,
            y: evt.clientY - rect.top
        });
    }
}