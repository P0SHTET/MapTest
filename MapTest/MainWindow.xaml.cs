using System.Windows;

using Esri.ArcGISRuntime.Geometry;
using Esri.ArcGISRuntime.Mapping;
using Esri.ArcGISRuntime.Symbology;
using Esri.ArcGISRuntime.UI;

using System;
using System.Collections.Generic;
using Esri.ArcGISRuntime.UI.Controls;
using System.Windows.Media;
using MapTest.MapMaster;
using ExcelUtils;
using Microsoft.Win32;
using System.Windows.Input;
using System.Windows.Controls;
using MapTest.Pages;

namespace MapTest
{

    public partial class MainWindow : Window
    {
        private MapController _mapController;
        private ExcelUtil _excelUtil;
        private List<MapPoint> _inputPointCollection;

        private GraphicsOverlay _graphicsOverlayAddPoints;
        private GraphicsOverlay _graphicsOverlayPoints;
        private GraphicsOverlay _graphicsOverlayPolygon;

        private MapPoint _selectedPointGraphics;
        private ICollection<TemperaturePointModel> _dataTable;

        private ViewMapPage _viewMapPage = new ViewMapPage();
        private AddPointsPage _addPointsPage = new AddPointsPage();
        private EditPointPage _editPointPage = new EditPointPage();

        private readonly Brush _defaultColor = Brushes.White;
        private readonly Brush _activeColor = Brushes.DarkGray;

        private readonly Thickness _activeThickness = new Thickness(1, 1, 1, 0);
        private readonly Thickness _notActiveThickness = new Thickness(0, 0, 0, 1);

        private readonly CornerRadius _activeRadius = new CornerRadius(5, 5, 0, 0);
        private readonly CornerRadius _notActiveRadius = new CornerRadius(0, 0, 0, 0);

        private const double _scale = 1000000;

        public double MaxTemp { get; set; }
        public double MinTemp { get; set; }

        public MainWindow()
        {
            _mapController = new MapController();
            _inputPointCollection = new List<MapPoint>();
            _graphicsOverlayAddPoints = new GraphicsOverlay() { Id = "addPoints"};
            _graphicsOverlayPoints = new GraphicsOverlay() { Id = "dataPoints" };
            _graphicsOverlayPolygon = new GraphicsOverlay() { Id = "dataPolygon" };
            

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

            MyMapView.Map = new Map(BasemapStyle.ArcGISTerrain);

            MyMapView.GraphicsOverlays.Add(_graphicsOverlayPoints);
            MyMapView.GraphicsOverlays.Add(_graphicsOverlayPolygon);

            _dataTable = _excelUtil.GetPointsData();

            MaxTemp = _excelUtil.GetMaxTemp();
            MinTemp = _excelUtil.GetMinTemp();

            _mapController.MapChangedEvent += _mapController_MapChanged;
            _viewMapPage.MapChangedEvent += _viewMapPage_MapChangedEvent;
            _addPointsPage.ClearAddPointsEvent += _addPointsPage_ClearAddPointsEvent;
            _addPointsPage.AddNewPointsEvent += _addPointsPage_AddNewPointsEvent;

            _mapController.UpdateGraphics(_dataTable, MaxTemp, MinTemp);

            ChangePage(ViewBut);

            ControlPage.Navigate(_viewMapPage);
        }

        private void _addPointsPage_AddNewPointsEvent(IEnumerable<TemperaturePointModel> points)
        {
            foreach (var point in points)
                _dataTable.Add(point);

            _excelUtil.SetPointsData(_dataTable);
            MaxTemp = _excelUtil.GetMaxTemp();
            MinTemp = _excelUtil.GetMinTemp();

            _mapController.UpdateGraphics(_dataTable, MaxTemp, MinTemp);
            _graphicsOverlayAddPoints.Graphics.Clear();
        }

        private void _addPointsPage_ClearAddPointsEvent()
        {
            _graphicsOverlayAddPoints.Graphics.Clear();
        }

        private void _mapController_MapChanged(GraphicsOverlay polygons, GraphicsOverlay points)
        {
            MyMapView.GraphicsOverlays.Remove(_graphicsOverlayPoints);
            MyMapView.GraphicsOverlays.Remove(_graphicsOverlayPolygon);

            _graphicsOverlayPoints = points;
            _graphicsOverlayPolygon = polygons;
                        
            MyMapView.GraphicsOverlays.Add(_graphicsOverlayPolygon);
            MyMapView.GraphicsOverlays.Add(_graphicsOverlayPoints);

            _viewMapPage.UpdateDataGrid(_dataTable);
        }

        private void _viewMapPage_MapChangedEvent(double x, double y)
        {
            MyMapView.SetViewpointAsync(new Viewpoint(y,x, _scale));
        }

        private void MyMapView_GeoViewTapped_Add(object sender, GeoViewInputEventArgs e)
        {
            try
            {
                var centralizedPoint = (MapPoint)GeometryEngine.Project( GeometryEngine.NormalizeCentralMeridian(e.Location), new SpatialReference(4326));
                _inputPointCollection.Add(centralizedPoint);
                
                SimpleMarkerSymbol userTappedSimpleMarkerSymbol = new SimpleMarkerSymbol(SimpleMarkerSymbolStyle.Circle, System.Drawing.Color.Red, 10);

                Graphic userTappedGraphic = new Graphic(e.Location, new Dictionary<string, object>
                {
                    { "Type", "Point" }
                }, userTappedSimpleMarkerSymbol)
                { ZIndex = 0 };
                
                userTappedGraphic.ZIndex = 1;

                MyMapView.GraphicsOverlays["addPoints"].Graphics.Add(userTappedGraphic);
                _addPointsPage.AddPoint(centralizedPoint.X,centralizedPoint.Y);
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

            foreach(var point in _graphicsOverlayPoints.Graphics)
            {
                if (point.Geometry.GeometryType != GeometryType.Point) continue;

                var convertGeometry = GeometryEngine.Project(point.Geometry, e.Location.SpatialReference);

                distance = GeometryEngine.Distance(e.Location, convertGeometry);
                if (distance < minDistance)
                {
                    minDistance = distance;
                    _selectedPointGraphics = (MapPoint)point.Geometry;
                }
            }

            MyMapView.SetViewpointAsync(new Viewpoint(_selectedPointGraphics,_scale));
        }

        private void ViewBut_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            MyMapView.GeoViewTapped -= MyMapView_GeoViewTapped_Edit;
            MyMapView.GeoViewTapped -= MyMapView_GeoViewTapped_Add;

            if (MyMapView.GraphicsOverlays.Contains(_graphicsOverlayAddPoints))
                MyMapView.GraphicsOverlays.Remove(_graphicsOverlayAddPoints);

            ChangePage(sender);

            ControlPage.Navigate(_viewMapPage);
        }

        private void AddBut_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            MyMapView.GeoViewTapped -= MyMapView_GeoViewTapped_Edit;
            MyMapView.GeoViewTapped += MyMapView_GeoViewTapped_Add;

            if (!MyMapView.GraphicsOverlays.Contains(_graphicsOverlayAddPoints))
                MyMapView.GraphicsOverlays.Add(_graphicsOverlayAddPoints);

            ChangePage(sender);

            ControlPage.Navigate(_addPointsPage);
        }

        private void EditBut_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            MyMapView.GeoViewTapped += MyMapView_GeoViewTapped_Edit;
            MyMapView.GeoViewTapped -= MyMapView_GeoViewTapped_Add;

            if (MyMapView.GraphicsOverlays.Contains(_graphicsOverlayAddPoints))
                MyMapView.GraphicsOverlays.Remove(_graphicsOverlayAddPoints);

            ChangePage(sender);

            ControlPage.Navigate(_editPointPage);
        }

        private void ChangePage(object sender)
        {
            ViewBut.Background =
                AddBut.Background =
                EditBut.Background = _defaultColor;

            ((Border)sender).Background = _activeColor;

            ViewBut.BorderThickness =
                AddBut.BorderThickness =
                EditBut.BorderThickness = _notActiveThickness;

            ((Border)sender).BorderThickness = _activeThickness;

            ViewBut.CornerRadius =
                AddBut.CornerRadius =
                EditBut.CornerRadius = _notActiveRadius;

            ((Border)sender).CornerRadius = _activeRadius;
        }
    }
}



