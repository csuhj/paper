export class Player {
    public id: string;
    public name: string;
    public colour: string;
    public x: number;
    public y: number;

    public constructor(player: Partial<Player>) {
        if  (player) {
            this.id = player.id;
            this.name = player.name;
            this.colour = player.colour;
            this.x = player.x;
            this.y = player.y;
        }
    }
}