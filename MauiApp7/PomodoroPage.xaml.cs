namespace MauiApp7;

public partial class PomodoroPage : ContentPage
{
    // ����� ��������� �������
    private bool timerRunning = false;
    private bool workPhase = true; // true � ������� ����, false � �������

    // �������� �����
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
        StartStopButton.Text = timerRunning ? "����" : "�����";

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
            return true; // ���������� ������
        }
        else
        {
            // ���� ����������� � �����������
            workPhase = !workPhase;
            remainingTime = workPhase ? workTime : breakTime;
            DisplayAlert("����������", workPhase ? "������� ������� ����� (25 �����)" : "������� ������� (5 �����)", "��");
            UpdateTimerDisplay();
            return timerRunning; // ���� ������ �������, ����������
        }
    }
}