using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace GPUControl
{
    public static class ProcessMonitor
    {
        public static TimeSpan UpdateInterval = TimeSpan.FromSeconds(5);

        public static List<string>? ProgramNames = new List<string>();

        private static CancellationTokenSource? CancelTokenSource = null;

        public static void Start()
        {
            if (CancelTokenSource != null) return;

            CancelTokenSource = new CancellationTokenSource();
            new Thread(() =>
            {
                var token = CancelTokenSource.Token;
                while (!token.IsCancellationRequested)
                {
                    var processes = Process.GetProcesses();
                    ProgramNames = processes.Select(p => p.ProcessName).Distinct().OrderBy(n => n).ToList();

                    try
                    {
                        Task.Delay(UpdateInterval, token).Wait();
                    }
                    catch
                    {
                        break;
                    }
                }
            }).Start();
        }

        public static void Stop()
        {
            if (CancelTokenSource == null) return;
            CancelTokenSource.Cancel();

            CancelTokenSource = null;
        }
    }
}
