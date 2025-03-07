using CatCode.Timers;
using System;
using System.Collections;
using UnityEngine;

public sealed class Tester : MonoBehaviour
{
    private DateTime _lastTime2;
    [SerializeField] private float _seconds;
    private void Update()
    {
        var dateTime = DateTime.Now;
        var interval = TimeSpan.FromSeconds(_seconds);
        long ticks = (dateTime.Ticks / interval.Ticks) * interval.Ticks;
        var time = new DateTime(ticks);
        if (time > _lastTime2)
        {
            _lastTime2 = time;
            Debug.Log($"Time - {dateTime:mm.ss.fff}, LastTime - {_lastTime:mm.ss.fff}");
        }

        StartCoroutine(Routine());
    }

    private float _interval = 1f;
    private int _count;
    private Action _onTick;
    private void OnTick() { }

    private IEnumerator Routine()
    {
        yield return new WaitForSeconds(_interval);
        OnTick();
    }

    private IEnumerator TimerRoutine()
    {
        {
            yield return new WaitForSeconds(_interval);
            OnTick();
        }
    }

    private void InvokeAndInvokeRepeating()
    {
        Invoke("OnTick", _interval);
        InvokeRepeating("OnTick", _interval, _interval);
    }

    private float _elapsedTime;

    private void DeltaUpdate1()
    {
        _elapsedTime += Time.deltaTime;
        if (_elapsedTime >= _interval)
        {
            OnTick();
        }
    }


    RealtimeTimer _timer = new RealtimeTimer();
    RealtimeTimer _timerCopy = new RealtimeTimer();

    private void Example()
    {
        _timerCopy.Interval = _timer.Interval;
        _timerCopy.LastTime = _timer.LastTime;

        _timerCopy.Stop();

        var time = Time.time;
        var remainingTime = time % _interval;
        _timer.LastTime = time - remainingTime;
        _timer.Start();
    }

    public float ElapsedTime { get; set; }
    public int TicksCount { get; private set; }
    private void DeltaUpdate2()
    {
        _elapsedTime += Time.deltaTime;
        if (_elapsedTime >= _interval)
        {
            _elapsedTime = 0;
            OnTick();
        }
    }

    private void DeltaUpdate3()
    {
        _elapsedTime += Time.deltaTime;
        if (_elapsedTime >= _interval)
        {
            _elapsedTime -= _interval;
            OnTick();
        }
    }

    private void DeltaUpdate4()
    {
        _elapsedTime += Time.deltaTime;
        if (_elapsedTime >= _interval)
        {
            var ticks = Mathf.FloorToInt(_elapsedTime / _interval);
            _elapsedTime -= _interval * ticks;
            OnTick();
        }
    }


    private void DeltaUpdate45()
    {
        _elapsedTime += Time.deltaTime;
        if (_elapsedTime >= _interval)
        {
            TicksCount = Mathf.FloorToInt(_elapsedTime / _interval);
            _elapsedTime -= _interval * TicksCount;
            OnTick();
        }
    }

    private void DeltaUpdate46()
    {
        ElapsedTime += Time.deltaTime;
        if (ElapsedTime >= Interval)
        {
            TicksCount = Mathf.FloorToInt(ElapsedTime / Interval);
            ElapsedTime -= Interval * TicksCount;
            OnTick();
        }
    }
    private void DeltaUpdate5()
    {
        _elapsedTime += Time.deltaTime;
        if (_elapsedTime >= _interval)
        {
            var ticks = Mathf.FloorToInt(_elapsedTime / _interval);
            for (int i = 0; i < ticks; i++)
            {
                _elapsedTime -= _interval;
                OnTick();
            }
        }
    }

    private void DeltaUpdate6()
    {
        _elapsedTime += Time.deltaTime;
        while (_elapsedTime >= _interval)
        {
            _elapsedTime -= _interval;
            OnTick();
        }
    }

    private void DeltaUpdate65()
    {
        ElapsedTime += Time.deltaTime;
        while (ElapsedTime >= Interval)
        {
            ElapsedTime -= Interval;
            OnTick();
        }
    }

    private Func<float> _getDeltaTime;
    private void DeltaUpdate7()
    {
        var deltaTime = _getDeltaTime();
        _elapsedTime += deltaTime;
        while (_elapsedTime >= _interval)
        {
            _elapsedTime -= _interval;
            OnTick();
        }
    }


    private float _lastTime;
    private void RealUpdate1()
    {
        if (Time.time >= _lastTime + _interval)
        {
            OnTick();
        }
    }

    private void RealUpdate2()
    {
        if (Time.time >= _lastTime + _interval)
        {
            _lastTime = Time.time;
            OnTick();
        }
    }

    private void RealUpdate3()
    {
        if (Time.time >= _lastTime + _interval)
        {
            _lastTime += _interval;
            OnTick();
        }
    }

    private void RealUpdate4()
    {
        if (Time.time >= _lastTime + _interval)
        {
            var elapsedTime = Time.time - _lastTime;
            var ticks = Mathf.FloorToInt(_elapsedTime / _interval);
            _lastTime += _interval * ticks;
            OnTick();
        }
    }


    private void RealUpdate45()
    {
        if (Time.time >= _lastTime + _interval)
        {
            var elapsedTime = Time.time - _lastTime;
            TicksCount = Mathf.FloorToInt(_elapsedTime / _interval);
            _lastTime += _interval * TicksCount;
            OnTick();
        }
    }

    private void RealUpdate46()
    {
        if (Time.time >= LastTime + Interval)
        {
            var elapsedTime = Time.time - LastTime;
            TicksCount = Mathf.FloorToInt(_elapsedTime / Interval);
            LastTime += Interval * TicksCount;
            OnTick();
        }
    }
    private void RealUpdate5()
    {
        if (Time.time >= _lastTime + _interval)
        {
            var elapsedTime = Time.time - _lastTime;
            var ticks = Mathf.FloorToInt(_elapsedTime / _interval);

            for (int i = 0; i < ticks; i++)
            {
                _lastTime += _interval;
                OnTick();
            }
        }
    }

    public float LastTime { get; set; }
    public float Interval { get; set; }
    private void RealUpdate6()
    {
        var scheduledTime = _lastTime + _interval;

        while (Time.time > scheduledTime)
        {
            _lastTime = scheduledTime;
            scheduledTime += _interval;
            OnTick();
        }
    }

    private void RealUpdate65()
    {
        var scheduledTime = LastTime + Interval;

        while (Time.time > scheduledTime)
        {
            LastTime = scheduledTime;
            scheduledTime += Interval;
            OnTick();
        }
    }


    private Func<float> GetTime;
    private void RealUpdate7()
    {
        var time = GetTime();
        var scheduledTime = _lastTime + _interval;

        while (time > scheduledTime)
        {
            _lastTime = scheduledTime;
            scheduledTime += _interval;
            OnTick();
        }
    }


    class EffectController
    {
        public void SpawnEffect(float t = 0) { }
    }
    private void Temp()
    {
        var effectController = new EffectController();


        var timer = new DeltaTimeTimer();
        

        timer.Interval = 0.01f;
        timer.Tick += () =>
        {
            var timeOffset = timer.ElapsedTime;
            effectController.SpawnEffect(timeOffset);
        };
        timer.Start();




        timer.Interval = 0.01f;
        timer.Tick += () =>
        {
            effectController.SpawnEffect();
        };
        timer.Start();
    }

    private void Temp2() {
        var timer = new RealtimeTimer();


        timer.SetTimeFunction(() => Time.time);
        timer.SetTimeFunction(()=> Time.unscaledDeltaTime);

    }

    private void Temp3()
    {
        var timer = new RealtimeTimer();

        timer.Tick += () =>
        {
            for (int i = 0; i < timer.TicksCount; i++)
                DoSomething();
        };

        void DoSomething() { }
    }
}
