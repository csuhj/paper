import { Player } from "./player";
import { Trail } from "./trail";

export class GameState {
    public players: Player[];
    public playerIdToTrailMap: {[id: string]: Trail};

    public constructor(gameState: Partial<GameState>) {
        if  (gameState) {
            this.players = gameState.players?.map(p => new Player(p));
            this.playerIdToTrailMap = !gameState.playerIdToTrailMap
            ? undefined
            : Object.keys(gameState.playerIdToTrailMap).reduce(
                (obj, key) => ({ ...obj, [key]: new Trail(gameState.playerIdToTrailMap[key]) }),
                {}
              );
        }
    }
}