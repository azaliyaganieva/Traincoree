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

namespace Traincore.Pages
{
    /// <summary>
    /// Логика взаимодействия для StartEndDatePage.xaml
    /// </summary>
    public partial class StartEndDatePage : Page
    {
        public StartEndDatePage()
        {
            InitializeComponent();
        }

        

        private void btnDalee_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new TrainSelectionPage());
            string fromStation = FromComboBox.SelectedItem.ToString();
            string toStation = ToComboBox.SelectedItem.ToString();
            DateTime date = Datetxb.SelectedDate.Value;

            if (string.IsNullOrEmpty(fromStation) || string.IsNullOrEmpty(toStation))
            {
                MessageBox.Show("Пожалуйста, выберите станции отправления и прибытия.");
                return;
            }

            
           
        }
    }
}
