using System.Windows;

using Esri.ArcGISRuntime.Geometry;
using Esri.ArcGISRuntime.Mapping;
using Esri.ArcGISRuntime.Symbology;
using Esri.ArcGISRuntime.UI;

using System.ComponentModel;
using System.Runtime.CompilerServices;
using Esri.ArcGISRuntime.Data;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Esri.ArcGISRuntime.UI.Controls;
using System.Drawing;
using System.Windows.Media;
using Windows.UI.Popups;
using MapTest.MapMaster;
using ExcelUtils;
using System.IO;
using Microsoft.Win32;

namespace MapTest
{

    public partial class MainWindow : Window
    {
        private MapController _mapController;
        private ExcelUtil _excelUtil;
        private List<MapPoint> _inputPointCollection;
        private GraphicsOverlay _graphicsOverlayAddPoints;
        private Graphic? _selectedPointGraphics;
        private readonly Brush _defaultColor;

        public double MaxTemp { get; set; }
        public double MinTemp { get; set; }

        public MainWindow()
        {
            _mapController = new MapController();
            _inputPointCollection = new List<MapPoint>();
            _graphicsOverlayAddPoints = new GraphicsOverlay() { Id = "addPoints"};
            

            OpenFileDialog dialog = new OpenFileDialog()
            {
                Multiselect = false,
                Filter = "Excel Files|*.xls;*.xlsx;*.xlsm"
            };
            
            if (dialog.ShowDialog()??false)            
                _excelUtil = new ExcelUtil(dialog.FileName);            
            else            
                _excelUtil = new ExcelUtil();
            

            InitializeComponent();

            _defaultColor = AddBut.Background;

            MyMapView.Map = new Map(BasemapStyle.ArcGISTerrain);

            var data = _excelUtil.GetPointsData();

            MaxTemp = _excelUtil.GetMaxTemp();
            MinTemp = _excelUtil.GetMinTemp();

            _mapController.MapChanged += _mapController_MapChanged;

            _mapController.UpdateGraphics(data, MaxTemp, MinTemp);
        }

        private void _mapController_MapChanged(GraphicsOverlay polygons, GraphicsOverlay points)
        {
            MyMapView.GraphicsOverlays.Clear();
            MyMapView.GraphicsOverlays.Add(polygons);
            MyMapView.GraphicsOverlays.Add(points);

        }

        private void MyMapView_GeoViewTapped_Add(object sender, GeoViewInputEventArgs e)
        {
            try
            {
                var centralizedPoint = (MapPoint)GeometryEngine.NormalizeCentralMeridian(e.Location);
                _inputPointCollection.Add(centralizedPoint);
                
                SimpleMarkerSymbol userTappedSimpleMarkerSymbol = new SimpleMarkerSymbol(SimpleMarkerSymbolStyle.Circle, System.Drawing.Color.Red, 10);

                Graphic userTappedGraphic = new Graphic(e.Location, new Dictionary<string, object>
                {
                    { "Type", "Point" }
                }, userTappedSimpleMarkerSymbol)
                { ZIndex = 0 };
                
                userTappedGraphic.ZIndex = 1;

                MyMapView.GraphicsOverlays["addPoints"].Graphics.Add(userTappedGraphic);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Can't add user tapped graphic!");
            }
        }
        
        private void MyMapView_GeoViewTapped_Edit(object? sender, GeoViewInputEventArgs e)
        {
            double minDistance = double.MaxValue;
            double distance;

            foreach(var point in MyMapView.GraphicsOverlays["dataPoint"].Graphics)
            {
                

                if (point.Geometry.GeometryType != GeometryType.Point) continue;

                var convertGeometry = GeometryEngine.Project(point.Geometry, e.Location.SpatialReference);

                distance = GeometryEngine.Distance(e.Location, convertGeometry);
                if (distance < minDistance)
                {
                    minDistance = distance;
                    _selectedPointGraphics = point;
                }
            }

            MyMapView.SetViewpointAsync(new Viewpoint((MapPoint)_selectedPointGraphics.Geometry,100000));
        }

        private void ViewBut_Click(object sender, RoutedEventArgs e)
        {
            MyMapView.GeoViewTapped -= MyMapView_GeoViewTapped_Edit;
            MyMapView.GeoViewTapped -= MyMapView_GeoViewTapped_Add;

            if (MyMapView.GraphicsOverlays.Contains(_graphicsOverlayAddPoints))
                MyMapView.GraphicsOverlays.Remove(_graphicsOverlayAddPoints);

            ViewBut.Background = Brushes.DarkGray;
            AddBut.Background = _defaultColor;
            EditBut.Background = _defaultColor;
        }

        private void AddBut_Click(object sender, RoutedEventArgs e)
        {
            MyMapView.GeoViewTapped -= MyMapView_GeoViewTapped_Edit;
            MyMapView.GeoViewTapped += MyMapView_GeoViewTapped_Add;

            if (!MyMapView.GraphicsOverlays.Contains(_graphicsOverlayAddPoints))
                MyMapView.GraphicsOverlays.Add(_graphicsOverlayAddPoints);

            ViewBut.Background = _defaultColor;
            AddBut.Background = Brushes.DarkGray;
            EditBut.Background = _defaultColor;
        }

        private void EditBut_Click(object sender, RoutedEventArgs e)
        {
            MyMapView.GeoViewTapped += MyMapView_GeoViewTapped_Edit;
            MyMapView.GeoViewTapped -= MyMapView_GeoViewTapped_Add;

            if(MyMapView.GraphicsOverlays.Contains(_graphicsOverlayAddPoints))
                MyMapView.GraphicsOverlays.Remove(_graphicsOverlayAddPoints);

            ViewBut.Background = _defaultColor;
            AddBut.Background = _defaultColor;
            EditBut.Background = Brushes.DarkGray;
        }
    }
}



