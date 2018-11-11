namespace Server.Game
{
    public class TileContentData
    {
        public TileContentType Type { get; set; }
        public string Name { get; set; }

        public string Id { get; set; }

        public TileContentData(TileContentType type, string name)
        {
            Type = type;
            Name = name;
        }
    }
}