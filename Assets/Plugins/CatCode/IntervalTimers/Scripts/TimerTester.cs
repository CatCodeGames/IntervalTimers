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

        private float _elapsed;

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

        private void TestDeltaTimer()
        {
    float interval = 0.001f;
    int loopCount = 5;
    
    var timer = RealtimeTimer.Create(interval, loopCount, invokeMode: InvokeMode.Multi);
    timer.Started += ()=> Debug.Log($"Timer started at {Time.time}");
    timer.Completed += () => Debug.Log($"Timer finished at {Time.time}");
    timer.Elapsed += () => Debug.Log($"Current time - {Time.time}. Scheduled time - {timer.Data.LastTime}. Ticks {timer.TickData.TickNumber + 1}/{timer.TickData.TicksPerFrame}");
    timer.Start();
        }

        private void Start()
        {
        }


        private void Awake()
        {
            QualitySettings.vSyncCount = 1;

            _startButton.onClick.AddListener(StartTimers);
            _stopButton.onClick.AddListener(StopTimers);
            _resetButton.onClick.AddListener(ResetTimers);

            _deltaTimeTimer = DeltaTimeTimer.Create(_interval, _loops, _multiInvoke);
            _deltaTimeTimer.Elapsed += () =>
            {
                Debug.Log($"Elapsed at {Time.time}. Ticks {_deltaTimeTimer.TickData.TickNumber}/{_deltaTimeTimer.TickData.TicksPerFrame}");

                //Debug.Log($"DeltaTime - {Time.time}" +
                //$"; Elapsed - {_deltaTimeTimer.Data.ElapsedTime}" +
                //$"; TicksPerFrame - {_deltaTimeTimer.TickData.TickNumber}/{_deltaTimeTimer.TickData.TicksPerFrame}");
            };
            _deltaTimeTimer.Started += () => { Debug.Log("DeltaTime Started - " + Time.time); };
            _deltaTimeTimer.Stopped += () => { Debug.Log("DeltaTime Stopped - " + Time.time); };
            _deltaTimeTimer.Completed += () => { Debug.Log("DeltaTime Completed - " + Time.time); };

            _realtimeTimer = RealtimeTimer.Create(_interval, _loops, _multiInvoke);
            _realtimeTimer.Elapsed += () =>
            {
                var timer = _realtimeTimer;
                Debug.Log($"Current time - {Time.time}. Scheduled time - {timer.Data.LastTime}. Ticks {timer.TickData.TickNumber}/{timer.TickData.TicksPerFrame}");
                //Debug.Log($"Realtime - {Time.time}" +
                //$"; LastTime - {_realtimeTimer.Data.LastTime}" +
                //$"; TicksPerFrame - {_realtimeTimer.TickData.TickNumber}/{_realtimeTimer.TickData.TicksPerFrame}");
            };
            _realtimeTimer.Started += () => { Debug.Log("Realtime Started - " + Time.time); };
            _realtimeTimer.Stopped += () => { Debug.Log("Realtime Stopped - " + Time.time); };
            _realtimeTimer.Completed += () => { Debug.Log("Realtime Completed - " + Time.time); };

            _dateTimeTimer = DateTimeTimer.Create(TimeSpan.FromSeconds(_interval), _loops, _multiInvoke);
            _dateTimeTimer.Elapsed += () =>
            {
                Debug.Log($"DateTime - {DateTime.Now:mm:ss.fff}" +
                $"; LastTime - {_dateTimeTimer.Data.LastTime:mm:ss.fff}" +
                $"; TicksPerFrame - {_dateTimeTimer.TickData.TickNumber}/{_dateTimeTimer.TickData.TicksPerFrame}");
            };
            _dateTimeTimer.Started += () => { Debug.Log("DateTime Started - " + DateTime.Now.ToString("mm:ss.fff")); };
            _dateTimeTimer.Stopped += () => { Debug.Log("DateTime Stopped - " + DateTime.Now.ToString("mm:ss.fff")); };
            _dateTimeTimer.Completed += () => { Debug.Log("DateTime Completed - " + DateTime.Now.ToString("mm:ss.fff")); };

        }

        private void Timer_Completed()
        {
            throw new NotImplementedException();
        }

        private void StartTimers()
        {
            if (_delta)
            {
                var deltaTimeData = _deltaTimeTimer.Data;
                deltaTimeData.Interval = _interval;
                deltaTimeData.TotalTicks = _loops;
                _deltaTimeTimer.InvokeMode = _multiInvoke;
                _deltaTimeTimer.Start();
            }

            if (_realtime)
            {
                var realtimeData = _realtimeTimer.Data;
                realtimeData.Interval = _interval;
                realtimeData.TotalTicks = _loops;
                _realtimeTimer.InvokeMode = _multiInvoke;
                _realtimeTimer.Start();
            }

            if (_dateTime)
            {
                var dateTimeData = _dateTimeTimer.Data;
                dateTimeData.Interval = TimeSpan.FromSeconds(_interval);
                dateTimeData.TotalTicks = _loops;
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


            if (_elapsed > _interval)
                return;
            _elapsed += Time.deltaTime;
            //Debug.Log("Frame : " + Time.frameCount);
            //Debug.Log("Time : " + Time.time);
            //Debug.Log("Local Elapsed: " + _elapsed);
            //Debug.Log("Timer Elapsed: " + _timer.Data.ElapsedTime);
        }
    }
}