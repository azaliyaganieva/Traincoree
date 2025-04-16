using System;
using System.Collections.Generic;
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
using Traincore.Connection;

namespace Traincore.Pages
{
    /// <summary>
    /// Логика взаимодействия для TicketBookingPage.xaml
    /// </summary>
    public partial class TicketBookingPage : Page
    {
        private readonly Schedule _schedule;
        private readonly Wagons _wagon;
        private readonly Seats _seat;
        private readonly DateTime _date;

        public TicketBookingPage(Schedule schedule, Wagons wagon, Seats seat, DateTime date)
        {
            InitializeComponent();

            // Сохраняем параметры в поля класса
            _schedule = schedule;
            _wagon = wagon;
            _seat = seat;
            _date = date;

            // Если DataContext еще не установлен (передан из PassengerInfoPage)
            if (DataContext == null)
            {
                DataContext = new TicketBookingViewModel
                {
                    TrainInfo = $"{_schedule.Trains.Number_train} {_schedule.Trains.Name_train}",
                    RouteInfo = $"{_schedule.Marshrut.Stations.Name_Station} → {_schedule.Marshrut.Stations1.Name_Station}",
                    DateInfo = _date.ToString("dd.MM.yyyy"),
                    WagonInfo = $"Вагон №{_wagon.Number_wagon} ({_wagon.Type_Wagons.Name_type_wagon})",
                    SeatInfo = $"Место {_seat.Number_seats}",
                    Price = $"{_seat.Price} руб."
                };
            }
        }

        // Остальные методы остаются без изменений
        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.GoBack();
        }

        private void BookButton_Click(object sender, RoutedEventArgs e)
        {
            var viewModel = (TicketBookingViewModel)DataContext;

            if (string.IsNullOrWhiteSpace(viewModel.LastName))
            {
                MessageBox.Show("Введите фамилию пассажира");
                return;
            }

            if (string.IsNullOrWhiteSpace(viewModel.FirstName))
            {
                MessageBox.Show("Введите имя пассажира");
                return;
            }

            try
            {
                using (var db = new TrainEntities2())
                using (var transaction = db.Database.BeginTransaction())
                {
                    try
                    {
                        // 1. Обрабатываем пассажира
                        var passenger = db.Passangers.FirstOrDefault(p =>
                            p.Number_passport == viewModel.PassportNumber);

                        if (passenger == null)
                        {
                            passenger = new Passangers
                            {
                                Name_Pas = viewModel.FirstName,
                                Fam_Pas = viewModel.LastName,
                                Number_passport = viewModel.PassportNumber
                            };
                            db.Passangers.Add(passenger);
                            db.SaveChanges();
                        }

                        // 2. Загружаем место в текущем контексте
                        var seat = db.Seats.Find(_seat.ID_Seat);
                        if (seat == null)
                        {
                            throw new Exception("Место не найдено");
                        }

                        // 3. Проверяем доступность места
                        if (!seat.IsAvailable.GetValueOrDefault())
                        {
                            // Если место занято, предлагаем выбрать другое
                            if (MessageBox.Show("Это место уже занято. Хотите выбрать другое место?",
                                "Место занято",
                                MessageBoxButton.YesNo) == MessageBoxResult.Yes)
                            {
                                NavigationService.Navigate(new SeatSelectionPage(_schedule, _date));
                                return;
                            }
                            else
                            {
                                return;
                            }
                        }

                        // 4. Помечаем место как занятое
                        seat.IsAvailable = false;

                        // 5. Создаем билет
                        var ticket = new Tickets
                        {
                            ID_Passanger = passenger.ID_Passanger,
                            ID_Seat = seat.ID_Seat,
                            ID_Schedule = _schedule.ID_Schedule,
                            Stoimost = seat.Price,
                            Date_buy = DateTime.Now
                        };
                        db.Tickets.Add(ticket);

                        db.SaveChanges();
                        transaction.Commit();

                        // 6. Переходим на страницу подтверждения
                        NavigationService.Navigate(new TicketDonePage(
                            _schedule,
                            _wagon,
                            seat,
                            _date,
                            ticket));
                    }
                    catch (Exception ex)
                    {
                        transaction.Rollback();
                        MessageBox.Show($"Ошибка: {ex.Message}", "Ошибка",
                            MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка: {ex.Message}", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }

    }

    public class TicketBookingViewModel
        {
            public string TrainInfo { get; set; }
            public string RouteInfo { get; set; }
            public string DateInfo { get; set; }
            public string WagonInfo { get; set; }
            public string SeatInfo { get; set; }
            public string Price { get; set; }
            public string LastName { get; set; }
            public string FirstName { get; set; }
            public string PassportNumber { get; set; }
            public bool CanBook => !string.IsNullOrWhiteSpace(LastName) &&
                                 !string.IsNullOrWhiteSpace(FirstName);
        }
    


