# 1. Description
Timers for UniTask. Using UniTask's functionality to integrate with PlayerLoop. Featuring flexible settings and eliminating accumulation of errors in cyclic executions.\

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
# 2. Timers

## 1. DeltaTimeTimer
- Uses the difference in time between frames (`Time.deltaTime` and similar) for its calculations.
- Upon stopping and restarting, continues without accounting for the time when the timer was paused.

### Creation
``` csharp
public static DeltaTimeTimer Create( float interval, int loopCount, InvokeMode invokeMode, bool unscaledTime, PlayerLoopTiming playerLoopTiming) 
```
  - `interval` - The time interval after which the timer triggers the Elapsed event.
  - `loopCount` - The number of times the timer should trigger the Elapsed event. -1 indicates an infinite loop.
  - `invokeMode` - Determines whether the timer can trigger the Elapsed event multiple times within a single frame or only once.
  - `unscaledTime` - Specifies whether to use Time.unscaledDeltaTime or Time.deltaTime for time calculation.
  - `playerLoopTiming` - Determines which game loop (e.g., Update, FixedUpdate, etc.) the timer will use.

``` csharp
public static DeltaTimeTimer Create( float interval, Func<float> getDeltaTime, int loopCount, InvokeMode invokeMode, PlayerLoopTiming playerLoopTiming)
```
- `getDeltaTime` - A custom function that returns the interval between the current and previous frame.

### Properties
#### Data
Contains all the working parameters of the timer.
- `ElapsedTime` - The time since the timer was started or last triggered.
- `Interval` - The time interval after which the timer triggers the Elapsed event.
- `CompletedTicks` - The current number of times the timer has triggered.
- `TotalTicks` - The number of times the timer should trigger. -1 indicates an infinite loop.

#### TickData
Contains information about the last trigger of the timer. If the timer can trigger multiple times within a single frame, this property determines the number of event calls and their order.
- `TicksPerFrame` - The number of times the timer triggers within a single frame.
- `TickNumber` - The sequential number of the timer trigger in InvokeMode.Multi; always 0 in InvokeMode.Single.


&nbsp;
## 2. RealtimeTimer
- Uses game time (`Time.time` and similar, or a user-defined time function).
- Upon stopping and restarting, triggers all missed events.
- In the case of multiple events within a single frame, the timer accurately determines the time of each event using the Date.Time property.

### Creation
``` csharp
public static RealtimeTimer Create(float interval, int loopsCount, InvokeMode invokeMode, RealtimeMode timeMode, PlayerLoopTiming playerLoopTiming)
```
- `interval` - The time interval after which the timer triggers the `Elapsed` event.
- `loopCount` - The number of times the timer should trigger the `Elapsed` event. `-1` indicates an infinite loop.
- `invokeMode` - Determines whether the timer can trigger the Elapsed event multiple times within a single frame or only once.
- `timeMode` - Specifies the type of time used: `Time.unscaledTime`, `Time.time`, or `Time.realtimeSinceStartup`.
- `playerLoopTiming` - Determines which game loop (e.g., Update, FixedUpdate, etc.) the timer will use.

``` csharp
public static RealtimeTimer Create(float interval, Func<float> getTime, int loopsCount, InvokeMode invokeMode, PlayerLoopTiming playerLoopTiming)
```
- `getDeltaTime`: A custom function that returns the interval between the current and previous frame.

### Properties
#### Data
Contains all the working parameters of the timer.
- `LastTime` - The time of the last trigger or when the timer started.
- `Interval` - The time interval after which the timer triggers the `Elapsed` event.
- `CompletedTicks` - The current number of times the timer has triggered.
- `TotalTicks` - The total number of times the timer will trigger before stopping. `-1` indicates an infinite loop.

#### TickData
Contains information about the last trigger of the timer. If the timer can trigger multiple times within a single frame, this property determines the number of event calls and their order.
- `TicksPerFrame` - The number of times the timer triggers within a single frame.
- `TickNumber` - The sequential number of the timer trigger in InvokeMode.Multi; always 0 in InvokeMode.Single.


&nbsp;
## 3. DateTimeTimer
- Uses system time (`DateTime.Now` or a user-defined function).
- Upon stopping and restarting, triggers all missed events.
- In the case of multiple events within a single frame, the timer accurately determines the time of each event using the Date.Time property.
- 
### Creation
``` csharp
public static DateTimeTimer Create(TimeSpan interval, int loopsCount, InvokeMode invokeMode, PlayerLoopTiming playerLoopTiming)
```
- `interval` - The time interval after which the timer triggers the `Elapsed` event.
- `loopCount` - The number of times the timer should trigger the `Elapsed` event. `-1` indicates an infinite loop.
- `invokeMode` - Determines whether the timer can trigger the Elapsed event multiple times within a single frame or only once.
- `playerLoopTiming` - Determines which game loop (e.g., Update, FixedUpdate, etc.) the timer will use.

``` csharp
public static DateTimeTimer Create(TimeSpan interval, Func<DateTime> getTime, int loopsCount, InvokeMode invokeMode, PlayerLoopTiming playerLoopTiming)
```
- `getDeltaTime`: A custom function that returns the interval between the current and previous frame.

### Properties
#### Data
Contains all the working parameters of the timer.
- `LastTime` - The time of the last trigger or when the timer started.
- `Interval` - The time interval after which the timer triggers the `Elapsed` event.
- `CompletedTicks` - The current number of times the timer has triggered.
- `TotalTicks` - The total number of times the timer will trigger before stopping. `-1` indicates an infinite loop.

#### TickData
Contains information about the last trigger of the timer. If the timer can trigger multiple times within a single frame, this property determines the number of event calls and their order.
- `TicksPerFrame` - The number of times the timer triggers within a single frame.
- `TickNumber` - The sequential number of the timer trigger in InvokeMode.Multi; always 0 in InvokeMode.Single.


&nbsp;
# 3. Examples
An example of a timer that ticks every second and stops after 10 ticks
``` csharp
    float interval = 1f;
    int loopCount = 10;
    // Custom time function for example.
    Func<float> getTime = GameWorldTime.GetTime;
    InvokeMode invokeMode = InvokeMode.Multi;
    
    // Create timer
    var timer = RealtimeTimer.Create(interval, getTime, loopCount, invokeMode);
    
    timer.Elapsed += () => { Debug.Log($"Elapsed at realtime {Time.time}. Calculate time - {timer.Data.LastTime}"); };
    timer.Started += () => Debug.Log("Timer started");
    timer.Stopped += () => Debug.Log("Timer stopped");
    timer.Completed += () => Debug.Log("Tier finished");
    
    // Start or resume the timer
    timer.Start();
    
    // Modify timer properties
    timer.Data.Interval = interval * 2;
    timer.Data.LastTime = GameWorldTime.GetTime() + 10;
    
    // Stop the timer, with the ability to resume.
    timer.Stop();
    
    // Stop and reset the timer
    timer.Reset();
```

### InvokeMode
When the timer's interval is shorter than the frame time, this parameter defines how the timer will trigger events.
- `InvokeMode.Multi`
In this mode, the `Elapsed` event is called multiple times within a single frame. The `TickData` property provides information on how many times the timer has triggered and the sequence number of the current event.
- `InvokeMode.Single`
In this mode, the `Elapsed` method is called once per frame. The `TickData` properties provide information only about the number of timer triggers.



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
# 2. Таймера:

## 1. DeltaTimeTimer:
- Использует разницу времени между кадрами для своих расчётов (Time.deltaTime и другие).
- При остановке и последующем запуске продолжает работу без учета времени, когда таймер был остановлен.
  
### Создание
``` csharp
public static DeltaTimeTimer Create( float interval, int loopCount, InvokeMode invokeMode, bool unscaledTime, PlayerLoopTiming playerLoopTiming) 
```
-```interval``` - Интервал времени, по истечению которого таймер вызывает событие ```Elapsed```.\
-```loopCount``` - Количество раз, которое таймер должен вызвать событие ```Elapsed```. ```-1``` - бесконечный режим работы таймера.\
-```invokeMode``` - Определяет, может ли таймер вызывать событие ```Elapsed``` несколько раз за один кадр или только один раз.\
-```unscaledTime``` - Указывает, использовать ли ```Time.unscaledDeltaTime``` или ```Time.deltaTime``` для расчета времени.\
-```playerLoopTiming``` - Определяет, в каком из игровых циклов (например, ```Update```, ```FixedUpdate``` и т.д.) будет использоваться таймер.

``` csharp
public static DeltaTimeTimer Create( float interval, Func<float> getDeltaTime, int loopCount, InvokeMode invokeMode, PlayerLoopTiming playerLoopTiming)
```
-```getDeltaTime``` - пользовательская функция, возвращающая интервал между текущим и предыдущим кадром.

### Свойства

#### Data
Содержит все рабочие параметры таймера.
-```ElapsedTime``` - Время с момента запуска или последнего срабатывания таймера.
-```Interval``` - Интервал времени, по истечению которого таймер вызывает событие ```Elapsed```.
-```CompletedTicks``` - Текущее количество срабатываний таймера.
-```TotalTicks``` - Количество раз сколько должен сработать таймер. -1 - бесконечный режим работы таймера.

#### TickData
Содержит информацию о последнем срабатывании таймера. Если таймер может сработать несколько раз в пределах одного кадра, это свойство позволяет определить количество вызовов события и их порядок.
-```TicksPerFrame``` - Количество срабатываний таймера в пределах одного кадра.
-```TickNumber``` - Порядковый номер срабатывания таймера в режиме ```InvokeMode.Multi```; всегда 0 в режиме ```InvokeMode.Single```.

&nbsp;
## 2. RealtimeTimer 
- Использует игровое время (Time.time и другие или пользовательскую функцию времени).
- При остановке и последующем запуске вызываются все пропущенные срабатывания.
- В случае многократных срабатываний в пределах одного кадра, таймер позволяет точно определить время каждого срабатывания, используя свойство Date.Time.
### Создание
``` csharp
public static RealtimeTimer Create(float interval, int loopsCount, InvokeMode invokeMode, RealtimeMode timeMode, PlayerLoopTiming playerLoopTiming)
```
-```interval``` - Интервал времени, по истечению которого таймер вызывает событие ```Elapsed```.\
-```loopCount``` - Количество раз, которое таймер должен вызвать событие ```Elapsed```. ```-1``` - бесконечный режим работы таймера.
-```invokeMode``` - Определяет, может ли таймер вызывать событие ```Elapsed``` несколько раз за один кадр или только один раз.
-```timeMode``` - Указывает тип используемого времени : ```Time.unscaledTime```, ```Time.time``` или ```Time.realtimeSinceStartup```.
-```playerLoopTiming``` - Определяет, в каком из игровых циклов (например, ```Update```, ```FixedUpdate``` и т.д.) будет использоваться таймер.

``` csharp
public static RealtimeTimer Create(float interval, Func<float> getTime, int loopsCount, InvokeMode invokeMode, PlayerLoopTiming playerLoopTiming)
```
-```getTime``` - Пользовательская функция, возвращающая текущее игровое время.

### Свойства
#### Data
Содержит все рабочие параметры таймера.
-```LastTime``` - Время последнего срабатывания таймера или время его запуска.
-```Interval``` - Временной интервал, по истечению которого таймер генерирует событие ```Elapsed```.
-```CompletedTicks``` - Текущее количество срабатываний таймера
-```TotalTicks``` - Общее количество срабатываний таймера до его остановки. Значение -1 указывает на бесконечный режим работы.

#### TickData
Содержит информацию о последнем срабатывании таймера. Если таймер может сработать несколько раз в пределах одного кадра, это свойство позволяет определить количество вызовов события и их порядок.
-```TicksPerFrame``` - Количество срабатываний таймера в пределах одного кадра;
-```TickNumber``` - Порядковый номер срабатывания таймера в режиме ```InvokeMode.Multi```; всегда 0 в режиме ```InvokeMode.Single```;

&nbsp;
## 3. DateTimeTimer 
- Использует системное время (DateTime.Now или пользовательскую функцию).
- При остановке и последующем запуске вызываются все пропущенные срабатывания.
- В случае многократных срабатываний в пределах одного кадра, таймер позволяет точно определить время каждого срабатывания, используя свойство Date.Time.
  
### Создание
``` csharp
public static DateTimeTimer Create(TimeSpan interval, int loopsCount, InvokeMode invokeMode, PlayerLoopTiming playerLoopTiming)
```
-```interval``` - Интервал времени, по истечению которого таймер вызывает событие ```Elapsed```.\
-```loopCount``` - Количество раз, которое таймер должен вызвать событие ```Elapsed```. ```-1``` - бесконечный режим работы таймера.\
-```invokeMode``` - Определяет, может ли таймер вызывать событие ```Elapsed``` несколько раз за один кадр или только один раз.\
-```playerLoopTiming``` - Определяет, в каком из игровых циклов (например, ```Update```, ```FixedUpdate``` и т.д.) будет использоваться таймер.

``` csharp
public static DateTimeTimer Create(TimeSpan interval, Func<DateTime> getTime, int loopsCount, InvokeMode invokeMode, PlayerLoopTiming playerLoopTiming)
```
-```getTime``` - Пользовательская функция, возвращающая текущее системное время.

&nbsp;
# 3. Использование
Пример таймера, который срабатывает каждую секунду и останавливается после 10 срабатываний.
``` csharp
    float interval = 1f;
    int loopCount = 10;
    Func<float> getTime = GameWorldTime.GetTime;
    InvokeMode invokeMode = InvokeMode.Multi;
    
    // Create timer
    var timer = RealtimeTimer.Create(interval, getTime, loopCount, invokeMode);
    
    timer.Elapsed += () => { Debug.Log($"Elapsed at realtime {Time.time}. Calculate time - {timer.Data.LastTime}"); };
    timer.Started += () => Debug.Log("Timer started");
    timer.Stopped += () => Debug.Log("Timer stopped");
    timer.Completed += () => Debug.Log("Tier finished");
    
    // Start or resume the timer
    timer.Start();
    
    // Modify timer properties
    timer.Data.Interval = interval * 2;
    timer.Data.LastTime = GameWorldTime.GetTime() + 10;
    
    // Stop the timer, with the ability to resume.
    timer.Stop();
    
    // Stop and reset the timer
    timer.Reset();
```

### InvokeMode
Если интервал срабатывания таймера меньше, чем интервал времени между кадрами, то этот параметр определяет как таймер будет вызывать событие.
- `InvokeMode.Multi`
В этом режиме событие `Elapsed` будет вызываться несколько раз в течении одного кадра. Свойство `TickData` позволяет получить информацию о количестве срабатываний таймера за кадр и номер текущего события.
- `InvokeMode.Single`
В этом режиме событие `Elapsed` будет вызываться один раз в течении одного кадра. Свойство `TickData` позволяет получить информацию только о коичестве срабатываний таймера.
