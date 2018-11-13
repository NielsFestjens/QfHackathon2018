import * as signalR from '@aspnet/signalr';

const server = 'http://localhost:60860';

async function start() {
    const connection = new signalR.HubConnectionBuilder()
        .withUrl(`${server}/hub/client`)
        .build();

    const gameConnection = new GameConnection(connection);
    const game = new Game(gameConnection);
    const gameDrawer = new GameDrawer(game, <HTMLCanvasElement>document.getElementById("container"))
    gameDrawer.start();
    
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
                for (let content of tile.contents) {
                    ++content.informationAge;
                }
            }
        }

        for (let tile of data.tiles) {
            this.updateTile(tile);
        }
    }

    updateTile = (tileInfo: TileInfo) => {
        const tile = this.tiles[tileInfo.column][tileInfo.row];
        tile.contents = tile.contents.filter(x => x.informationAge === 0 || (x.type != tileInfo.content.type && x.id != tileInfo.content.id));
        var content = new TileContent();
        content.type = tileInfo.content.type;
        tile.contents.push(content)
    }
}

class Tile {
    contents: TileContent[] = [];
}

class TileContent {
    informationAge: number = 0;
    type: TileInfoContentType = 0;
    id: string = "";
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
    content: TileInfoContent;
}

type TileInfoContent = {
    type: TileInfoContentType;
    id: string;
}

enum TileInfoContentType {
    Unknown = 0,
    Obstacle = 1,
    Object = 2,
    Character = 3,
    Enemy = 4,
    Friendly = 5,
    Finish = 6
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

class GameDrawer
{
    private game: Game;
    private container: HTMLCanvasElement;
    private context: CanvasRenderingContext2D;

    private rectangleSize = 20;

    constructor(game: Game, container: HTMLCanvasElement) {
        this.game = game;
        this.container = container;
        this.context = this.container.getContext("2d")!;
        this.container.width = window.innerWidth;
        this.container.height = window.innerHeight;
    }

    start() {        
        setInterval(() => this.draw(), 1000 / 6);
    }

    draw() {
        var tiles = this.game.grid.tiles;
        for (var left = 0; left < tiles.length; left++) {
            for (var top = 0; top < tiles[left].length; top++) {
                this.drawTile(left, tiles[left].length - top - 1, tiles[left][top])
            }
        }
    }

    drawTile(left: number, top: number, tile: Tile) {
        this.fillRect(left, top, { Color: Color.LightGray });
        var contentColor = this.determineTileColor(tile);
        if (contentColor)
            this.fillRect(left, top, contentColor);
    }

    determineTileColor(tile: Tile): ColorInfo {
        var finish = this.getContentOfType(tile, TileInfoContentType.Finish);
        if (finish)
            return { Color: Color.Gold, Alpha: this.getAlphaForAge(finish) };

        var friendly = this.getContentOfType(tile, TileInfoContentType.Friendly);
        if (friendly)
            return { Color: Color.Green, Alpha: this.getAlphaForAge(friendly) };

        return { Color: Color.LightGray };
    }
    
    getContentOfType(tile: Tile, type: TileInfoContentType): TileContent {
        return tile.contents.filter(x => x.type == type)[0];
    }

    getAlphaForAge(tileContent: TileContent): number {
        if (tileContent.informationAge === 0)
            return 1;
        if (tileContent.informationAge === 1)
            return .5;
        if (tileContent.informationAge === 2)
            return .25;
        if (tileContent.informationAge === 3)
            return .125;
        if (tileContent.informationAge === 4)
            return .0625;
        return 0;
    }

    fillRect(left: number, top: number, color: ColorInfo) {
        this.context.fillStyle = color.Color;
        this.context.globalAlpha = color.Alpha === undefined ? 1 : color.Alpha;
        this.context.fillRect(left * this.rectangleSize + 1, top * this.rectangleSize + 1, this.rectangleSize - 2, this.rectangleSize - 2);
    }
}

type ColorInfo = {
    Color: string;
    Alpha?: number;
}

const Color = {
    White: "#FFFFFF",
    LightGray: "#EEEEEE",
    DarkGray: "#DDDDDD",
    Black: "#000000",
    Red: "#FF0000",
    Green: "#00FF00",
    Blue: "#0000FF",
    Cyan: "#00FFFF",
    Gold: "#EEDD44",
    LightGold: "#EEEE88"
};

start();