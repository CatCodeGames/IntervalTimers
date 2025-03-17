using System;
using UnityEngine;
using UnityEngine.UI;

namespace CatCode.Timers
{
    public sealed class TimerTester : MonoBehaviour
    {
        private int _totalTicks;
        private float _startTime;
        private DateTime _startDateTime;

        private DeltaTimeTimer _deltaTimeTimer;
        private RealtimeTimer _realtimeTimer;
        private DateTimeTimer _dateTimeTimer;

        [SerializeField] private float _interval = 1;
        [SerializeField] private int _loops = 5;
        [SerializeField] private TimerMode _mode = TimerMode.Single;
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


        private void Awake()
        {
            _startButton.onClick.AddListener(StartTimers);
            _stopButton.onClick.AddListener(StopTimers);
            _resetButton.onClick.AddListener(ResetTimers);

            _deltaTimeTimer = new DeltaTimeTimer();
            _deltaTimeTimer.Tick += () =>
            {
                var timer = _deltaTimeTimer;
                _totalTicks = timer.Mode == TimerMode.Single
                    ? _totalTicks + timer.FrameTicksCount
                    : _totalTicks + 1;
                Debug.Log($"CT - {Time.time:0.000}. ST - {(Time.time - timer.ElapsedTime).ToString("0.000")}. Ticks - {timer.FrameTicksCount}");
            };
            _deltaTimeTimer.Started += () =>
            {
                _totalTicks = 0;
                _startTime = Time.time;
                Debug.Log("DeltaTime Started - " + Time.time.ToString("0.000"));
            };
            _deltaTimeTimer.Stopped += () => { Debug.Log("DeltaTime Stopped - " + Time.time.ToString("0.000")); };
            _deltaTimeTimer.Completed += () =>
            {
                var timer = _deltaTimeTimer;
                var rt = (Time.time - _startTime);
                var st = (Time.time - _startTime - timer.ElapsedTime);
                Debug.Log($"DeltaTime Completed - {Time.time:0.000}. RT - {rt:0.000}. ST - {st:0.000}. TotalTicks - {_totalTicks}");
            };


            _realtimeTimer = new RealtimeTimer();
            _realtimeTimer.Tick += () =>
            {
                var timer = _realtimeTimer;
                _totalTicks = timer.Mode == TimerMode.Single
                    ? _totalTicks + timer.FrameTicksCount
                    : _totalTicks + 1;
                Debug.Log($"CT - {Time.time:0.000}. ST - {timer.LastTime:0.000}. Ticks - {timer.FrameTicksCount}");
            };
            _realtimeTimer.Started += () =>
            {
                var timer = _realtimeTimer;
                _totalTicks = 0;
                _startTime = Time.time;
                Debug.Log("Realtime Started - " + Time.time.ToString("0.000"));
            };
            _realtimeTimer.Stopped += () => { Debug.Log("Realtime Stopped - " + Time.time.ToString("0.000")); };
            _realtimeTimer.Completed += () =>
            {
                var timer = _realtimeTimer;
                var rt = (Time.time - _startTime);
                var st = (timer.LastTime - _startTime);
                Debug.Log($"Realtime Completed - {Time.time:0.000}. RT - {rt:0.000}. ST - {st:0.000}. TotalTicks - {_totalTicks}");
            };

            _dateTimeTimer = new DateTimeTimer();
            _dateTimeTimer.Tick += () =>
            {
                var timer = _dateTimeTimer;
                _totalTicks = timer.Mode == TimerMode.Single
                    ? _totalTicks + timer.FrameTicksCount
                    : _totalTicks + 1;
                Debug.Log($"CT - {Time.time:0.000}. ST - {timer.LastTime:mm:ss.fff}. Ticks - {timer.FrameTicksCount}");
            };
            _dateTimeTimer.Started += () =>
            {
                _totalTicks = 0;
                _startDateTime = DateTime.Now;
                Debug.Log("DateTime Started - " + DateTime.Now.ToString("mm:ss.fff"));
            };
            _dateTimeTimer.Stopped += () => { Debug.Log("DateTime Stopped - " + DateTime.Now.ToString("mm:ss.fff")); };
            _dateTimeTimer.Completed += () =>
            {
                Debug.Log("DateTime Completed - " + DateTime.Now.ToString("mm:ss.fff"));
                var timer = _dateTimeTimer;
                var rt = (DateTime.Now - _startDateTime);
                var st = (timer.LastTime - _startDateTime);
                Debug.Log($"DateTime Completed - {DateTime.Now:mm:ss.fff}. RT - {rt:mm\\:ss\\.fff}. ST - {st:mm\\:ss\\.fff}. TotalTicks - {_totalTicks}");
            };

        }

        [ContextMenu("Start Timers")]
        private void StartTimers()
        {
            Debug.ClearDeveloperConsole();

            if (_delta)
            {
                _deltaTimeTimer.Interval = _interval;
                _deltaTimeTimer.TotalTicks = _loops;
                _deltaTimeTimer.Mode = _mode;
                _deltaTimeTimer.Start();
            }

            if (_realtime)
            {
                _realtimeTimer.Interval = _interval;
                _realtimeTimer.TotalTicks = _loops;
                _realtimeTimer.Mode = _mode;
                _realtimeTimer.Start();
            }

            if (_dateTime)
            {
                _dateTimeTimer.Interval = TimeSpan.FromSeconds(_interval);
                _dateTimeTimer.TotalTicks = _loops;
                _dateTimeTimer.Mode = _mode;
                _dateTimeTimer.Start();
            }
        }

        [ContextMenu("Stop Timers")]
        private void StopTimers()
        {
            _deltaTimeTimer.Stop();
            _realtimeTimer.Stop();
            _dateTimeTimer.Stop();
        }

        [ContextMenu("ResetTimers")]
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