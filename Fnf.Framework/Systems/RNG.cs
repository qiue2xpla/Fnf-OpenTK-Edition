using System;

namespace Fnf.Framework
{
    public static class RNG
    {
        private static Random random = new Random();

        public static int Next(int min_inclusive, int max_exlusive) => random.Next(min_inclusive, max_exlusive);
    }
}
