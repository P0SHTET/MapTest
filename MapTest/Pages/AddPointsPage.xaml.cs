using ExcelUtils;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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

namespace MapTest.Pages
{
    /// <summary>
    /// Логика взаимодействия для AddPointsPage.xaml
    /// </summary>
    public partial class AddPointsPage : Page
    {
        public delegate void ClearAddPoints();
        public event ClearAddPoints ClearAddPointsEvent;

        public delegate void AddNewPoints(IEnumerable<TemperaturePointModel> points);
        public event AddNewPoints AddNewPointsEvent;

        public ObservableCollection<TemperaturePointModel> Points { get; set; }

        public AddPointsPage()
        {
            InitializeComponent();
            
            Points = new ObservableCollection<TemperaturePointModel>();
            
            ViewDG.ItemsSource = Points;
        }

        public void AddPoint(double x, double y)
        {
            Points.Add(new TemperaturePointModel($"{Points.Count+1}",x,y,null));
            SaveBut.IsEnabled = true;
        }

        private void CancelBut_Click(object sender, RoutedEventArgs e)
        {
            Points.Clear();
            SaveBut.IsEnabled = false;
            ClearAddPointsEvent?.Invoke();
        }

        private void SaveBut_Click(object sender, RoutedEventArgs e)
        {
            AddNewPointsEvent?.Invoke(Points);
            Points.Clear();
            SaveBut.IsEnabled = false;
        }
    }
}
