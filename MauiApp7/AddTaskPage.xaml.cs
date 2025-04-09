using Microsoft.Maui.Controls;
using System;

namespace MauiApp7;

public partial class AddTaskPage : ContentPage
{
    // ������� ��� ����������� � �������� ������
    public event EventHandler<TaskModel> TaskCreated;

    public AddTaskPage()
    {
        InitializeComponent();
    }

    private async void OnSaveClicked(object sender, EventArgs e)
    {
        var task = new TaskModel
        {
            Name = NameEntry.Text,
            Category = CategoryPicker.SelectedItem?.ToString(),
            Duration = DurationPicker.SelectedItem?.ToString(),
            IsImportant = ImportantSwitch.IsToggled
        };

        // ���������� ������� �������� ������
        TaskCreated?.Invoke(this, task);
        // ������� �� ���������� �������� (TaskBoardPage)
        await Navigation.PopAsync();
    }

    private void OnCancelClicked(object sender, EventArgs e)
    {
        // ������� ������
        NameEntry.Text = string.Empty;
        CategoryPicker.SelectedIndex = -1;
        DurationPicker.SelectedIndex = -1;
        ImportantSwitch.IsToggled = false;
    }
}
