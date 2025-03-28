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
    /// Логика взаимодействия для TrainSelectionPage.xaml
    /// </summary>
    public partial class TrainSelectionPage : Page
    {
        public class Train
        {
            public string Number_train { get; set; }
            public string Date_Start { get; set; }
            public string Time_Start { get; set; }
            public string Date_finish { get; set; }
            public string Time_finish { get; set; }
            public string Name_type_train { get; set; }
            public int IsAvailable { get; set; }
            public decimal Price { get; set; }
        }
        public TrainSelectionPage()
        {
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (TrainsListView.SelectedItem != null)
            {
                Train selectedTrain = (Train)TrainsListView.SelectedItem;
                MessageBox.Show($"Выбран поезд: {selectedTrain.Number_train}");
            }
            }
    }
}
