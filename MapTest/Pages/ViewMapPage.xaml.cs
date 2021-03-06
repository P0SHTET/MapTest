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
    /// Логика взаимодействия для ViewMapPage.xaml
    /// </summary>
    public partial class ViewMapPage : Page
    {
        public delegate void MapScale(double x, double y);
        public event MapScale MapScaleEvent;

        public delegate void DisplayGraph(string pointName);
        public event DisplayGraph DisplayGraphEvent;

        private readonly ObservableCollection<TemperaturePointModel> _list;

        public ViewMapPage()
        {
            InitializeComponent();
            _list = new ObservableCollection<TemperaturePointModel>();
            ViewDG.ItemsSource = _list;
        }

        public void UpdateDataGrid(IEnumerable<TemperaturePointModel> list)
        {
            _list.Clear();
            foreach (var point in list)
                if (point.Name != null && point.Name.Length>0)
                    _list.Add(point);
            
        }

        private void ViewDG_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            TemperaturePointModel point = (TemperaturePointModel)ViewDG.SelectedItem;
            MapScaleEvent?.Invoke(point.X, point.Y);
        }

        private void SearchBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (string.IsNullOrEmpty(SearchBox.Text))
            {
                ViewDG.ItemsSource = _list;
                return;
            }
            ViewDG.ItemsSource = _list.Where(x => x.Name.ToLower().Contains(SearchBox.Text.ToLower()));
        }

        private void MenuItem_Click(object sender, RoutedEventArgs e)
        {
            DisplayGraphEvent?.Invoke(((TemperaturePointModel)ViewDG.SelectedItem).Name);
        }
    }
}
