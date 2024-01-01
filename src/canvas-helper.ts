import { Player } from "./player";

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
        var centerX = this.canvas.width / 2;
        var centerY = this.canvas.height / 2;

        var viewPortLeft = me.x - centerX;
        var viewPortTop = me.y - centerY;

        var offsetX = player.x - viewPortLeft;
        var offsetY = player.y - viewPortTop;

        this.context.beginPath();
        this.context.arc(offsetX, offsetY, /*player.size*/ 25, 0, 2 * Math.PI, false);
        this.context.fillStyle = player.colour;
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