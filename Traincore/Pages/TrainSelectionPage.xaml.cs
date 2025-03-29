using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data.Entity;
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
using Traincore.Connection;
using static System.Collections.Specialized.BitVector32;

namespace Traincore.Pages
{
    /// <summary>
    /// Логика взаимодействия для TrainSelectionPage.xaml
    /// </summary>
    public partial class TrainSelectionPage : Page
    {
            private List<Schedule> _schedules;
        private List<Stations> _stations;
        private DateTime _selectedDate = DateTime.Today;

        public TrainSelectionPage()
        {
            InitializeComponent();
            LoadStations();
            DatePicker.SelectedDate = _selectedDate;
        }

        private void LoadStations()
        {
            try
            {
                _stations = DBconnect.trainEntities.Stations.ToList();
                StartStationComboBox.ItemsSource = _stations;
                EndStationComboBox.ItemsSource = _stations;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при загрузке станций: {ex.Message}");
            }
        }

        private void SearchTrainsButton_Click(object sender, RoutedEventArgs e)
        {
            if (StartStationComboBox.SelectedItem == null || EndStationComboBox.SelectedItem == null)
            {
                MessageBox.Show("Пожалуйста, выберите станции отправления и назначения");
                return;
            }

            if (DatePicker.SelectedDate == null)
            {
                MessageBox.Show("Пожалуйста, выберите дату");
                return;
            }

            var startStation = (Stations)StartStationComboBox.SelectedItem;
            var endStation = (Stations)EndStationComboBox.SelectedItem;
            _selectedDate = DatePicker.SelectedDate.Value;

            try
            {
                _schedules = DBconnect.trainEntities.Schedule
                    .Include(s => s.Marshrut)
                    .Include(s => s.Trains)
                    .Include(s => s.Marshrut.Stations) // Start station
                    .Include(s => s.Marshrut.Stations) // End station
                    .Where(s => s.Marshrut.ID_start_station == startStation.ID_Station &&
                                s.Marshrut.ID_finish_station == endStation.ID_Station &&
                                DbFunctions.TruncateTime(s.Date_Start) == _selectedDate.Date)
                    .ToList();

                TrainsDataGrid.ItemsSource = _schedules.Select(s => new
                {
                    TrainNumber = s.Trains.Number_train,
                    TrainName = s.Trains.Name_train,
                    DepartureTime = s.Time_start,
                    ArrivalTime = s.Time_finish,
                    DepartureDate = s.Date_Start,
                    ArrivalDate = s.Date_finish,
                    StartStation = s.Marshrut.Stations.Name_Station,
                    EndStation = s.Marshrut.Stations.Name_Station,
                    ScheduleId = s.ID_Schedule
                }).ToList();

                if (!_schedules.Any())
                {
                    MessageBox.Show("На выбранную дату поездов не найдено");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при поиске поездов: {ex.Message}");
            }
        }

        private void SelectTrainButton_Click(object sender, RoutedEventArgs e)
        {
            
            if (TrainsDataGrid.SelectedItem == null)
            {
                MessageBox.Show("Пожалуйста, выберите поезд");
                return;
            }

            try
            {
                dynamic selectedItem = TrainsDataGrid.SelectedItem;
                int scheduleId = selectedItem.ScheduleId;
                var selectedSchedule = (Schedule)TrainsDataGrid.SelectedItem;
                var seatSelectionPage = new SeatSelectionPage(selectedSchedule, _selectedDate);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при переходе к выбору мест: {ex.Message}");
            }
        }
    }
    
}
