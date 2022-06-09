using Esri.ArcGISRuntime.Geometry;
using ExcelUtils;
using System;
using System.Linq;
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
    /// Логика взаимодействия для EditPointPage.xaml
    /// </summary>
    public partial class EditPointPage : Page
    {
        public delegate void MapScale(double x, double y);
        public event MapScale MapScaleEvent;

        public delegate void SaveChangePoint(ICollection<TemperaturePointModel> points);
        public event SaveChangePoint SaveChangePointEvent;

        public ObservableCollection<TemperaturePointModel> Points { get; set; }

        private ObservableCollection<TemperaturePointModel> _truePoints;

        public EditPointPage()
        {
            InitializeComponent();

            Points = new ObservableCollection<TemperaturePointModel>();
            _truePoints = new ObservableCollection<TemperaturePointModel>();

            ViewDG.ItemsSource = Points;            
        }

        public void SelectPoint(MapPoint point)
        {
            foreach (var p in Points)
                if (p.X == point.X && p.Y == point.Y)
                { 
                    ViewDG.SelectedItem = p; 
                    break;
                }


        }

        public void UpdateDataGrid(IEnumerable<TemperaturePointModel> points)
        {
            Points.Clear();
            _truePoints.Clear();

            foreach (var point in points) 
            {
                Points.Add(new TemperaturePointModel(point));
                _truePoints.Add(point);
            }
        }

        private void SaveBut_Click(object sender, RoutedEventArgs e)
        {
            
            _truePoints.Clear();
            foreach (var point in Points)
            {
                _truePoints.Add(new TemperaturePointModel(point));
            }

            SaveChangePointEvent?.Invoke(_truePoints);
        }

        private void CancelBut_Click(object sender, RoutedEventArgs e)
        {
            Points.Clear();

            foreach (var point in _truePoints)            
                Points.Add(new TemperaturePointModel(point));            
        }

        private void ViewDG_SelectedCellsChanged(object sender, SelectedCellsChangedEventArgs e)
        {

            TemperaturePointModel point = (TemperaturePointModel)ViewDG.SelectedItem;
            if (point is null) return;
            MapScaleEvent?.Invoke(point.X, point.Y);
            

        }

        private void RemoveBut_Click(object sender, RoutedEventArgs e)
        {
            Points.Remove((TemperaturePointModel)ViewDG.SelectedItem);
        }


    }
}
