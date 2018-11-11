using System;

namespace Server.Game
{
    public class SpawnArea
    {
        public int MinRow { get; set; }
        public int MaxRow { get; set; }
        public int MinCol { get; set; }
        public int MaxCol { get; set; }

        public SpawnArea(int[] location)
        {
            MinRow = location[0];
            MinCol = location[1];
            MaxRow = location[2];
            MaxCol = location[3];
        }

        public (int, int) GetRandomPoint(Random random)
        {
            return (random.Next(MinRow, MaxRow), random.Next(MinCol, MaxCol));
        }
    }
}