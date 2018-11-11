namespace Server.Game
{
    public enum Move
    {
        None = 0,
        Up = 1,
        Right = 2,
        Down = 3,
        Left = 4
    }

    public static class MoveExtensions
    {
        public static (int, int) MapToOffset(this Move move)
        {
            switch (move)
            {
                case Move.Up: return (0, 1);
                case Move.Right: return (1, 0);
                case Move.Down: return (0, -1);
                case Move.Left: return (-1, 0);
                default: return (0, 0);
            }
        }
    }
}