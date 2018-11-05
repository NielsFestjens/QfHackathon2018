namespace Server.Game.Levels
{
    public class LevelData
    {
        public int Level { get; set; }
        public string Name { get; set; }
        public int Rooms { get; set; }
        public int SlotsPerRoom { get; set; }
        public bool Locked { get; set; }
        public GridData Grid { get; set; }
    }

    public class GridData
    {
        public int Rows { get; set; }
        public int Cols { get; set; }
        public int[][] Walls { get; set; }
        public int[][] SpwawnAreas { get; set; }
    }
}