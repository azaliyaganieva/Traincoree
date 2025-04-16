using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
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
using static Traincore.Pages.TicketBookingPage;

namespace Traincore.Pages
{
    /// <summary>
    /// Логика взаимодействия для PassangerInfoPage.xaml
    /// </summary>
    public partial class PassengerInfoPage : Page
    {
        private readonly Schedule _selectedSchedule;
        private readonly Seats _selectedSeat;
        private readonly DateTime _selectedDate;
        private readonly Wagons _selectedWagon;

        public PassengerInfoPage(Schedule selectedSchedule, Seats selectedSeat, Wagons selectedWagon, DateTime selectedDate)
        {
            InitializeComponent();
            _selectedSchedule = selectedSchedule;
            _selectedSeat = selectedSeat;
            _selectedWagon = selectedWagon;
            _selectedDate = selectedDate;
            DataContext = new PassengerData(this);
        }

        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.GoBack();
        }

        public void NavigateToTicketBooking()
        {
            var passengerData = (PassengerData)DataContext;

            if (!passengerData.IsDataValid)
            {
                MessageBox.Show("Заполните все обязательные поля");
                return;
            }

            // Создаем TicketBookingPage и передаем все данные
            var ticketBookingPage = new TicketBookingPage(
                _selectedSchedule,
                _selectedWagon,
                _selectedSeat,
                _selectedDate);

            // Устанавливаем DataContext с данными пассажира
            ticketBookingPage.DataContext = new TicketBookingViewModel
            {
                TrainInfo = $"{_selectedSchedule.Trains.Number_train} {_selectedSchedule.Trains.Name_train}",
                RouteInfo = $"{_selectedSchedule.Marshrut.Stations.Name_Station} → {_selectedSchedule.Marshrut.Stations1.Name_Station}",
                DateInfo = _selectedDate.ToString("dd.MM.yyyy"),
                WagonInfo = $"Вагон №{_selectedWagon.Number_wagon} ({_selectedWagon.Type_Wagons.Name_type_wagon})",
                SeatInfo = $"Место {_selectedSeat.Number_seats}",
                Price = $"{_selectedSeat.Price} руб.",
                LastName = passengerData.LastName,
                FirstName = passengerData.FirstName,
                PassportNumber = passengerData.PassportNumber
            };

            NavigationService.Navigate(ticketBookingPage);
        }

        private void Return_Click(object sender, RoutedEventArgs e)
        {
           
        }
    }

    public class PassengerData : INotifyPropertyChanged
    {
        private readonly PassengerInfoPage _page;
        private string _lastName;
        private string _firstName;
        private string _passportNumber;
        private string _phoneNumber;


        public string LastName
        {
            get => _lastName;
            set { _lastName = value; OnPropertyChanged(); OnPropertyChanged(nameof(IsDataValid)); }
        }

        public string FirstName
        {
            get => _firstName;
            set { _firstName = value; OnPropertyChanged(); OnPropertyChanged(nameof(IsDataValid)); }
        }

        public string PassportNumber
        {
            get => _passportNumber;
            set { _passportNumber = value; OnPropertyChanged(); OnPropertyChanged(nameof(IsDataValid)); }
        }

        public string PhoneNumber
        {
            get => _phoneNumber;
            set { _phoneNumber = value; OnPropertyChanged(); OnPropertyChanged(nameof(IsDataValid)); }
        }

        public bool IsDataValid =>
            !string.IsNullOrWhiteSpace(LastName) &&
            !string.IsNullOrWhiteSpace(FirstName) &&
            !string.IsNullOrWhiteSpace(PassportNumber) &&
            !string.IsNullOrWhiteSpace(PhoneNumber);

        public ICommand ContinueCommand { get; }

        public PassengerData(PassengerInfoPage page)
        {
            _page = page;
            ContinueCommand = new RelayCommand(_ => _page.NavigateToTicketBooking());
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }

    public class RelayCommand : ICommand
    {
        private readonly Action<object> _execute;
        private readonly Func<object, bool> _canExecute;

        public RelayCommand(Action<object> execute, Func<object, bool> canExecute = null)
        {
            _execute = execute;
            _canExecute = canExecute;
        }

        public bool CanExecute(object parameter) => _canExecute == null || _canExecute(parameter);

        public void Execute(object parameter) => _execute(parameter);

        public event EventHandler CanExecuteChanged
        {
            add => CommandManager.RequerySuggested += value;
            remove => CommandManager.RequerySuggested -= value;
        }
    }
}
