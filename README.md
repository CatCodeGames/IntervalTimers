# 1. Description
Timers for UniTask. Using UniTask's functionality to integrate with PlayerLoop. Featuring flexible settings and eliminating accumulation of errors in cyclic executions.

### Key Features and Advantages:
- **Accuracy without error accumulation.**\
  Each timer tick accounts for the difference between the scheduled and actual execution times, preventing error accumulation.
- **Configurable number of ticks.**\
 You can set a specific number of timer ticks, such as having the timer run 10 times at one-second intervals and then stop.
- **Choice of game cycle for execution.**\
 The timer can run in various cycles like `Update`, `FixedUpdate`, and others.
- **Custom time function.**\
 You can use your own time function instead of the standard Time.time and Time.deltaTime.
- **Multiple ticks per frame.**\
 For very short intervals, the timer can trigger events multiple times within a single frame.
- **Safe multiple starts and stops.**\
 The timer remains stable even if Start() and Stop() are called multiple times within the same frame.
- **On-the-fly parameter changes.**\
 Parameters such as the interval and number of ticks can be adjusted during the timer's operation.
- **Easy saving and copying.**\
 Internal parameters like elapsed time and the number of ticks are accessible, making it easy to save the state or create copies.

&nbsp;
# 2. Example
An example of a timer that ticks every second and stops after 10 ticks
``` csharp
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
 
 // The timer can be stopped and later resumed.
 timer.Stop();
 
 // Stop and reset the timer
 timer.Reset();
```

&nbsp;
# 3. Timers
## IntervalTimer
Base class for timers.

### Properties
- `Interval` - The time interval after which the timer triggers the `Tick` event.
- `CompletedTicks` - The current number of times the timer has triggered.
- `TotalTicks` - The total number of times the timer will trigger before stopping. `-1` indicates an infinite loop.
- InvokeMode` - Event invocation mode for the `Tick` event. Used when the timer needs to trigger multiple times within a single frame.- `playerLoopTiming` - Determines which game loop (e.g., Update, FixedUpdate, etc.) the timer will use.
- `State` - Timer state.
Properties сontains information about the last trigger of the timer. If the timer can trigger multiple times within a single frame, these properties allow you to determine the number of event invocations and their order.
- `TicksPerFrame` - The number of times the timer triggers within a single frame.
- `TickNumber` - The sequential number of the timer trigger in `InvokeMode.Multi`; always 0 in `InvokeMode.Single`.
### Methods
- `Start()` - Starts or resumes the timer. If it's the first start, parameters are initialized automatically
- `Stop()` - Stops the timer with the option to resume later.
- `Reset()` - Stops the timer and resets its state.


&nbsp;
## 1. DeltaTimeTimer
- Uses the difference in time between frames (`Time.deltaTime` and similar) for its calculations.
- Upon stopping and restarting, continues without accounting for the time when the timer was paused.

### Properties and methods
- `ElapsedTime` - The time since the timer was started or last triggered.
- `SetDeltaTimeFunction(bool unscaled)` - Specifies which type of time the timer will use: `Time.deltaTime` or `Time.unscaledDeltaTime`
- `SetDeltaTimeFunction(Func<float> getDeltaTime)` - Sets a custom function that returns the interval between the current and previous frames.

&nbsp;
## 2. RealtimeTimer 
- Uses game time (`Time.time` and similar, or a user-defined time function).
- Upon stopping and restarting, triggers all missed events.
- In the case of multiple events within a single frame, the timer accurately determines the time of each event using the `LateTime` property.

### Properties and methods
- `LastTime` - The time of the last trigger or when the timer started.
- `SetTimeFunction(RealtimeMode mode)` - Specifies which type of time the timer will use: `Time.time`, `Time.unscaledDeltaTime`, or `Time.realtimeSinceStartup`.
- `SetTimeFunction(Func<float> getTime)` - Sets a custom function that returns the time.

&nbsp;
## 3. DateTimeTimer 
- Uses system time (`DateTime.Now` or a user-defined function).
- Upon stopping and restarting, triggers all missed events.
- In the case of multiple events within a single frame, the timer accurately determines the time of each event using the `LateTime` property.
  
### Properties and methods
- `LastTime` - The time of the last trigger or when the timer started.
- `SetTimeFunction(Func<DateTime> getTime)` - Sets a custom function that returns the time. Default is `DateTime.Now`.

&nbsp;
# 4. Additional Information

## InvokeMode
When the timer's interval is shorter than the frame time, this parameter defines how the timer will trigger events.
### InvokeMode.Multi
In this mode, the `Elapsed` event is called multiple times within a single frame. The `TickData` property provides information on how many times the timer has triggered and the sequence number of the current event.
``` csharp
float interval = 0.005f;
int ticksCount = 5;

var timer = new RealtimeTimer()
{
    Interval = interval,
    TotalTicks = ticksCount,
    InvokeMode = InvokeMode.Multi
};
timer.Started += () => Debug.Log($"Timer started at {Time.time}");
timer.Completed += () => Debug.Log($"Timer finished at {Time.time}");
timer.Tick += () => Debug.Log($"Current time - {Time.time}. Scheduled time - {timer.LastTime}. Ticks - {timer.TickIndex + 1}/{timer.TicksPerFrame}");

timer.Start();
```
Output
```
  Timer started at 19,02731
  Current time - 19,03973. Scheduled time - 19,03231. Ticks - 1/2
  Current time - 19,03973. Scheduled time - 19,03731. Ticks - 2/2
  Current time - 19,05369. Scheduled time - 19,04231. Ticks - 1/3
  Current time - 19,05369. Scheduled time - 19,04731. Ticks - 2/3
  Current time - 19,05369. Scheduled time - 19,05231. Ticks - 3/3
  Timer finished at 19,05369
```
### InvokeMode.Single
In this mode, the `Elapsed` method is called once per frame. The `TickData` properties provide information only about the number of timer triggers.
``` csharp
float interval = 0.005f;
int ticksCount = 5;

var timer = new RealtimeTimer()
{
    Interval = interval,
    TotalTicks = ticksCount,
    InvokeMode = InvokeMode.Single
};
timer.Started += () => Debug.Log($"Timer started at {Time.time}");
timer.Completed += () => Debug.Log($"Timer finished at {Time.time}");
timer.Tick += () => Debug.Log($"Current time - {Time.time}. Scheduled time - {timer.LastTime}. Ticks - {timer.TickIndex + 1}/{timer.TicksPerFrame}");

timer.Start();
```
Output
```
  Timer started at 19,02731
  Current time - 19,03973. Ticks - 2
  Current time - 19,05369. Ticks - 3
  Timer finished at 19,05369
```



&nbsp;
&nbsp;
# 1. Описание 
Таймеры для UniTask. Используют функционал библиотеки UniTask для интеграции в PlayerLoop. Имеют гибкие настройки и не накапливают погрешности в циклических срабатываниях.

### Основные возможности и преимущества:
- **Точность работы без накопления ошибок.**\
  При каждом срабатывании таймера учитывается разница между запланированным и фактическим временем выполнения.
- **Настраиваемое количество срабатываний.**\
  Возможность задать количество срабатываний, например, чтобы таймер отработал 10 раз с интервалом в одну секунду и затем остановился.
- **Выбор игрового цикла для выполнения.**\
 Таймер может работать в Update, FixedUpdate и других циклах.
- **Пользовательская функция времени.**\
 Возможность использовать собственную функцию времени вместо стандартных Time.time, Time.deltaTime и других.
- **Многократные срабатывания за один кадр.**\
 При малых интервалах времени таймер может вызывать события несколько раз за один кадр.
- **Безопасный многократный запуск и остановка.**\
 Таймер остается стабильным при многократных вызовах Start() и Stop() в пределах одного кадра
- **Изменение параметров на лету.**\
 Интервал, количество срабатываний и другие параметры могут быть изменены в процессе работы таймера.
- **Простота сохранения и копирования.**\
 Внутренние параметры таймера, такие как прошедшее время и количество срабатываний, доступны для сохранения и создания копий.

&nbsp;
# 2. Пример
Пример таймера, который срабатывает каждую секунду и останавливается после 10 срабатываний.
``` csharp
 Func<float> getTimeFunc = GameWorldTime.GetTime;
 
 // Создание и инициализация таймера.
 var timer = new RealtimeTimer()
 {
     Interval = 1f,
     TotalTicks = 10
 };
 
 // Установка пользовательской функции времени
 timer.SetTimeFunction(getTimeFunc);
 // Или указание одной из Time.time, Time.unscaledDeltaTime или Time.realtimeSinceStartup
 // По умолчанию Time.time;
 timer.SetTimeFunction(RealtimeMode.Unscaled);
 
 timer.Started += () => Debug.Log("Timer started");
 timer.Completed += () => Debug.Log("Timer finished");
 timer.Tick += () => Debug.Log($"Tick at {Time.time}. Scheduled time - {timer.LastTime}");
 
 // Запуск таймера или продолжение его работы.
 timer.Start();
 
 // Свойства таймера могут быть изменены во время его выполнения.
 timer.Interval /= 2;
 timer.LastTime = GameWorldTime.GetTime() + 10f;
 
 // Остановка таймера с возможностью продолжения его работу позже.
 timer.Stop();
 
 // Остановка и сброс параметров таймера.
 timer.Reset();
```
&nbsp;
# 3. Таймеры
## IntervalTimer
Базовый класс всех таймеров.

### Cвойства

- `Interval` - Интервал времени, по истечению которого таймер вызывает событие Tick.
- `CompletedTicks` - Текущее количество срабатываний таймера.
- `TotalTicks` - Количество раз сколько должен сработать таймер. -1 - бесконечный режим работы таймера.
- `InvokeMode` - Режим вызова события Elapsed. В случае, если таймер должен сработать несколько раз за один кадр.
- `PlayerLoopTimiming` - Определяет, в каком из игровых циклов (например, Update, FixedUpdate и т.д.) будет использоваться таймер.
- `State` - Состояние таймера.
Свойства, содержат информацию о последнем срабатывании таймера. Если таймер может сработать несколько раз в пределах одного кадра, эти свойство позволяет определить количество вызовов события и их порядок.
- `TickIndex` - Порядковый номер срабатывания таймера в режиме InvokeMode.Multi; всегда -1 в режиме InvokeMode.Single
- `TicksPerFrame` - Количество срабатываний таймера в пределах одного кадра.

### Методы
- `Start()` - Запуск таймера или возобновление его работы
- `Stop()` - Остановка таймера с возможностью его последующего возобновления.
- `Reset()` - Остановка таймера и сброс его состояния.

&nbsp;
## 1. DeltaTimeTimer
- Использует разницу времени между кадрами для своих расчётов (Time.deltaTime и другие).
- При остановке и последующем запуске продолжает работу без учета времени, когда таймер был остановлен.

### Свойства и методы
- `ElapsedTime1 - Время с момента запуска или последнего срабатывания таймера.
- `SetDeltaTimeFunction(bool unscaled)` - Позволяет указать какой тип времени использовать для обновления таймера: `Time.deltaTime` или `Time.unscaledDeltaTime`.
- `SetDeltaTimeFunction(Func<float> getDeltaTime)` - Позволяет указать пользовательскую функцию, возвращающая интервал между текущим и предыдущим кадром.

&nbsp;
## 2. RealtimeTimer 
- Использует игровое время (`Time.time` и другие или пользовательскую функцию времени).
- При остановке и последующем запуске вызываются все пропущенные срабатывания.
- В случае многократных срабатываний в пределах одного кадра, таймер позволяет точно определить время каждого срабатывания, используя свойство `LastTime`.

### Свойства и методы
- `LastTime` - Время последнего срабатывания таймера или время его запуска.
- `SetTimeFunction(RealtimeMode mode)` - Позволяет указать какой тип времени используется таймером : `Time.time`, `Time.unscaledDeltaTime` или `Time.realtimeSinceStartup`
- `SetTimeFunction(Func<float> getTime)` - Позволяет указать пользовательскую функцию, возвращающая время.

&nbsp;
## 3. DateTimeTimer 
- Использует системное время (DateTime.Now или пользовательскую функцию).
- При остановке и последующем запуске вызываются все пропущенные срабатывания.
- В случае многократных срабатываний в пределах одного кадра, таймер позволяет точно определить время каждого срабатывания, используя свойство `LastTime`.
  
### Свойства и методы
- `LastTime` - Время последнего срабатывания таймера или время его запуска.
- `SetTimeFunction(Func<DateTime> getTime)` - Позволяет указать пользовательскую функцию, возвращающая время. По умолчанию используется `DateTime.Now`;


&nbsp;
# 4. Дополнительно
## InvokeMode 
Если интервал срабатывания таймера меньше, чем интервал времени между кадрами, то этот параметр определяет как таймер будет вызывать событие.
### InvokeMode.Multi
В этом режиме событие `Elapsed` будет вызываться несколько раз в течении одного кадра. Свойство `TickData` позволяет получить информацию о количестве срабатываний таймера за кадр и номер текущего события.
``` csharp
float interval = 0.005f;
int ticksCount = 5;

var timer = new RealtimeTimer()
{
    Interval = interval,
    TotalTicks = ticksCount,
    InvokeMode = InvokeMode.Multi
};
timer.Started += () => Debug.Log($"Timer started at {Time.time}");
timer.Completed += () => Debug.Log($"Timer finished at {Time.time}");
timer.Tick += () => Debug.Log($"Current time - {Time.time}. Scheduled time - {timer.LastTime}. Ticks - {timer.TickIndex + 1}/{timer.TicksPerFrame}");

timer.Start();
```
Вывод
```
  Timer started at 19,02731
  Current time - 19,03973. Scheduled time - 19,03231. Ticks - 1/2
  Current time - 19,03973. Scheduled time - 19,03731. Ticks - 2/2
  Current time - 19,05369. Scheduled time - 19,04231. Ticks - 1/3
  Current time - 19,05369. Scheduled time - 19,04731. Ticks - 2/3
  Current time - 19,05369. Scheduled time - 19,05231. Ticks - 3/3
  Timer finished at 19,05369
```
### InvokeMode.Single
В этом режиме событие `Elapsed` будет вызываться один раз в течении одного кадра. Свойство `TickData` позволяет получить информацию только о коичестве срабатываний таймера.
``` csharp
float interval = 0.005f;
int ticksCount = 5;

var timer = new RealtimeTimer()
{
    Interval = interval,
    TotalTicks = ticksCount,
    InvokeMode = InvokeMode.Single
};
timer.Started += () => Debug.Log($"Timer started at {Time.time}");
timer.Completed += () => Debug.Log($"Timer finished at {Time.time}");
timer.Tick += () => Debug.Log($"Current time - {Time.time}. Scheduled time - {timer.LastTime}. Ticks - {timer.TickIndex + 1}/{timer.TicksPerFrame}");

timer.Start();
```
Вывод
```
  Timer started at 19,02731
  Current time - 19,03973. Ticks - 2
  Current time - 19,05369. Ticks - 3
  Timer finished at 19,05369
```
