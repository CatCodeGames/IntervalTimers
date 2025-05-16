using System;
using Cysharp.Threading.Tasks;
using System.Threading;

namespace CatCode.Timers
{
    public static class IntervalTimerUniTaskExtensions
    {
        public static UniTask WaitTickAsync(this IntervalTimer timer, CancellationToken token)
        {
            return WaitEventAsync(h => timer.Tick += h, h => timer.Tick -= h, token);
        }

        public static UniTask WaitCompletedAsync(this IntervalTimer timer, CancellationToken token)
        {
            var timerState = timer.State;
            if (timerState is TimerState.Completed)
                return UniTask.CompletedTask;
            return WaitEventAsync(h => timer.Completed += h, h => timer.Completed -= h, token);
        }

        public static UniTask WaitStartedAsync(this IntervalTimer timer, CancellationToken token)
        {
            var timerState = timer.State;
            if (timerState is TimerState.Active)
                return UniTask.CompletedTask;
            return WaitEventAsync(h => timer.Started += h, h => timer.Started -= h, token);
        }

        public static UniTask WaitStoppedAsync(this IntervalTimer timer, CancellationToken token)
        {
            var timerState = timer.State;
            if (timerState is TimerState.Paused)
                return UniTask.CompletedTask;
            return WaitEventAsync(h => timer.Stopped += h, h => timer.Stopped -= h, token);
        }

        private static UniTask WaitEventAsync(Action<Action> subscribe, Action<Action> unsubscribe, CancellationToken token)
        {
            if (token.IsCancellationRequested)
                return UniTask.FromCanceled(token);

            var tcs = new UniTaskCompletionSource();
            CancellationTokenRegistration ctr = default;
            ctr = token.Register(OnCancel);

            subscribe(Handler);
            return tcs.Task;

            void Handler()
            {
                unsubscribe(Handler);
                tcs.TrySetResult();
                ctr.Dispose();
            }

            void OnCancel()
            {
                unsubscribe(Handler);
                tcs.TrySetCanceled();
                ctr.Dispose();
            }
        }
    }
}