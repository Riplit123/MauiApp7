namespace MauiApp7;

public partial class PomodoroPage : ContentPage
{
    // Флаги состояния таймера
    private bool timerRunning = false;
    private bool workPhase = true; // true – рабочая фаза, false – перерыв

    // Заданное время
    private TimeSpan workTime = TimeSpan.FromMinutes(25);
    private TimeSpan breakTime = TimeSpan.FromMinutes(5);
    private TimeSpan remainingTime;

    public PomodoroPage()
    {
        InitializeComponent();
        remainingTime = workTime;
        UpdateTimerDisplay();
    }

    private void UpdateTimerDisplay()
    {
        TimerLabel.Text = remainingTime.ToString(@"mm\:ss");
    }

    private void OnStartStopClicked(object sender, EventArgs e)
    {
        timerRunning = !timerRunning;
        StartStopButton.Text = timerRunning ? "Стоп" : "Старт";

        if (timerRunning)
        {
            Device.StartTimer(TimeSpan.FromSeconds(1), TimerTick);
        }
    }

    private bool TimerTick()
    {
        if (!timerRunning)
            return false;

        if (remainingTime.TotalSeconds > 0)
        {
            remainingTime = remainingTime.Subtract(TimeSpan.FromSeconds(1));
            UpdateTimerDisplay();
            return true; // продолжаем таймер
        }
        else
        {
            // Фаза закончилась – переключаем
            workPhase = !workPhase;
            remainingTime = workPhase ? workTime : breakTime;
            DisplayAlert("Информация", workPhase ? "Начался рабочий режим (25 минут)" : "Начался перерыв (5 минут)", "ОК");
            UpdateTimerDisplay();
            return timerRunning; // если таймер запущен, продолжаем
        }
    }
}