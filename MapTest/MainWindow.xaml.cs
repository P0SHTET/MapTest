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

        public double MaxTemp { get; set; }
        public double MinTemp { get; set; }

        public MainWindow()
        {
            _mapController = new MapController();

            OpenFileDialog dialog = new OpenFileDialog()
            {
                Multiselect = false,
                Filter = "Excel Files|*.xls;*.xlsx;*.xlsm"
            };
            
            if ((bool)dialog.ShowDialog())
            {
                _excelUtil = new ExcelUtil(dialog.FileName);
            }
            else
            {
                _excelUtil = new ExcelUtil();
            }

            InitializeComponent();
            MyMapView.Map = new Map(BasemapStyle.ArcGISTerrain);

            var data = _excelUtil.GetPointsData();

            MaxTemp = _excelUtil.GetMaxTemp();
            MinTemp = _excelUtil.GetMinTemp();

            _mapController.MapChanged += _mapController_MapChanged;

            _mapController.UpdateGraphics(data, MaxTemp, MinTemp);
        }

        private void _mapController_MapChanged(GraphicsOverlay graphicsOverlay)
        {
            MyMapView.GraphicsOverlays.Clear();
            MyMapView.GraphicsOverlays.Add(graphicsOverlay);

        }        
    }
}



