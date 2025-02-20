using System;
using UnityEngine;
using UnityEngine.UI;

namespace CatCode.Timers
{

    public sealed class TimerTester : MonoBehaviour
    {
        private DeltaTimeTimer _deltaTimeTimer;
        private RealtimeTimer _realtimeTimer;
        private DateTimeTimer _dateTimeTimer;

        [SerializeField] private float _interval = 1;
        [SerializeField] private int _loops = 5;
        [SerializeField] private InvokeMode _multiInvoke = InvokeMode.Single;
        [SerializeField] private bool _delta;
        [SerializeField] private bool _realtime;
        [SerializeField] private bool _dateTime;
        [Space]
        [SerializeField] private TimerState _deltaTimerState;
        [SerializeField] private TimerState _realtimeTimerState;
        [SerializeField] private TimerState _dateTimeTimerState;
        [Space]
        [SerializeField] private Button _startButton;
        [SerializeField] private Button _stopButton;
        [SerializeField] private Button _resetButton;

        public sealed class GameWorldTime
        {
            public static float GetDeltaTime() => 0;
            public static float GetTime() => 0;
        }



        private void Awake()
        {
            _startButton.onClick.AddListener(StartTimers);
            _stopButton.onClick.AddListener(StopTimers);
            _resetButton.onClick.AddListener(ResetTimers);

            _deltaTimeTimer = new DeltaTimeTimer();
            _deltaTimeTimer.Tick += () =>
            {
                var timer = _realtimeTimer;
                Debug.Log($"Elapsed at {Time.time}. Ticks {timer.TickIndex + 1}/{timer.TicksPerFrame}");
            };
            _deltaTimeTimer.Started += () => { Debug.Log("DeltaTime Started - " + Time.time); };
            _deltaTimeTimer.Stopped += () => { Debug.Log("DeltaTime Stopped - " + Time.time); };
            _deltaTimeTimer.Completed += () => { Debug.Log("DeltaTime Completed - " + Time.time); };

            _realtimeTimer = new RealtimeTimer();
            _realtimeTimer.Tick += () =>
            {
                var timer = _realtimeTimer;
                Debug.Log($"Current time - {Time.time}. Scheduled time - {timer.LastTime}. Ticks {timer.TickIndex + 1}/{timer.TicksPerFrame}");
            };
            _realtimeTimer.Started += () => { Debug.Log("Realtime Started - " + Time.time); };
            _realtimeTimer.Stopped += () => { Debug.Log("Realtime Stopped - " + Time.time); };
            _realtimeTimer.Completed += () => { Debug.Log("Realtime Completed - " + Time.time); };

            _dateTimeTimer = new DateTimeTimer();
            _dateTimeTimer.Tick += () =>
            {
                var timer = _dateTimeTimer;
                Debug.Log($"Current time - {Time.time}. Scheduled time - {timer.LastTime}. Ticks {timer.TickIndex + 1}/{timer.TicksPerFrame}");
            };
            _dateTimeTimer.Started += () => { Debug.Log("DateTime Started - " + DateTime.Now.ToString("mm:ss.fff")); };
            _dateTimeTimer.Stopped += () => { Debug.Log("DateTime Stopped - " + DateTime.Now.ToString("mm:ss.fff")); };
            _dateTimeTimer.Completed += () => { Debug.Log("DateTime Completed - " + DateTime.Now.ToString("mm:ss.fff")); };

        }

        private void StartTimers()
        {
            if (_delta)
            {
                _deltaTimeTimer.Interval = _interval;
                _deltaTimeTimer.TotalTicks = _loops;
                _deltaTimeTimer.InvokeMode = _multiInvoke;
                _deltaTimeTimer.Start();
            }

            if (_realtime)
            {
                _realtimeTimer.Interval = _interval;
                _realtimeTimer.TotalTicks = _loops;
                _realtimeTimer.InvokeMode = _multiInvoke;
                _realtimeTimer.Start();
            }

            if (_dateTime)
            {
                _dateTimeTimer.Interval = TimeSpan.FromSeconds(_interval);
                _dateTimeTimer.TotalTicks = _loops;
                _dateTimeTimer.InvokeMode = _multiInvoke;
                _dateTimeTimer.Start();
            }
        }

        private void StopTimers()
        {
            _deltaTimeTimer.Stop();
            _realtimeTimer.Stop();
            _dateTimeTimer.Stop();
        }

        private void ResetTimers()
        {
            _deltaTimeTimer.Reset();
            _realtimeTimer.Reset();
            _dateTimeTimer.Reset();
        }

        private void Update()
        {
            _deltaTimerState = _deltaTimeTimer.State;
            _realtimeTimerState = _realtimeTimer.State;
            _dateTimeTimerState = _dateTimeTimer.State;
        }
    }
}