using GPUControl.Lib.GPU;
using GPUControl.Lib.Model;
using GPUControl.Model;
using GPUControl.Overclock.Result;
using GPUControl.ViewModels;
using NvAPIWrapper.GPU;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace GPUControl.Overclock
{
    public static class OverclockManager
    {
        public static TimeSpan UpdateInterval = TimeSpan.FromSeconds(5);

        private class Context
        {
            public CancellationTokenSource TokenSource { get; set; }
            public Thread CurrentThread { get; set; }
            public bool Paused { get; set; }
        }

        private static Context? currentContext = null;

        public static event Action<IBehaviorResult> BehaviorApplied;
        public static event Action ManagerStateChanged;
        public static event Action<Exception> UnexpectedError;

        public static bool IsEnabled => currentContext != null;

        public static bool IsRunning => IsEnabled && !currentContext!.Paused;

        public static IBehaviorResult? LastResult { get; private set; }
        public static IOverclockBehavior? CurrentBehavior { get; set; }

        public static void Start(List<IGpuWrapper> gpus)
        {
            if (currentContext != null) return;

            currentContext = new Context { TokenSource = new CancellationTokenSource(), Paused = false };

            currentContext.CurrentThread = new Thread(() =>
            {
                var logger = Serilog.Log.ForContext(typeof(OverclockManager));
                var token = currentContext.TokenSource.Token;
                while (!token.IsCancellationRequested)
                {
                    if (CurrentBehavior != null && !currentContext.Paused)
                    {
                        var task = CurrentBehavior.Apply(gpus);

                        try { task.Wait(token); }
                        catch (Exception ex) when (ex is OperationCanceledException) { }
                        catch (Exception ex)
                        {
                            LastResult = null;
                            currentContext.Paused = true;
                            ManagerStateChanged?.Invoke();
                            UnexpectedError?.Invoke(ex);
                        }

                        if (task.IsCompletedSuccessfully)
                        {
                            var result = task.Result;
                            LastResult = result;
                            BehaviorApplied?.Invoke(result);
                        }
                    }

                    try { Task.Delay(UpdateInterval, token).Wait(); }
                    catch { break; }
                }
            });

            currentContext.CurrentThread.Start();
            ManagerStateChanged?.Invoke();
        }

        public static void Resume()
        {
            if (currentContext != null)
            {
                currentContext.Paused = false;
                ManagerStateChanged?.Invoke();
            }
        }

        public static void Pause()
        {
            if (currentContext != null)
            {
                currentContext.Paused = true;
                ManagerStateChanged?.Invoke();
            }
        }

        public static void Stop()
        {
            if (currentContext == null) return;

            currentContext.TokenSource.Cancel();
            currentContext.CurrentThread.Join();
            ManagerStateChanged?.Invoke();
        }
    }
}
