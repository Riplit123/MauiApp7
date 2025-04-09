using Microsoft.Maui.Controls;
using System;

namespace MauiApp7;

public partial class AddTaskPage : ContentPage
{
    // Событие для уведомления о создании задачи
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

        // Генерируем событие создания задачи
        TaskCreated?.Invoke(this, task);
        // Возврат на предыдущую страницу (TaskBoardPage)
        await Navigation.PopAsync();
    }

    private void OnCancelClicked(object sender, EventArgs e)
    {
        // Очистка данных
        NameEntry.Text = string.Empty;
        CategoryPicker.SelectedIndex = -1;
        DurationPicker.SelectedIndex = -1;
        ImportantSwitch.IsToggled = false;
    }
}
