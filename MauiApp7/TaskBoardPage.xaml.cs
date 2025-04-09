using Microsoft.Maui.Controls;
using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using Microsoft.Maui.Controls;
using Microsoft.Maui.Graphics;
using Microsoft.Maui.Layouts;
using Microsoft.Maui.Controls.Shapes;
using System.IO;
// Алиасы для устранения неоднозначностей имён:
using MAUIPath = Microsoft.Maui.Controls.Shapes.Path;
namespace MauiApp7
{
    public partial class TaskBoardPage : ContentPage
    {
        private List<TaskModel> tasks = new();
        private Dictionary<TaskModel, View> taskShapes = new();
        private double timeScale = 0;

        // Добавьте поле класса
        private Point _panStartPosition;

        public TaskBoardPage()
        {
            InitializeComponent();
        }

        private async void OnAddTaskClicked(object sender, EventArgs e)
        {
            // Создаем страницу добавления задачи и подписываемся на событие TaskCreated
            var addTaskPage = new AddTaskPage();
            addTaskPage.TaskCreated += OnTaskCreated;
            await Navigation.PushAsync(addTaskPage);
        }

        private void OnTaskCreated(object sender, TaskModel task)
        {
            tasks.Add(task);
            AddTaskShape(task);
        }

        private void AddTaskShape(TaskModel task)
        {
            var shapeView = CreateTetrisShape(task);
            // Устанавливаем начальное положение фигуры через объект TaskField
            TaskField.SetLayoutBounds(shapeView, new Rect(100, 100, shapeView.WidthRequest, shapeView.HeightRequest));
            TaskField.SetLayoutFlags(shapeView, AbsoluteLayoutFlags.None);

            // Гестура для перемещения
            var panGesture = new PanGestureRecognizer();
            panGesture.PanUpdated += (s, e) => OnPanUpdated(s, e, shapeView);
            shapeView.GestureRecognizers.Add(panGesture);

            // Гестура двойного нажатия для показа информации о задаче
            var doubleTapGesture = new TapGestureRecognizer { NumberOfTapsRequired = 2 };
            doubleTapGesture.Tapped += (s, e) => ShowTaskInfo(task);
            shapeView.GestureRecognizers.Add(doubleTapGesture);

            TaskField.Children.Add(shapeView);
            taskShapes[task] = shapeView;
        }

        // Обновите метод OnPanUpdated
        private void OnPanUpdated(object sender, PanUpdatedEventArgs e, View shape)
        {
            switch (e.StatusType)
            {
                case GestureStatus.Started:
                    var bounds = TaskField.GetLayoutBounds(shape);
                    _panStartPosition = new Point(bounds.X, bounds.Y);
                    break;

                case GestureStatus.Running:
                    var newX = _panStartPosition.X + e.TotalX;
                    var newY = _panStartPosition.Y + e.TotalY;

                    if (!IsCollision(shape, newX, newY))
                    {
                        TaskField.SetLayoutBounds(shape, new Rect(newX, newY, shape.WidthRequest, shape.HeightRequest));
                    }
                    break;
            }
        }

        private bool IsCollision(View current, double x, double y)
        {
            var movingRect = new Rect(x, y, current.Width, current.Height);
            foreach (var view in TaskField.Children)
            {
                if (view == current)
                    continue;
                var bounds = TaskField.GetLayoutBounds(view);
                var rect = new Rect(bounds.X, bounds.Y, view.Width, view.Height);
                if (movingRect.IntersectsWith(rect))
                    return true;
            }
            return false;
        }

        private View CreateTetrisShape(TaskModel task)
        {
            Shape shape;
            switch (task.Category)
            {
                case "Работа":
                    // Фигура-палка (прямоугольник)
                    shape = new Rectangle
                    {
                        Fill = task.IsImportant ? Brush.Red : Brush.Blue,
                        WidthRequest = 20,
                        HeightRequest = 80
                    };
                    break;
                case "Учёба":
                    // Фигура в виде "Т", создается с помощью Path
                    var converter = new PathGeometryConverter();
                    var geometry = (PathGeometry)converter.ConvertFromInvariantString("M0,0 L60,0 L60,20 L40,20 L40,40 L20,40 L20,20 L0,20 Z");
                    shape = new MAUIPath
                    {
                        Data = geometry,
                        Fill = task.IsImportant ? Brush.Red : Brush.Green,
                        WidthRequest = 60,
                        HeightRequest = 40
                    };
                    break;
                case "Личное":
                default:
                    // Квадрат
                    shape = new Rectangle
                    {
                        Fill = task.IsImportant ? Brush.Red : Brush.Purple,
                        WidthRequest = 50,
                        HeightRequest = 50
                    };
                    break;
            }
            return new ContentView
            {
                Content = shape,
                WidthRequest = shape.WidthRequest,
                HeightRequest = shape.HeightRequest
            };
        }

        private async void ShowTaskInfo(TaskModel task)
        {
            if (task.IsImportant)
            {
                // Если задача важная, предлагаем выбор: "Режим Pomodoro" или просто ОК
                string action = await DisplayActionSheet(
                    $"Название: {task.Name}\nКатегория: {task.Category}\nДлительность: {task.Duration}",
                    "Отмена",
                    null,
                    "Режим Pomodoro", "ОК");
                if (action == "Режим Pomodoro")
                {
                   // await Navigation.PushAsync(new MainPage());
                }
            }
            else
            {
                await DisplayAlert("Информация о задаче",
                    $"Название: {task.Name}\nКатегория: {task.Category}\nДлительность: {task.Duration}",
                    "ОК");
            }
        }

        private void OnTimeScaleChanged(object sender, ValueChangedEventArgs e)
        {
            timeScale = e.NewValue;
            string scale = timeScale switch
            {
                0 => "Часы",
                1 => "Дни",
                2 => "Недели",
                3 => "Месяцы",
                _ => "Неизвестно"
            };
            TimeScaleLabel.Text = $"Масштаб: {scale}";
        }
    }
}
