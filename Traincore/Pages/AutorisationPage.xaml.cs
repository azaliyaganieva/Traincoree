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
    /// Логика взаимодействия для AutorisationPage.xaml
    /// </summary>
    public partial class AutorisationPage : Page
    {
        public AutorisationPage()
        {
            InitializeComponent();
        }

        private void btnReg_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new RegistrationPage());
        }

        private void btnVhod_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new VhodPage());
        }
    }
}
