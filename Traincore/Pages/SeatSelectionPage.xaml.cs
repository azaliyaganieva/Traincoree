using System;
using System.Collections.Generic;
using System.Data.Entity.Infrastructure;
using System.Data.Entity.Validation;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Traincore;
using Traincore.Connection;


namespace Traincore.Pages
{
    /// <summary>
    /// Логика взаимодействия для SeatSelectionPage.xaml
    /// </summary>
    public partial class SeatSelectionPage : Page
    {
        private Schedule _selectedSchedule;
        private Seats _selectedSeat;
        private DateTime _selectedDate; // Добавляем поле для даты
        private Wagons _selectedWagon;
        private List<Wagons> _wagons;


        public SeatSelectionPage(Schedule selectedSchedule, DateTime selectedDate)
        {
            InitializeComponent();

            if (selectedSchedule == null)
                throw new ArgumentNullException(nameof(selectedSchedule));

            _selectedSchedule = selectedSchedule;
            _selectedDate = selectedDate;

            LoadTrainInfo(_selectedSchedule.ID_Train); // Теперь метод существует
            LoadWagons();
        }
        private void LoadTrainInfo(int trainId)
        {
            try
            {
                // Загружаем информацию о поезде
                var train = DBconnect.trainEntities.Trains
                    .FirstOrDefault(t => t.ID_Train == trainId);

                if (train != null)
                {
                    TrainInfoTextBlock.Text = $"{train.Number_train} {train.Name_train}";
                }

                // Загружаем информацию о маршруте
                var route = DBconnect.trainEntities.Marshrut
                    .Include("Stations")    // Станция отправления
                    .Include("Stations1")  // Станция назначения
                    .FirstOrDefault(m => m.ID_Route == _selectedSchedule.ID_Route);

                if (route != null)
                {
                    RouteInfoTextBlock.Text = $"{route.Stations.Name_Station} → {route.Stations1.Name_Station}";
                }

                DateInfoTextBlock.Text = _selectedDate.ToString("dd.MM.yyyy");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при загрузке информации о поезде: {ex.Message}");
            }
        }
        private void LoadData()
        {
            try
            {
                // Загружаем полную информацию о расписании
                var fullSchedule = DBconnect.trainEntities.Schedule
                .Include("Trains")
                .Include("Marshrut.Stations")  // станция отправления
                .Include("Marshrut.Stations1") // станция назначения
                .FirstOrDefault(s => s.ID_Schedule == _selectedSchedule.ID_Schedule);


                if (fullSchedule == null)
                {
                    MessageBox.Show("Не удалось загрузить данные о поезде");
                    NavigationService?.GoBack();
                    return;
                }

                // Обновляем _selectedSchedule
                _selectedSchedule = fullSchedule;

                // Отображаем информацию о поезде
                TrainInfoTextBlock.Text = $"{_selectedSchedule.Trains.Number_train} {_selectedSchedule.Trains.Name_train}";
                RouteInfoTextBlock.Text = $"{_selectedSchedule.Marshrut.Stations.Name_Station} → {_selectedSchedule.Marshrut.Stations1.Name_Station}";
                DateInfoTextBlock.Text = _selectedSchedule.Date_Start.ToString("dd.MM.yyyy");


                // Загружаем вагоны
                LoadWagons();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при загрузке данных: {ex.Message}");
                NavigationService?.GoBack();
            }
        }

        private void LoadWagons()
        {
            try
            {
                _wagons = DBconnect.trainEntities.Wagons
                    .Include("Type_Wagons")
                    .Where(w => w.ID_Train == _selectedSchedule.ID_Train)
                    .OrderBy(w => w.Number_wagon)
                    .ToList();

                if (_wagons.Count == 0)
                {
                    MessageBox.Show("Для этого поезда не найдено вагонов");
                    return;
                }

                WagonsListBox.ItemsSource = _wagons;
                WagonsListBox.DisplayMemberPath = "Number_wagon";
                WagonsListBox.SelectedIndex = 0; // Автоматически выбираем первый вагон
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при загрузке вагонов: {ex.Message}");
            }
        }

        private void LoadSeats(int wagonId)
        {
            try
            {
                var seats = DBconnect.trainEntities.Seats
                    .Where(s => s.ID_Wagon == wagonId) // Убираем условие IsAvailable
                    .OrderBy(s => s.Number_seats)
                    .ToList();

                SeatsItemsControl.ItemsSource = seats;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при загрузке мест: {ex.Message}");
            }
        }

        private void WagonsListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            _selectedWagon = WagonsListBox.SelectedItem as Wagons;
            if (_selectedWagon == null) return;

            LoadSeats(_selectedWagon.ID_Wagon);
        }

        private void SeatButton_Click(object sender, RoutedEventArgs e)
        {
            if (((Button)sender).DataContext is Seats seat)
            {
                if ((bool)!seat.IsAvailable)
                {
                    MessageBox.Show("Это место уже занято, выберите другое");
                    return;
                }

                _selectedSeat = seat;
                BookButton.IsEnabled = true;
                MessageBox.Show($"Вы выбрали место номер {seat.Number_seats}");
            }
        }



        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            if (_selectedSeat == null || _selectedWagon == null)
            {
                MessageBox.Show("Пожалуйста, выберите вагон и место");
                return;
            }
        }

        private void BookButton_Click(object sender, RoutedEventArgs e)
        {
            if (_selectedSeat == null || _selectedWagon == null)
            {
                MessageBox.Show("Выберите место и вагон");
                return;
            }

            try
            {
                // Сразу переходим на страницу ввода данных пассажира
                NavigationService.Navigate(new PassengerInfoPage(
                    _selectedSchedule,
                    _selectedSeat,
                    _selectedWagon,
                    _selectedDate));
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка: {ex.Message}", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}            
            
    


    


