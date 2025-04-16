using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.NetworkInformation;
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
using ZXing;
using ZXing.QrCode;
using ZXing.Common;
using ZXing.Rendering;
using System.IO;
using System.Drawing;
using QRCoder;
using static System.Net.WebRequestMethods;


namespace Traincore.Pages
{
    /// <summary>
    /// Логика взаимодействия для TicketDonePage.xaml
    /// </summary>
    public partial class TicketDonePage : Page
    {
        public TicketDonePage(Schedule schedule, Wagons wagon, Seats seat, DateTime date, Tickets ticket)
        {
            InitializeComponent();

            // Устанавливаем DataContext с данными билета
            DataContext = new TicketDoneViewModel
            {
                TrainInfo = $"{schedule.Trains.Number_train} {schedule.Trains.Name_train}",
                RouteInfo = $"{schedule.Marshrut.Stations.Name_Station} → {schedule.Marshrut.Stations1.Name_Station}",
                DateInfo = date.ToString("dd.MM.yyyy"),
                WagonInfo = $"Вагон №{wagon.Number_wagon} ({wagon.Type_Wagons.Name_type_wagon})",
                SeatInfo = $"Место {seat.Number_seats}",
                Price = $"{seat.Price} руб.",
                TicketNumber = $"TK-{ticket.ID_Ticket:000000}"
            };

            GenerateQrCode(ticket.ID_Ticket);
        }

        private void GenerateQrCode(int ticketId)
        {
            string qrContent = "https://static.tildacdn.com/tild3530-3231-4431-b735-396339363564/81d8b70ee26f614b8150.jpg";

            try
            {
                using (var qrGenerator = new QRCodeGenerator())
                using (var qrCodeData = qrGenerator.CreateQrCode(qrContent, QRCodeGenerator.ECCLevel.Q))
                using (var qrCode = new QRCode(qrCodeData))
                using (var bitmap = qrCode.GetGraphic(20))
                using (var memoryStream = new MemoryStream())
                {
                    bitmap.Save(memoryStream, System.Drawing.Imaging.ImageFormat.Png);
                    memoryStream.Position = 0;

                    var bitmapImage = new BitmapImage();
                    bitmapImage.BeginInit();
                    bitmapImage.StreamSource = memoryStream;
                    bitmapImage.CacheOption = BitmapCacheOption.OnLoad;
                    bitmapImage.EndInit();

                    QrCodeImage.Source = bitmapImage;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка генерации QR-кода: {ex.Message}");
                QrCodeImage.Source = null;
            }
        }

        private void HomeButton_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new AutorisationPage());
        }

        public class TicketDoneViewModel
        {
            public string TrainInfo { get; set; }
            public string RouteInfo { get; set; }
            public string DateInfo { get; set; }
            public string WagonInfo { get; set; }
            public string SeatInfo { get; set; }
            public string Price { get; set; }
            public string TicketNumber { get; set; }
            public BitmapImage QrCodeImage { get; set; }
        }
    }
}
