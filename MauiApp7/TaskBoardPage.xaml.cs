using Microsoft.Maui.Layouts;
using Microsoft.Maui.Controls.Shapes;
// ������ ��� ���������� ���������������� ���:
using MAUIPath = Microsoft.Maui.Controls.Shapes.Path;

// �������� ������������ ���� ��� LongPressBehavior �� CommunityToolkit.Maui
using CommunityToolkit.Maui.Behaviors;
namespace MauiApp7
{
    public partial class TaskBoardPage : ContentPage
    {
        // ��������� ����� � ��������������� �����
        private List<TaskModel> tasks = new();
        private Dictionary<TaskModel, View> taskShapes = new();
        private double timeScale = 0;

        // ������� "��������" ���� ��� ���������� ������ (�� ��������� ���� "���")
        private AbsoluteLayout currentField;

        public TaskBoardPage()
        {
            InitializeComponent();
            currentField = HourField;
            // ������� ��������� ����� ��� �������������
            UpdateTimeline("hour");
        }

        // ���������� ��������� ����� ��� ��������� ��������
        protected override void OnAppearing()
        {
            base.OnAppearing();
            // ��������� ����� � ����������� �� ��������� ����
            // ����� currentField == HourField �� ���������, ����� �������� ������, ���� �����
            UpdateTimeline("hour");
        }

        #region ������������ ����� � ���������� �����

        private void OnHourButtonClicked(object sender, EventArgs e)
        {
            HourField.IsVisible = true;
            DayField.IsVisible = false;
            WeekField.IsVisible = false;
            MonthField.IsVisible = false;
            currentField = HourField;
            UpdateTimeline("hour");
        }

        private void OnDayButtonClicked(object sender, EventArgs e)
        {
            HourField.IsVisible = false;
            DayField.IsVisible = true;
            WeekField.IsVisible = false;
            MonthField.IsVisible = false;
            currentField = DayField;
            UpdateTimeline("day");
        }

        private void OnWeekButtonClicked(object sender, EventArgs e)
        {
            HourField.IsVisible = false;
            DayField.IsVisible = false;
            WeekField.IsVisible = true;
            MonthField.IsVisible = false;
            currentField = WeekField;
            UpdateTimeline("week");
        }

        private void OnMonthButtonClicked(object sender, EventArgs e)
        {
            HourField.IsVisible = false;
            DayField.IsVisible = false;
            WeekField.IsVisible = false;
            MonthField.IsVisible = true;
            currentField = MonthField;
            UpdateTimeline("month");
        }

        // ����� ���������� ��������� ����� (TimelinePanel)
        private void UpdateTimeline(string timelineType)
        {
            TimelinePanel.Children.Clear();

            // ������� ���������
            var header = new Label
            {
                Text = "��������� �����:",
                FontAttributes = FontAttributes.Bold,
                HorizontalOptions = LayoutOptions.Center,
                TextColor = Colors.Black
            };
            TimelinePanel.Children.Add(header);

            if (timelineType == "hour")
            {
                // 6 ������� �� 10 �����, �� 10 �� 60 �����
                string[] divisions = new string[] { "10 ���", "20 ���", "30 ���", "40 ���", "50 ���", "60 ���" };
                // ���������� �� ����� ����� (����� �������� ��������� �������, ���� ���������)
                foreach (var div in divisions)
                {
                    TimelinePanel.Children.Add(new Label
                    {
                        Text = div,
                        FontSize = 12,
                        HorizontalOptions = LayoutOptions.Center,
                        TextColor = Colors.Black
                    });
                }
            }
            else if (timelineType == "day")
            {
                // 24 ������� � �� ����, ��������� �����, ����� ����������
                for (int i = 0; i < 24; i++)
                {
                    TimelinePanel.Children.Add(new Label
                    {
                        Text = $"{i}:00",
                        FontSize = 10,
                        HorizontalOptions = LayoutOptions.Center,
                        TextColor = Colors.Black
                    });
                }
            }
            else if (timelineType == "week")
            {
                // 7 ������� � ��� ������ (�������� �������, ���� ���������)
                string[] days = new string[] { "��", "��", "��", "��", "��", "��", "��" };
                // ����� ������ ������� ���� ��������� ����, ���������� ������
                Array.Reverse(days);
                foreach (string day in days)
                {
                    TimelinePanel.Children.Add(new Label
                    {
                        Text = day,
                        FontSize = 14,
                        HorizontalOptions = LayoutOptions.Center,
                        TextColor = Colors.Black
                    });
                }
            }
            else if (timelineType == "month")
            {
                // 4 ������� � ������ 1, ������ 2, ...
                for (int i = 1; i <= 4; i++)
                {
                    TimelinePanel.Children.Add(new Label
                    {
                        Text = $"������ {i}",
                        FontSize = 14,
                        HorizontalOptions = LayoutOptions.Center,
                        TextColor = Colors.Black
                    });
                }
            }
        }

        #endregion

        private async void OnAddTaskClicked(object sender, EventArgs e)
        {
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
            AbsoluteLayout targetField = DetermineTargetField(task);
            targetField.SetLayoutBounds(shapeView, new Rect(100, 100, shapeView.WidthRequest, shapeView.HeightRequest));
            targetField.SetLayoutFlags(shapeView, AbsoluteLayoutFlags.None);

            AttachPanGesture(shapeView);

            targetField.Children.Add(shapeView);
            taskShapes[task] = shapeView;
        }

        private AbsoluteLayout DetermineTargetField(TaskModel task)
        {
            string lowerDuration = task.Duration.ToLower();
            if (lowerDuration.Contains("���"))
                return HourField;
            else if (lowerDuration.Contains("����"))
                return DayField;
            else if (lowerDuration.Contains("������"))
                return WeekField;
            else if (lowerDuration.Contains("�����"))
                return MonthField;
            else
                return currentField;
        }

        private void AttachPanGesture(View shape)
        {
            double initialX = 0, initialY = 0;
            double initialWidth = 0, initialHeight = 0;
            var panGesture = new PanGestureRecognizer();
            panGesture.PanUpdated += (s, e) =>
            {
                var parent = (AbsoluteLayout)shape.Parent;
                switch (e.StatusType)
                {
                    case GestureStatus.Started:
                        {
                            var bounds = parent.GetLayoutBounds(shape);
                            initialX = bounds.X;
                            initialY = bounds.Y;
                            initialWidth = bounds.Width;
                            initialHeight = bounds.Height;
                            break;
                        }
                    case GestureStatus.Running:
                        {
                            var newX = initialX + e.TotalX;
                            var newY = initialY + e.TotalY;

                            double containerWidth = parent.Width;
                            double containerHeight = parent.Height;
                            if (newX < 0)
                                newX = 0;
                            if (newY < 0)
                                newY = 0;
                            if (newX + initialWidth > containerWidth)
                                newX = containerWidth - initialWidth;
                            if (newY + initialHeight > containerHeight)
                                newY = containerHeight - initialHeight;

                            if (!IsCollision(shape, newX, newY, parent))
                            {
                                parent.SetLayoutBounds(shape, new Rect(newX, newY, initialWidth, initialHeight));
                            }
                            break;
                        }
                }
            };
            shape.GestureRecognizers.Add(panGesture);
        }

        private bool IsCollision(View current, double x, double y, AbsoluteLayout container)
        {
            var movingRect = new Rect(x, y, current.Width, current.Height);
            foreach (var view in container.Children)
            {
                if (view == current)
                    continue;
                var bounds = container.GetLayoutBounds(view);
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
                case "������":
                    shape = new Rectangle
                    {
                        Fill = task.IsImportant ? Brush.Red : Brush.Blue,
                        WidthRequest = 20,
                        HeightRequest = 80
                    };
                    break;
                case "�����":
                    var converter = new PathGeometryConverter();
                    var geometry = (PathGeometry)converter.ConvertFromInvariantString(
                        "M0,0 L60,0 L60,20 L40,20 L40,40 L20,40 L20,20 L0,20 Z");
                    shape = new MAUIPath
                    {
                        Data = geometry,
                        Fill = task.IsImportant ? Brush.Red : Brush.Green,
                        WidthRequest = 60,
                        HeightRequest = 40
                    };
                    break;
                case "������":
                default:
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

        private void RotateShape(View shape)
        {
            shape.Rotation = (shape.Rotation + 90) % 360;
        }

        private void OnRotateLastTaskClicked(object sender, EventArgs e)
        {
            if (tasks.Count > 0)
            {
                TaskModel lastTask = tasks[tasks.Count - 1];
                if (taskShapes.TryGetValue(lastTask, out View shapeView))
                {
                    RotateShape(shapeView);
                }
            }
            else
            {
                DisplayAlert("���������", "����� ���� ���", "��");
            }
        }

        private void OnTimeScaleChanged(object sender, ValueChangedEventArgs e)
        {
            timeScale = e.NewValue;
            string scale = timeScale switch
            {
                0 => "����",
                1 => "���",
                2 => "������",
                3 => "������",
                _ => "����������"
            };
            TimeScaleLabel.Text = $"�������: {scale}";
        }
    }
}
