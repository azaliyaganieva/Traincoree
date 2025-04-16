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
    /// Логика взаимодействия для TicketConfirmationPage.xaml
    /// </summary>
    public partial class TicketConfirmationPage : Page
    {
        public TicketConfirmationPage(Tickets ticket)
        {
            InitializeComponent();

            DataContext = new
            {
                TicketNumber = $"Номер билета: {ticket.ID_Ticket}",
                PassengerName = $"Пассажир: {ticket.Passangers.Fam_Pas} {ticket.Passangers.Name_Pas}",
                TrainInfo = $"Поезд: {ticket.Schedule.Trains.Number_train} {ticket.Schedule.Trains.Name_train}",
                RouteInfo = $"Маршрут: {ticket.Schedule.Marshrut.Stations.Name_Station} → {ticket.Schedule.Marshrut.Stations1.Name_Station}",
                DateInfo = $"Дата: {ticket.Schedule.Date_Start.ToString("dd.MM.yyyy")}",
                SeatInfo = $"Место: Вагон {ticket.Seats.Wagons.Number_wagon}, Место {ticket.Seats.Number_seats}",
                Price = $"Стоимость: {ticket.Stoimost} руб."
            };
        }

        private void HomeButton_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}
