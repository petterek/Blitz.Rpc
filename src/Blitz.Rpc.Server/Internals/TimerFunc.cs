namespace Blitz.Rpc.HttpServer.Internals
{
    static class TimerFunc
    {
        [System.Runtime.InteropServices.DllImport("Kernel32.dll")]
        public static extern bool QueryPerformanceCounter(out long perfcount);

        [System.Runtime.InteropServices.DllImport("Kernel32.dll")]
        public static extern bool QueryPerformanceFrequency(out long freq);


        internal class InternalTimer
        {
            long StartTime;
            static long ticksPrSecond = QueryPerformanceFrequency() /(1000*1000) ; //nano seconds
            public InternalTimer()
            {
                StartTime = QueryPerformanceCounter();
            }

            public long Elapsed()
            {
                return (QueryPerformanceCounter() - StartTime) /ticksPrSecond;
            }
        }


        #region Query Performance Counter
        /// <summary>
        /// Gets the current 'Ticks' on the performance counter
        /// </summary>
        /// <returns>Long indicating the number of ticks on the performance counter</returns>
        internal static long QueryPerformanceCounter()
        {
            long perfcount;
            QueryPerformanceCounter(out perfcount);
            return perfcount;
        }
        #endregion

        #region Query Performance Frequency
        /// <summary>
        /// Gets the number of performance counter ticks that occur every second
        /// </summary>
        /// <returns>The number of performance counter ticks that occur every second</returns>
        internal static long QueryPerformanceFrequency()
        {
            long freq;
            QueryPerformanceFrequency(out freq);
            return freq;
        }
        #endregion
    }
}