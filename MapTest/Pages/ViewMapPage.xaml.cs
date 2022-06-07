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
        public event MapScale MapChangedEvent;

        private ObservableCollection<TemperaturePointModel> _list;

        public ViewMapPage()
        {
            InitializeComponent();
            _list = new ObservableCollection<TemperaturePointModel>();
            ViewDG.ItemsSource = _list;
        }

        public void UpdateDataGrid(IEnumerable<TemperaturePointModel> list)
        {
            foreach (var point in list)
                if (!_list.Contains(point))
                    _list.Add(point);
            
        }

        private void ViewDG_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            TemperaturePointModel point = (TemperaturePointModel)ViewDG.SelectedItem;
            MapChangedEvent?.Invoke(point.X, point.Y);
        }
    }
}
