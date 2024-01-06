import { Player } from "./player";
import { Trail } from "./trail";

export class CanvasHelper {
    private readonly backgroundPatternWidth = 20;
    private readonly backgroundPatternHeight = 20;
    private context: CanvasRenderingContext2D;
    private backgroundPattern: CanvasPattern;

    public constructor(private canvas: HTMLCanvasElement) {
        this.context = canvas.getContext('2d');
        this.backgroundPattern = this.createBackgroundPattern();
    }

    public clearCanvas() {
        this.context.clearRect(0, 0, this.canvas.width, this.canvas.height);
    }

    public drawBackground(me: Player) {
        var centerX = this.canvas.width / 2;
        var centerY = this.canvas.height / 2;

        var viewPortLeft = me.x - centerX;
        var viewPortTop = me.y - centerY;

        var offsetX = viewPortLeft % this.backgroundPatternWidth;
        var offsetY = viewPortTop % this.backgroundPatternHeight;

        this.context.translate(-offsetX, -offsetY);

        this.context.fillStyle = this.backgroundPattern;
        this.context.fillRect(0, 0, this.canvas.width + this.backgroundPatternWidth, this.canvas.height + this.backgroundPatternHeight);
        this.context.fill();

        this.context.translate(offsetX, offsetY);
    }

    public drawPlayer(me: Player, player: Player) {
        const [viewPortLeft, viewPortTop] = this.getViewPortTopLeft(me);

        const offsetX = player.x - viewPortLeft;
        const offsetY = player.y - viewPortTop;

        this.context.beginPath();
        this.context.arc(offsetX, offsetY, /*player.size*/ 25, 0, 2 * Math.PI, false);
        this.context.fillStyle = this.getPlayerColour(player);
        this.context.fill();
        this.context.lineWidth = 1;
        this.context.strokeStyle = 'black';
        this.context.stroke();

        if (player.name != undefined) {
            this.context.fillStyle = 'black';
            this.context.textAlign = 'center';
            this.context.font='20px Georgia';
            this.context.fillText(player.name,offsetX, offsetY);
        }
    }

    public drawTrail(me: Player, player: Player, trail: Trail) {
        const [viewPortLeft, viewPortTop] = this.getViewPortTopLeft(me);

        this.context.beginPath();
        this.context.lineWidth = 5;
        this.context.strokeStyle = this.getPlayerColour(player);

        for (var i=0; i<trail.points?.length; i++) {
            if (!trail.points[i]) {
                continue;
            }
            
            const trailPointOffsetX = trail.points[i].x - viewPortLeft;
            const trailPointOffsetY = trail.points[i].y - viewPortTop;

            if (i === 0) {
                this.context.moveTo(trailPointOffsetX, trailPointOffsetY);
            } else {
                this.context.lineTo(trailPointOffsetX, trailPointOffsetY);
            }
        }

        const playerOffsetX = player.x - viewPortLeft;
        const playerOffsetY = player.y - viewPortTop;
        this.context.lineTo(playerOffsetX, playerOffsetY);

        this.context.stroke();
    }

    private getViewPortTopLeft(me: Player) {
        var centerX = this.canvas.width / 2;
        var centerY = this.canvas.height / 2;

        var viewPortLeft = me.x - centerX;
        var viewPortTop = me.y - centerY;

        return [viewPortLeft, viewPortTop];
    }

    private getPlayerColour(player: Player) {
        return player.isDead? 'grey' : player.colour;
    }

    private createBackgroundPattern() {
        var canvasPattern = document.createElement("canvas");
        canvasPattern.width = this.backgroundPatternWidth;
        canvasPattern.height = this.backgroundPatternHeight;
        var contextPattern = canvasPattern.getContext("2d");

        contextPattern.beginPath();
        contextPattern.strokeStyle = '#BBBBBB';
        contextPattern.strokeRect(0.5, 0.5, this.backgroundPatternWidth, this.backgroundPatternHeight);
        contextPattern.stroke();

        return this.context.createPattern(canvasPattern,"repeat");
    }
}