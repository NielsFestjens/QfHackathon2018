import './astar';

export type AStar = {
    search: (graph: Graph, start: GridNode, end: GridNode, options?: any) => GridNode[];
}

export class Graph {
    grid: GridNode[][];
    constructor(grid: number[][]);
}

export type GridNode = {
    x: number;
    y: number;
}

export const astar: AStar;