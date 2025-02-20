using System;
using TMPro;
using UnityEngine;
using static CatCode.Timers.TimerTester;

namespace CatCode.Timers
{
    public sealed class ExampleTester : MonoBehaviour
    {
        private RealtimeTimer _realtimeTimer;
        private DeltaTimeTimer _deltaTimeTimer;

        [SerializeField] private Effect _effect;
        [SerializeField] private float _interval;
        [SerializeField] private int _ticks;
        [Space]
        [SerializeField] private TextMeshProUGUI _timeLabel;
        [SerializeField] private TextMeshProUGUI _timerLabel;


        public void Demo()
        {
    Func<float> getTimeFunc = GameWorldTime.GetTime;
    
    // Create and init timer
    var timer = new RealtimeTimer()
    {
        Interval = 1f,
        TotalTicks = 10
    };
    
    // Set custom time function
    timer.SetTimeFunction(getTimeFunc);
    // Or use Time.time, Time.unscaledDeltaTime or Time.realtimeSinceStartup
    // Default is Time.time;
    timer.SetTimeFunction(RealtimeMode.Unscaled);
    
    timer.Started += () => Debug.Log("Timer started");
    timer.Completed += () => Debug.Log("Timer finished");
    timer.Tick += () => Debug.Log($"Tick at {Time.time}. Scheduled time - {timer.LastTime}");
    
    // Start or resume the timer
    timer.Start();
    
    // Timer properties can be changed while running.
    timer.Interval /= 2;
    timer.LastTime = GameWorldTime.GetTime() + 10f;
    
    // Stop the timer, with the ability to resume.
    timer.Stop();
    
    // Stop and reset the timer
    timer.Reset();
        }

        private void Timer_Tick()
        {
            throw new System.NotImplementedException();
        }

        private void Update()
        {
            _timeLabel.text = Time.time.ToString("0.00");
        }

        private void Awake()
        {
            _realtimeTimer = new RealtimeTimer()
            {
                Interval = _interval,
                TotalTicks = _ticks
            };
            _realtimeTimer.Tick += () => _timerLabel.text = _realtimeTimer.LastTime.ToString("0.00");

            _deltaTimeTimer = new DeltaTimeTimer()
            {
                Interval = _interval,
                TotalTicks = _ticks
            };
            _deltaTimeTimer.Tick += OnTick;
        }

        [ContextMenu("Start")]
        public void StartTimer()
        {
            var time = Time.time;
            var remainingTime = time % _interval;

            _realtimeTimer.Reset();
            _realtimeTimer.InvokeMode = InvokeMode.Single;
            _realtimeTimer.Interval = _interval;
            _realtimeTimer.LastTime = time + _interval - remainingTime;
            _realtimeTimer.Start();
            Debug.Log($"Start. Current time - {Time.time}. Scheduled time - {_realtimeTimer.LastTime}");
        }

        [ContextMenu("DeltaTime")]
        public void StartDeltaTimer()
        {
            _deltaTimeTimer.Interval = _interval;
            _deltaTimeTimer.TotalTicks = _ticks;
            _deltaTimeTimer.InvokeMode = InvokeMode.Multi;
            _deltaTimeTimer.Reset();
            _deltaTimeTimer.Start();
        }

        private void OnTick()
        {
            Debug.ClearDeveloperConsole();
            Debug.Log($"elapsed time - {_deltaTimeTimer.ElapsedTime}, time - {Time.time}, diff - {Time.time - _deltaTimeTimer.ElapsedTime}");
            var elapsedtime = _deltaTimeTimer.ElapsedTime;
            var position = new Vector3(_deltaTimeTimer.CompletedTicks, 0, 0);
            var effect = Instantiate(_effect, position, Quaternion.identity);
            effect.Play(elapsedtime, () => Destroy(effect.gameObject));
        }


        [ContextMenu("test")]
        private void Test()
        {
            for (int i = 0; i < _ticks; i++)
            {
                var offset = i * _interval;
                var position = new Vector3(i, 0, 0);
                var effect = Instantiate(_effect, position, Quaternion.identity);
                effect.Play(offset, () => Destroy(effect.gameObject));
            }
        }
    }
}