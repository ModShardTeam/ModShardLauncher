using System;
using System.Diagnostics;
using Serilog;
using Serilog.Events;

namespace ModShardLauncher
{
    public class Simulation
    {
        public static void MonteCarloEstimation(int n, Action func, bool log = false)
        {
            long sum = 0;
            long sum_sq = 0;
            Stopwatch watch;
            
            if (!log) Main.lls.MinimumLevel = (LogEventLevel) 1 + (int) LogEventLevel.Fatal; // log off
            for(int _ = 0; _ < n; _++)
            {
                watch = Stopwatch.StartNew();
                func();
                watch.Stop();
                long elapsedMs = watch.ElapsedMilliseconds;
                sum += elapsedMs;
                sum_sq += elapsedMs * elapsedMs;
            }
            if (!log) Main.lls.MinimumLevel = LogEventLevel.Information; // log in

            double mean = sum / (double)n;
            double var = sum_sq / (double)(n - 1) - sum * sum / (double)((n - 1) * n);

            Log.Information("MonteCarlo parameter estimated at {{{0}}} +/- {{{1}}} ms", mean, Math.Sqrt(var / n) * 1.96);
        }
    }   
}
        