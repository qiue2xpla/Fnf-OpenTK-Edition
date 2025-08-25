using System.Diagnostics;

namespace Fnf.Framework
{
    /// <summary>
    /// Keeps track of the time and change in time 
    /// </summary>
    public static class Time
    {
        /// <summary>
        /// Time taken for a full update cycle
        /// </summary>
        public static float deltaTime { get; private set; } = 0;

        /// <summary>
        /// Total time passed
        /// </summary>
        public static float time { get; private set; } = 0;

        private static Stopwatch timer = new Stopwatch();
        private static float old = 0;

        /// <summary>
        /// Starts the timer
        /// </summary>
        internal static void Start()
        {
            timer.Start();
        }

        /// <summary>
        /// Updates the change in time
        /// </summary>
        internal static void Update()
        {
            time = (float)timer.Elapsed.TotalSeconds;
            deltaTime = time - old;
            old = time;
        }

        /// <summary>
        /// Resets the timer to 0
        /// </summary>
        public static void ResetTime()
        {
            timer.Restart();
            old = 0;
        }
    }
}