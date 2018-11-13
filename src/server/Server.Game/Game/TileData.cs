namespace Server.Game
{
    public class TileData
    {
        public int Column { get; set; }
        public int Row { get; set; }

        public TileContentData Content { get; set; }

        public TileData(int column, int row, TileContentData content)
        {
            Column = column;
            Row = row;
            Content = content;
        }
    }
}