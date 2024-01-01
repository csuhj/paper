import { Player } from "./player";

export class GameState {
    public players: Player[];

    public constructor(gameState: Partial<GameState>) {
        if  (gameState) {
            this.players = gameState.players?.map(p => new Player(p));
        }
    }
}