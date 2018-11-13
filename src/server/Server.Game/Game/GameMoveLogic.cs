using System;
using System.Collections.Generic;
using System.Linq;

namespace Server.Game
{
    public class GameMoveLogic : IDisposable
    {
        private readonly Dictionary<GamePlayer, Move> _moves = new Dictionary<GamePlayer, Move>();
        
        public void SetNextMove(GamePlayer player, Move move)
        {
            _moves[player] = move;
        }

        public void ProcessMoves()
        {
            foreach (var move in _moves)
            {
                ProcessMove(move.Key, move.Value);
            }
            _moves.Clear();
        }

        private void ProcessMove(GamePlayer player, Move move)
        {
            var (desiredColumn, desiredRow) = GetDesiredPosition(player, move);
            if (!CanWalkOn(desiredColumn, desiredRow, player))
                return;

            MovePlayer(player, desiredColumn, desiredRow);
        }

        private (int, int) GetDesiredPosition(GamePlayer player, Move move)
        {
            var (offSetColumn, offsetRow) = move.MapToOffset();
            var desiredColumn = (player.Column + offSetColumn).Between(0, player.Game.Columns - 1);
            var desiredRow = (player.Row + offsetRow).Between(0, player.Game.Rows - 1);
            return (desiredColumn, desiredRow);
        }

        private bool CanWalkOn(int column, int row, GamePlayer player)
        {
            return !player.Game.Tiles.For(column, row).Any(x => x.IsObstacle);
        }

        private void MovePlayer(GamePlayer player, int desiredColumn, int desiredRow)
        {
            if (player.Tile == null)
                return;

            player.Column = player.Tile.Column = desiredColumn;
            player.Row = player.Tile.Row = desiredRow;
        }

        public void Dispose()
        {
            _moves.Clear();
        }
    }
}