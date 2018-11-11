import * as signalR from '@aspnet/signalr';

const server = 'http://localhost:60860';

async function start() {
    const connection = new signalR.HubConnectionBuilder()
        .withUrl(`${server}/hub/client`)
        .build();

    const gameConnection = new GameConnection(connection);
    const game = new Game(gameConnection);
    
    try {
        await game.start();
    } catch (err) {
        console.error('Connection error', err.toString());
    }
}

class GameConnection {

    game!: IGame;

    constructor(
        private connection: signalR.HubConnection
    ) {
        this.connection.on("Kicked", () => this.game.kicked())
        this.connection.on("GameStarted", (event: GameStartedEvent) => this.game.started(event))
        this.connection.on("GameUpdated", (event: GameUpdatedEvent) => this.game.updated(event))
    }

    register = (game: IGame) => {
        this.game = game;
    }

    start = () => this.connection.start();
    connect = (command: ConnectCommand) => this.connection.send("Connect", command);
    stop = () => this.connection.stop();
    moveUp = () => this.connection.send("MoveUp");
    moveDown = () => this.connection.send("MoveDown");
    moveLeft = () => this.connection.send("MoveLeft");
    moveRight = () => this.connection.send("MoveRight");
    attack = (command: AttackCommand) => this.connection.send("Attack", command);
}

interface IGame {
    kicked: () => void;
    started: (event: GameStartedEvent) => void;
    updated: (event: GameUpdatedEvent) => void;
}

class Game implements IGame {
    grid: Grid = new Grid(0, 0);
    player: Player = new Player();

    constructor(
        private connection: GameConnection
    ) {
        connection.register(this);
    }

    start = async () => {
        await this.connection.start();
        this.connection.connect({ name: "KickMe" });
    } 

    kicked = () => {
        this.connection.stop();
    }

    started = (event: GameStartedEvent) => {
        this.grid = new Grid(event.columns, event.rows);
    };

    updated = (event: GameUpdatedEvent) => {
        console.log(event.player);
        this.grid.update(event);
        const nextMove = this.player.getNextAction(this.grid);
        console.log(ActionType[nextMove.action]);
        
        switch(nextMove.action) {
            case ActionType.moveUp: this.connection.moveUp(); break;
            case ActionType.moveDown: this.connection.moveDown(); break;
            case ActionType.moveLeft: this.connection.moveLeft(); break;
            case ActionType.moveRight: this.connection.moveRight(); break;
        }
    };
}

class Player {
    getNextAction(grid: Grid) {
        const nextAction = new PlayerAction();
        nextAction.action = ActionType.moveUp;        
        return nextAction;
    }
}

class PlayerAction {
    action: ActionType = 0;
    payload: any;
}

enum ActionType {
    unknown = 0,
    moveUp = 1,
    moveDown = 2,
    moveLeft = 3,
    moveRight = 4,
    attack = 5
}

class Grid {

    tiles: Tile[][];

    constructor(sizeX: number, sizeY: number) {
        this.tiles = [];
        for (let x = 0; x < sizeX; x++) {
            this.tiles[x] = [];
            for (let y = 0; y < sizeY; y++) {
                this.tiles[x][y] = new Tile();
            }
        }
    }

    update = (data: GameUpdatedEvent) => {
        for (let tileRow of this.tiles) {
            for (let tile of tileRow) {
                ++tile.informationAge;
            }
        }

        for (let tile of data.tiles) {
            this.updateTile(tile);
        }
    }

    updateTile = (tileInfo: TileInfo) => {
        const tile = this.tiles[tileInfo.column][tileInfo.row];

        tile.informationAge = 0;
    }
}

class Tile {
    informationAge: number = 0;
}

type GameStartedEvent = {
    level: number;
    name: string;
    columns: number;
    rows: number;
}

type GameUpdatedEvent = {
    tiles: TileInfo[];
    player: PlayerInfo;
}

type TileInfo = {
    row: number;
    column: number;
    contents: TileInfoContent;
}

type TileInfoContent = {
    type: TileInfoContentType;
}

enum TileInfoContentType {
    Unknown = 0,
    Obstacle = 1,
    Object = 2,
    Character = 3,
    Enemy = 4,
    Friendly = 5
}

type AttackCommand = {

}

type ConnectCommand = {
    name: string;
}

type PlayerInfo = {
    Row: number;
    Column: number;
}

start();