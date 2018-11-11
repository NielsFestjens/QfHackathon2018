namespace Server.Game
{
    public class TileData
    {
        public int Row { get; set; }
        public int Column { get; set; }

        public TileContentData Content { get; set; }

        public TileData(int row, int column, TileContentData content)
        {
            Row = row;
            Column = column;
            Content = content;
        }
    }
}