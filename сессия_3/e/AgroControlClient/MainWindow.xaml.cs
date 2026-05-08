using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace AgroControlClient
{
    public partial class MainWindow : Window
    {
        private Button _activeButton;

        public MainWindow()
        {
            InitializeComponent();

            btnProducts.Click += (s, e) => ShowProducts();
            btnRecipes.Click += (s, e) => ShowRecipes();
            btnTechCards.Click += (s, e) => ShowTechCards();
            btnOrders.Click += (s, e) => ShowOrders();
            btnBatches.Click += (s, e) => ShowBatches();
            btnExtruder.Click += (s, e) => ShowExtruder();
            btnMonitor.Click += (s, e) => ShowMonitor();
            btnDeviations.Click += (s, e) => ShowDeviations();
            btnReports.Click += (s, e) => ShowReports();
            btnLogout.Click += (s, e) => Logout();
            btnRefresh.Click += (s, e) => RefreshCurrentView();

            ShowDashboard();
        }

        private void SetActiveButton(Button button, string title)
        {
            if (_activeButton != null)
                _activeButton.Background = Brushes.Transparent;
            _activeButton = button;
            if (button != null)
                button.Background = new SolidColorBrush(Color.FromRgb(61, 86, 110));
            lblTitle.Text = title;
        }

        private void RefreshCurrentView()
        {
            if (_activeButton == btnProducts) ShowProducts();
            else if (_activeButton == btnRecipes) ShowRecipes();
            else if (_activeButton == btnTechCards) ShowTechCards();
            else if (_activeButton == btnOrders) ShowOrders();
            else if (_activeButton == btnBatches) ShowBatches();
            else if (_activeButton == btnExtruder) ShowExtruder();
            else if (_activeButton == btnMonitor) ShowMonitor();
            else if (_activeButton == btnDeviations) ShowDeviations();
            else if (_activeButton == btnReports) ShowReports();
            else ShowDashboard();
        }

        private void ShowDashboard()
        {
            SetActiveButton(null, "Главная");
            var stack = new StackPanel { Margin = new Thickness(20) };
            stack.Children.Add(new TextBlock { Text = "📊 Панель управления", FontSize = 22, FontWeight = FontWeights.Bold, Margin = new Thickness(0, 0, 0, 20) });

            var grid = new Grid();
            grid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });
            grid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });
            grid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });
            grid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });

            grid.Children.Add(CreateStatCard("Продуктов", "12", "📦", 0));
            grid.Children.Add(CreateStatCard("Рецептур", "8", "📋", 1));
            grid.Children.Add(CreateStatCard("Партий в работе", "5", "🏭", 2));
            grid.Children.Add(CreateStatCard("Отклонений", "3", "⚠️", 3));
            stack.Children.Add(grid);

            ContentContainer.Content = new ScrollViewer { Content = stack };
        }

        // ==================== 1. КАРТОЧКИ ПРОДУКЦИИ ====================
        private void ShowProducts()
        {
            SetActiveButton(btnProducts, "Карточки продукции");
            var panel = new StackPanel { Margin = new Thickness(20) };
            panel.Children.Add(new TextBlock { Text = "📦 Карточки продукции", FontSize = 22, FontWeight = FontWeights.Bold, Margin = new Thickness(0, 0, 0, 20) });

            var dg = new DataGrid { Height = 400, Background = Brushes.White, AutoGenerateColumns = false };
            dg.Columns.Add(new DataGridTextColumn { Header = "Код", Binding = new System.Windows.Data.Binding("Code"), Width = new DataGridLength(100) });
            dg.Columns.Add(new DataGridTextColumn { Header = "Наименование", Binding = new System.Windows.Data.Binding("Name"), Width = new DataGridLength(200) });
            dg.Columns.Add(new DataGridTextColumn { Header = "Тип", Binding = new System.Windows.Data.Binding("Type"), Width = new DataGridLength(120) });
            dg.Columns.Add(new DataGridTextColumn { Header = "Форма выпуска", Binding = new System.Windows.Data.Binding("Form"), Width = new DataGridLength(120) });
            dg.Columns.Add(new DataGridTextColumn { Header = "Статус", Binding = new System.Windows.Data.Binding("Status"), Width = new DataGridLength(100) });

            dg.ItemsSource = new[]
            {
                new { Code = "HERB-A", Name = "Гербицид А", Type = "Гербицид", Form = "Жидкость", Status = "Активен" },
                new { Code = "INS-B", Name = "Инсектицид Б", Type = "Инсектицид", Form = "Жидкость", Status = "Активен" },
                new { Code = "FUN-C", Name = "Фунгицид В", Type = "Фунгицид", Form = "Гранулы", Status = "Черновик" },
                new { Code = "REG-D", Name = "Регулятор роста Д", Type = "Регулятор", Form = "Жидкость", Status = "Активен" }
            };
            panel.Children.Add(dg);

            ContentContainer.Content = new ScrollViewer { Content = panel };
        }

        // ==================== 2. РЕЦЕПТУРЫ ====================
        private void ShowRecipes()
        {
            SetActiveButton(btnRecipes, "Рецептуры");
            var panel = new StackPanel { Margin = new Thickness(20) };
            panel.Children.Add(new TextBlock { Text = "📋 Рецептуры", FontSize = 22, FontWeight = FontWeights.Bold, Margin = new Thickness(0, 0, 0, 20) });

            var dg = new DataGrid { Height = 250, Background = Brushes.White, AutoGenerateColumns = false };
            dg.Columns.Add(new DataGridTextColumn { Header = "Продукт", Binding = new System.Windows.Data.Binding("Product"), Width = new DataGridLength(150) });
            dg.Columns.Add(new DataGridTextColumn { Header = "Версия", Binding = new System.Windows.Data.Binding("Version"), Width = new DataGridLength(80) });
            dg.Columns.Add(new DataGridTextColumn { Header = "Статус", Binding = new System.Windows.Data.Binding("Status"), Width = new DataGridLength(100) });
            dg.Columns.Add(new DataGridTextColumn { Header = "Дата создания", Binding = new System.Windows.Data.Binding("Date"), Width = new DataGridLength(120) });
            dg.Columns.Add(new DataGridTextColumn { Header = "Описание", Binding = new System.Windows.Data.Binding("Description"), Width = new DataGridLength(200) });

            dg.ItemsSource = new[]
            {
                new { Product = "Гербицид А", Version = "1", Status = "Архив", Date = "10.01.2025", Description = "Базовая формула" },
                new { Product = "Гербицид А", Version = "2", Status = "Активна", Date = "18.01.2025", Description = "Улучшенная формула" },
                new { Product = "Инсектицид Б", Version = "1", Status = "Архив", Date = "12.01.2025", Description = "Базовая версия" },
                new { Product = "Инсектицид Б", Version = "2", Status = "Активна", Date = "20.01.2025", Description = "Повышенная эффективность" }
            };
            panel.Children.Add(dg);

            panel.Children.Add(new TextBlock { Text = "Компоненты рецептуры (Гербицид А v2)", FontSize = 16, FontWeight = FontWeights.Bold, Margin = new Thickness(0, 20, 0, 10) });

            var compDg = new DataGrid { Height = 150, Background = Brushes.White, AutoGenerateColumns = false };
            compDg.Columns.Add(new DataGridTextColumn { Header = "Компонент", Binding = new System.Windows.Data.Binding("Component"), Width = new DataGridLength(180) });
            compDg.Columns.Add(new DataGridTextColumn { Header = "Доля, %", Binding = new System.Windows.Data.Binding("Percent"), Width = new DataGridLength(100) });
            compDg.Columns.Add(new DataGridTextColumn { Header = "Порядок загрузки", Binding = new System.Windows.Data.Binding("Order"), Width = new DataGridLength(120) });
            compDg.Columns.Add(new DataGridTextColumn { Header = "Критичный", Binding = new System.Windows.Data.Binding("Critical"), Width = new DataGridLength(100) });

            compDg.ItemsSource = new[]
            {
                new { Component = "Активное вещество А", Percent = "48%", Order = "1", Critical = "Да" },
                new { Component = "Растворитель Б", Percent = "28%", Order = "2", Critical = "Нет" },
                new { Component = "Эмульгатор В", Percent = "24%", Order = "3", Critical = "Нет" }
            };
            panel.Children.Add(compDg);

            var totalLabel = new TextBlock { Text = "✅ Сумма компонентов: 100%", FontSize = 14, FontWeight = FontWeights.Bold, Foreground = new SolidColorBrush(Color.FromRgb(76, 175, 80)), Margin = new Thickness(0, 10, 0, 0) };
            panel.Children.Add(totalLabel);

            ContentContainer.Content = new ScrollViewer { Content = panel };
        }

        // ==================== 3. ТЕХНОЛОГИЧЕСКИЕ КАРТЫ ====================
        private void ShowTechCards()
        {
            SetActiveButton(btnTechCards, "Технологические карты");
            var panel = new StackPanel { Margin = new Thickness(20) };
            panel.Children.Add(new TextBlock { Text = "🗺️ Технологические карты", FontSize = 22, FontWeight = FontWeights.Bold, Margin = new Thickness(0, 0, 0, 20) });

            var dg = new DataGrid { Height = 200, Background = Brushes.White, AutoGenerateColumns = false };
            dg.Columns.Add(new DataGridTextColumn { Header = "Продукт", Binding = new System.Windows.Data.Binding("Product"), Width = new DataGridLength(150) });
            dg.Columns.Add(new DataGridTextColumn { Header = "Версия", Binding = new System.Windows.Data.Binding("Version"), Width = new DataGridLength(80) });
            dg.Columns.Add(new DataGridTextColumn { Header = "Статус", Binding = new System.Windows.Data.Binding("Status"), Width = new DataGridLength(100) });
            dg.Columns.Add(new DataGridTextColumn { Header = "Активна", Binding = new System.Windows.Data.Binding("Active"), Width = new DataGridLength(80) });

            dg.ItemsSource = new[]
            {
                new { Product = "Гербицид А", Version = "1", Status = "Активна", Active = "Да" },
                new { Product = "Инсектицид Б", Version = "1", Status = "Активна", Active = "Да" }
            };
            panel.Children.Add(dg);

            panel.Children.Add(new TextBlock { Text = "Шаги технологического процесса (Гербицид А)", FontSize = 16, FontWeight = FontWeights.Bold, Margin = new Thickness(0, 20, 0, 10) });

            var stepsDg = new DataGrid { Height = 200, Background = Brushes.White, AutoGenerateColumns = false };
            stepsDg.Columns.Add(new DataGridTextColumn { Header = "Порядок", Binding = new System.Windows.Data.Binding("Order"), Width = new DataGridLength(70) });
            stepsDg.Columns.Add(new DataGridTextColumn { Header = "Шаг", Binding = new System.Windows.Data.Binding("Step"), Width = new DataGridLength(180) });
            stepsDg.Columns.Add(new DataGridTextColumn { Header = "Тип", Binding = new System.Windows.Data.Binding("Type"), Width = new DataGridLength(120) });
            stepsDg.Columns.Add(new DataGridTextColumn { Header = "Длительность, мин", Binding = new System.Windows.Data.Binding("Duration"), Width = new DataGridLength(120) });
            stepsDg.Columns.Add(new DataGridTextColumn { Header = "Инструкция", Binding = new System.Windows.Data.Binding("Instruction"), Width = new DataGridLength(250) });

            stepsDg.ItemsSource = new[]
            {
                new { Order = "1", Step = "Смешивание компонентов", Type = "mixing", Duration = "30", Instruction = "Загрузить компоненты по порядку" },
                new { Order = "2", Step = "Выдержка", Type = "holding", Duration = "120", Instruction = "Выдержать при 60°C" },
                new { Order = "3", Step = "Экструзия", Type = "extrusion", Duration = "45", Instruction = "Т=80°C, P=3.0 bar" },
                new { Order = "4", Step = "Охлаждение", Type = "cooling", Duration = "60", Instruction = "Охладить до 25°C" }
            };
            panel.Children.Add(stepsDg);

            ContentContainer.Content = new ScrollViewer { Content = panel };
        }

        // ==================== 4. ПРОИЗВОДСТВЕННЫЕ ЗАКАЗЫ ====================
        private void ShowOrders()
        {
            SetActiveButton(btnOrders, "Производственные заказы");
            var panel = new StackPanel { Margin = new Thickness(20) };
            panel.Children.Add(new TextBlock { Text = "📄 Производственные заказы", FontSize = 22, FontWeight = FontWeights.Bold, Margin = new Thickness(0, 0, 0, 20) });

            var dg = new DataGrid { Height = 350, Background = Brushes.White, AutoGenerateColumns = false };
            dg.Columns.Add(new DataGridTextColumn { Header = "Номер заказа", Binding = new System.Windows.Data.Binding("Number"), Width = new DataGridLength(120) });
            dg.Columns.Add(new DataGridTextColumn { Header = "Продукт", Binding = new System.Windows.Data.Binding("Product"), Width = new DataGridLength(150) });
            dg.Columns.Add(new DataGridTextColumn { Header = "Количество, кг", Binding = new System.Windows.Data.Binding("Quantity"), Width = new DataGridLength(100) });
            dg.Columns.Add(new DataGridTextColumn { Header = "Приоритет", Binding = new System.Windows.Data.Binding("Priority"), Width = new DataGridLength(80) });
            dg.Columns.Add(new DataGridTextColumn { Header = "Статус", Binding = new System.Windows.Data.Binding("Status"), Width = new DataGridLength(100) });
            dg.Columns.Add(new DataGridTextColumn { Header = "Плановая дата", Binding = new System.Windows.Data.Binding("Date"), Width = new DataGridLength(120) });

            dg.ItemsSource = new[]
            {
                new { Number = "PO-2401", Product = "Гербицид А", Quantity = "1000", Priority = "1", Status = "Выполнен", Date = "01.03.2025" },
                new { Number = "PO-2402", Product = "Инсектицид Б", Quantity = "500", Priority = "2", Status = "В работе", Date = "03.03.2025" },
                new { Number = "PO-2403", Product = "Гербицид А", Quantity = "800", Priority = "1", Status = "В работе", Date = "04.03.2025" },
                new { Number = "PO-2404", Product = "Инсектицид Б", Quantity = "300", Priority = "3", Status = "План", Date = "10.03.2025" }
            };
            panel.Children.Add(dg);

            ContentContainer.Content = new ScrollViewer { Content = panel };
        }

        // ==================== 5. ПРОИЗВОДСТВЕННЫЕ ПАРТИИ ====================
        private void ShowBatches()
        {
            SetActiveButton(btnBatches, "Производственные партии");
            var panel = new StackPanel { Margin = new Thickness(20) };
            panel.Children.Add(new TextBlock { Text = "🏭 Производственные партии", FontSize = 22, FontWeight = FontWeights.Bold, Margin = new Thickness(0, 0, 0, 20) });

            var dg = new DataGrid { Height = 350, Background = Brushes.White, AutoGenerateColumns = false };
            dg.Columns.Add(new DataGridTextColumn { Header = "Номер партии", Binding = new System.Windows.Data.Binding("Number"), Width = new DataGridLength(120) });
            dg.Columns.Add(new DataGridTextColumn { Header = "Продукт", Binding = new System.Windows.Data.Binding("Product"), Width = new DataGridLength(150) });
            dg.Columns.Add(new DataGridTextColumn { Header = "Заказ", Binding = new System.Windows.Data.Binding("Order"), Width = new DataGridLength(100) });
            dg.Columns.Add(new DataGridTextColumn { Header = "Статус", Binding = new System.Windows.Data.Binding("Status"), Width = new DataGridLength(100) });
            dg.Columns.Add(new DataGridTextColumn { Header = "Факт, кг", Binding = new System.Windows.Data.Binding("Quantity"), Width = new DataGridLength(80) });
            dg.Columns.Add(new DataGridTextColumn { Header = "Лаборатория", Binding = new System.Windows.Data.Binding("Lab"), Width = new DataGridLength(100) });
            dg.Columns.Add(new DataGridTextColumn { Header = "Дата запуска", Binding = new System.Windows.Data.Binding("Date"), Width = new DataGridLength(150) });

            dg.ItemsSource = new[]
            {
                new { Number = "B-2401-01", Product = "Гербицид А", Order = "PO-2401", Status = "Завершена", Quantity = "998", Lab = "Одобрена", Date = "01.03.2025 08:00" },
                new { Number = "B-2402-01", Product = "Инсектицид Б", Order = "PO-2402", Status = "В работе", Quantity = "250", Lab = "Ожидает", Date = "03.03.2025 09:00" },
                new { Number = "B-2403-01", Product = "Гербицид А", Order = "PO-2403", Status = "В работе", Quantity = "400", Lab = "Ожидает", Date = "04.03.2025 10:00" }
            };
            panel.Children.Add(dg);

            ContentContainer.Content = new ScrollViewer { Content = panel };
        }

        // ==================== 6. НАСТРОЙКА ЭКСТРУДЕРА ====================
        private void ShowExtruder()
        {
            SetActiveButton(btnExtruder, "Настройка экструдера");
            var panel = new StackPanel { Margin = new Thickness(20) };
            panel.Children.Add(new TextBlock { Text = "⚙️ Настройка экструдера", FontSize = 22, FontWeight = FontWeights.Bold, Margin = new Thickness(0, 0, 0, 20) });

            var settingsGrid = new Grid { Margin = new Thickness(0, 20, 0, 20) };
            for (int i = 0; i < 7; i++)
                settingsGrid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
            settingsGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(200) });
            settingsGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(150) });
            settingsGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(100) });

            AddSettingRow(settingsGrid, 0, "Температура зоны 1:", "85", "°C");
            AddSettingRow(settingsGrid, 1, "Температура зоны 2:", "90", "°C");
            AddSettingRow(settingsGrid, 2, "Температура зоны 3:", "95", "°C");
            AddSettingRow(settingsGrid, 3, "Температура зоны 4:", "100", "°C");
            AddSettingRow(settingsGrid, 4, "Давление:", "3.5", "bar");
            AddSettingRow(settingsGrid, 5, "Скорость шнека:", "150", "rpm");
            AddSettingRow(settingsGrid, 6, "Скорость подачи:", "120", "кг/ч");

            panel.Children.Add(settingsGrid);

            var saveBtn = new Button { Content = "💾 Сохранить настройки", Width = 200, Height = 40, Background = new SolidColorBrush(Color.FromRgb(46, 125, 50)), Foreground = Brushes.White, Margin = new Thickness(0, 10, 0, 0) };
            panel.Children.Add(saveBtn);

            ContentContainer.Content = new ScrollViewer { Content = panel };
        }

        private void AddSettingRow(Grid grid, int row, string label, string defaultValue, string unit)
        {
            var labelText = new TextBlock { Text = label, FontSize = 14, VerticalAlignment = VerticalAlignment.Center };
            grid.Children.Add(labelText);
            Grid.SetRow(labelText, row);
            Grid.SetColumn(labelText, 0);

            var textBox = new TextBox { Text = defaultValue, Width = 120, Height = 35, Margin = new Thickness(5) };
            grid.Children.Add(textBox);
            Grid.SetRow(textBox, row);
            Grid.SetColumn(textBox, 1);

            var unitText = new TextBlock { Text = unit, FontSize = 14, VerticalAlignment = VerticalAlignment.Center, Margin = new Thickness(5, 0, 0, 0) };
            grid.Children.Add(unitText);
            Grid.SetRow(unitText, row);
            Grid.SetColumn(unitText, 2);
        }

        // ==================== 7. МОНИТОРИНГ ПАРТИЙ ====================
        private void ShowMonitor()
        {
            SetActiveButton(btnMonitor, "Мониторинг партий");
            var panel = new StackPanel { Margin = new Thickness(20) };
            panel.Children.Add(new TextBlock { Text = "📊 Мониторинг выполнения партий", FontSize = 22, FontWeight = FontWeights.Bold, Margin = new Thickness(0, 0, 0, 20) });

            var activeCard = new Border { Background = Brushes.White, BorderBrush = Brushes.LightGray, BorderThickness = new Thickness(1), CornerRadius = new CornerRadius(8), Padding = new Thickness(20), Margin = new Thickness(0, 0, 0, 20) };
            var activeStack = new StackPanel();
            activeStack.Children.Add(new TextBlock { Text = "🟢 Активная партия", FontSize = 16, FontWeight = FontWeights.Bold, Foreground = new SolidColorBrush(Color.FromRgb(76, 175, 80)) });
            activeStack.Children.Add(new TextBlock { Text = "Партия: B-2402-01 | Продукт: Инсектицид Б", FontSize = 14, Margin = new Thickness(0, 5, 0, 0) });
            activeStack.Children.Add(new TextBlock { Text = "Текущий шаг: Экструзия", FontSize = 14, Margin = new Thickness(0, 5, 0, 0) });

            var progressBar = new ProgressBar { Minimum = 0, Maximum = 100, Value = 45, Height = 20, Margin = new Thickness(0, 15, 0, 0) };
            activeStack.Children.Add(progressBar);
            activeStack.Children.Add(new TextBlock { Text = "Прогресс выполнения: 45%", FontSize = 12, Foreground = Brushes.Gray, Margin = new Thickness(0, 5, 0, 0) });

            activeCard.Child = activeStack;
            panel.Children.Add(activeCard);

            panel.Children.Add(new TextBlock { Text = "Активные партии", FontSize = 16, FontWeight = FontWeights.Bold, Margin = new Thickness(0, 0, 0, 10) });

            var dg = new DataGrid { Height = 250, Background = Brushes.White, AutoGenerateColumns = false };
            dg.Columns.Add(new DataGridTextColumn { Header = "Партия", Binding = new System.Windows.Data.Binding("Number"), Width = new DataGridLength(120) });
            dg.Columns.Add(new DataGridTextColumn { Header = "Продукт", Binding = new System.Windows.Data.Binding("Product"), Width = new DataGridLength(150) });
            dg.Columns.Add(new DataGridTextColumn { Header = "Текущий шаг", Binding = new System.Windows.Data.Binding("Step"), Width = new DataGridLength(120) });
            dg.Columns.Add(new DataGridTextColumn { Header = "Прогресс", Binding = new System.Windows.Data.Binding("Progress"), Width = new DataGridLength(100) });
            dg.Columns.Add(new DataGridTextColumn { Header = "Отклонения", Binding = new System.Windows.Data.Binding("Deviations"), Width = new DataGridLength(100) });

            dg.ItemsSource = new[]
            {
                new { Number = "B-2402-01", Product = "Инсектицид Б", Step = "Экструзия", Progress = "45%", Deviations = "1" },
                new { Number = "B-2403-01", Product = "Гербицид А", Step = "Смешивание", Progress = "25%", Deviations = "0" }
            };
            panel.Children.Add(dg);

            ContentContainer.Content = new ScrollViewer { Content = panel };
        }

        // ==================== 8. АНАЛИЗ ОТКЛОНЕНИЙ ====================
        private void ShowDeviations()
        {
            SetActiveButton(btnDeviations, "Анализ отклонений");
            var panel = new StackPanel { Margin = new Thickness(20) };
            panel.Children.Add(new TextBlock { Text = "⚠️ Анализ отклонений и событий", FontSize = 22, FontWeight = FontWeights.Bold, Margin = new Thickness(0, 0, 0, 20) });

            var filterPanel = new Border { Background = Brushes.White, BorderBrush = Brushes.LightGray, BorderThickness = new Thickness(1), CornerRadius = new CornerRadius(8), Padding = new Thickness(15), Margin = new Thickness(0, 0, 0, 15) };
            var filterGrid = new Grid();
            filterGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });
            filterGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });
            filterGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });

            filterGrid.Children.Add(new TextBlock { Text = "Период: 01.03.2025 - 31.03.2025", Margin = new Thickness(5) });
            Grid.SetColumn(filterGrid.Children[0], 0);
            filterGrid.Children.Add(new TextBlock { Text = "Тип: Все отклонения", Margin = new Thickness(5) });
            Grid.SetColumn(filterGrid.Children[1], 1);

            var filterBtn = new Button { Content = "Показать", Width = 100, Height = 30, Background = new SolidColorBrush(Color.FromRgb(33, 150, 243)), Foreground = Brushes.White };
            filterGrid.Children.Add(filterBtn);
            Grid.SetColumn(filterBtn, 2);

            filterPanel.Child = filterGrid;
            panel.Children.Add(filterPanel);

            var dg = new DataGrid { Height = 350, Background = Brushes.White, AutoGenerateColumns = false };
            dg.Columns.Add(new DataGridTextColumn { Header = "Партия", Binding = new System.Windows.Data.Binding("Batch"), Width = new DataGridLength(100) });
            dg.Columns.Add(new DataGridTextColumn { Header = "Шаг", Binding = new System.Windows.Data.Binding("Step"), Width = new DataGridLength(120) });
            dg.Columns.Add(new DataGridTextColumn { Header = "Параметр", Binding = new System.Windows.Data.Binding("Parameter"), Width = new DataGridLength(100) });
            dg.Columns.Add(new DataGridTextColumn { Header = "План", Binding = new System.Windows.Data.Binding("Planned"), Width = new DataGridLength(80) });
            dg.Columns.Add(new DataGridTextColumn { Header = "Факт", Binding = new System.Windows.Data.Binding("Actual"), Width = new DataGridLength(80) });
            dg.Columns.Add(new DataGridTextColumn { Header = "Описание", Binding = new System.Windows.Data.Binding("Description"), Width = new DataGridLength(300) });
            dg.Columns.Add(new DataGridTextColumn { Header = "Дата", Binding = new System.Windows.Data.Binding("Date"), Width = new DataGridLength(150) });

            dg.ItemsSource = new[]
            {
                new { Batch = "B-2401-01", Step = "Экструзия", Parameter = "Температура", Planned = "80°C", Actual = "78.2°C", Description = "Температура ниже нормы", Date = "01.03.2025 10:35" },
                new { Batch = "B-2402-01", Step = "Экструзия", Parameter = "Давление", Planned = "3.5 bar", Actual = "2.9 bar", Description = "Давление ниже нормы", Date = "03.03.2025 09:40" }
            };
            panel.Children.Add(dg);

            ContentContainer.Content = new ScrollViewer { Content = panel };
        }

        // ==================== 9. ОТЧЕТЫ ====================
        private void ShowReports()
        {
            SetActiveButton(btnReports, "Отчеты");
            var panel = new StackPanel { Margin = new Thickness(20) };
            panel.Children.Add(new TextBlock { Text = "📈 Отчеты и аналитика", FontSize = 22, FontWeight = FontWeights.Bold, Margin = new Thickness(0, 0, 0, 20) });

            string[,] reports = new string[,]
            {
                { "📊", "Отчет по партиям за период", "Список всех партий с детализацией" },
                { "⚠️", "Отчет по отклонениям", "Анализ отклонений за период" },
                { "📋", "Отчет по использованию рецептур", "Статистика использования рецептур" },
                { "⚙️", "Отчет по экструдеру", "Параметры работы экструдера" }
            };

            for (int i = 0; i < reports.GetLength(0); i++)
            {
                var card = new Border { Background = Brushes.White, BorderBrush = Brushes.LightGray, BorderThickness = new Thickness(1), CornerRadius = new CornerRadius(8), Padding = new Thickness(15), Margin = new Thickness(0, 0, 0, 10) };
                var stack = new StackPanel();
                stack.Children.Add(new TextBlock { Text = reports[i, 0] + " " + reports[i, 1], FontSize = 16, FontWeight = FontWeights.Bold });
                stack.Children.Add(new TextBlock { Text = reports[i, 2], FontSize = 12, Foreground = Brushes.Gray, Margin = new Thickness(0, 5, 0, 0) });

                var btn = new Button { Content = "Сформировать отчет", Width = 150, Height = 30, Margin = new Thickness(0, 10, 0, 0), Background = new SolidColorBrush(Color.FromRgb(46, 125, 50)), Foreground = Brushes.White };
                stack.Children.Add(btn);
                card.Child = stack;
                panel.Children.Add(card);
            }

            ContentContainer.Content = new ScrollViewer { Content = panel };
        }

        private void Logout()
        {
            if (MessageBox.Show("Вы уверены, что хотите выйти?", "Выход", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
            {
                var login = new LoginWindow();
                login.Show();
                this.Close();
            }
        }

        private Border CreateStatCard(string title, string value, string icon, int column)
        {
            var border = new Border { Background = Brushes.White, BorderBrush = Brushes.LightGray, BorderThickness = new Thickness(1), CornerRadius = new CornerRadius(8), Margin = new Thickness(5), Padding = new Thickness(15) };
            var stack = new StackPanel();
            stack.Children.Add(new TextBlock { Text = icon + " " + title, FontSize = 12, Foreground = Brushes.Gray });
            stack.Children.Add(new TextBlock { Text = value, FontSize = 28, FontWeight = FontWeights.Bold, Foreground = new SolidColorBrush(Color.FromRgb(46, 125, 50)) });
            border.Child = stack;
            Grid.SetColumn(border, column);
            return border;
        }
    }
}