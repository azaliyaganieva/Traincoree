using System;
using System.Collections.Generic;
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


namespace Traincore.Pages
{
    /// <summary>
    /// Логика взаимодействия для SeatSelectionPage.xaml
    /// </summary>
    public partial class SeatSelectionPage : Page
    {
        private  Schedule _selectedSchedule;
        private  Schedule  _scheduleId;
        private   DateTime _selectedDate;
        private Seats selectedSeat;
        private List<Wagons> wagons;

        public SeatSelectionPage(Schedule selectedSchedule, DateTime selectedDate)
        {
            _selectedSchedule = selectedSchedule;
            _selectedDate =  selectedDate;
            LoadTrainInfo();
            LoadWagons();
        }
        private void LoadTrainInfo()
        {
            TrainInfoTextBlock.Text = $"{_selectedSchedule.Trains.Number_train} {_selectedSchedule.Trains.Name_train}";
            RouteInfoTextBlock.Text = $"{_selectedSchedule.Marshrut.Stations.Name_Station} - {_selectedSchedule.Marshrut.Stations.Name_Station}";
            DateInfoTextBlock.Text = _selectedSchedule.Date_Start.ToString("dd.MM.yyyy");
        }

        private void LoadWagons()
        {
            try
            {
                wagons = DBconnect.trainEntities.Wagons
                .Include("Type_Wagon")  
                .Where(w => w.ID_Train == _selectedSchedule.ID_Train)
                .ToList();

                WagonsListBox.ItemsSource = wagons;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при загрузке вагонов: {ex.Message}");
            }
        }

        private void WagonsListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (WagonsListBox.SelectedItem == null) return;

            var selectedWagon = (Wagons)WagonsListBox.SelectedItem;

            try
            {
                var seats = DBconnect.trainEntities.Seats
                    .Where(s => s.ID_Wagon == selectedWagon.ID_Wagon &&
                               s.IsAvailable == true)
                    .ToList();

                SeatsItemsControl.ItemsSource = seats;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при загрузке мест: {ex.Message}");
            }
        }

        private void SeatButton_Click(object sender, RoutedEventArgs e)
        {
            selectedSeat = (Seats)((Button)sender).DataContext;
            BookButton.IsEnabled = true;
        }

        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.GoBack();
        }

        private void BookButton_Click(object sender, RoutedEventArgs e)
        {
            if (selectedSeat == null)
            {
                MessageBox.Show("Выберите место");
                return;
            }

           
        }
    }
}

