using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace GPUControl.Util
{
    public static class ProcessMonitor
    {
        public static TimeSpan MaxUpdateInterval = TimeSpan.FromSeconds(5);
        public static List<string> LastProgramNames { get; private set; } = new List<string>();

        private static List<string> GetCurrentProgramNames() =>
            Process.GetProcesses().Select(p => p.ProcessName).Distinct().OrderBy(n => n).ToList();

        public static List<string> CurrentProgramNames()
        {
            LastProgramNames = GetCurrentProgramNames();
            if (CurrentContext != null) CurrentContext.LastRunTime = DateTime.Now;
            return LastProgramNames;
        }

        private class RunContext
        {
            public RunContext(CancellationTokenSource tokenSource)
            {
                TokenSource = tokenSource;
                Token = TokenSource.Token;
            }

            public CancellationTokenSource TokenSource { get; }
            public CancellationToken Token { get; }
            public DateTime? LastRunTime { get; set; } = null;
            public Thread Thread { get; set; }
        }

        private static RunContext? CurrentContext = null;

        public static void Start()
        {
            if (CurrentContext != null) return;

            CurrentContext = new RunContext(new CancellationTokenSource());
            CurrentContext.Thread = new Thread(() =>
            {
                while (!CurrentContext.Token.IsCancellationRequested)
                {
                    var now = DateTime.Now;
                    if (CurrentContext.LastRunTime == null || (now - CurrentContext.LastRunTime) < MaxUpdateInterval)
                    {
                        LastProgramNames = GetCurrentProgramNames();
                        CurrentContext.LastRunTime = now;
                    }

                    try { Task.Delay(MaxUpdateInterval, CurrentContext.Token).Wait(); }
                    catch { }
                }
            });
            CurrentContext.Thread.Start();
        }

        public static void Stop()
        {
            if (CurrentContext == null) return;

            CurrentContext.TokenSource.Cancel();
            CurrentContext.Thread.Join();

            CurrentContext = null;
        }
    }
}
