export class Player {
    public id: string;
    public name: string;
    public colour: string;
    public isDead: boolean;
    public x: number;
    public y: number;

    public constructor(player: Partial<Player>) {
        if  (player) {
            this.id = player.id;
            this.name = player.name;
            this.colour = player.colour;
            this.isDead = player.isDead;
            this.x = player.x;
            this.y = player.y;
        }
    }
}