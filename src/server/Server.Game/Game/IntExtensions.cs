using System;

namespace Server.Game
{
    public static class IntExtensions
    {
        public static int Between(this int input, int min, int max)
        {
            return Math.Max(Math.Min(input, max), min);
        }
    }
}