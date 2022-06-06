using Esri.ArcGISRuntime.Geometry;
using Esri.ArcGISRuntime.Mapping;
using Esri.ArcGISRuntime.Symbology;
using Esri.ArcGISRuntime.UI;
using ExcelUtils;
using GeometryUtils;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MapTest.MapMaster
{
    public class MapController
    {
        public delegate void MapChangedEvent(GraphicsOverlay polygons, GraphicsOverlay points);
        public event MapChangedEvent MapChanged;

        private GraphicsOverlay _graphicsOverlayPolygon;
        private GraphicsOverlay _graphicsOverlayPoint;
        private readonly SpatialReference _spatialReference = new SpatialReference(4326);

        private double _minTemp = -100;
        private double _maxTemp = 100;

        public MapController()
        {
            _graphicsOverlayPoint = new GraphicsOverlay() { Id="dataPoint"};
            _graphicsOverlayPolygon = new GraphicsOverlay() { Id="dataPolygon"};
        }

        public void UpdateGraphics(IEnumerable<TemperaturePointModel> pointsList, double maxTemp, double minTemp)
        {
            _graphicsOverlayPoint.ClearSelection();
            _graphicsOverlayPolygon.ClearSelection();

            var delanurator = new Delaunator(pointsList.ToArray());
            var triangles = delanurator.GetTriangles();

            _maxTemp = maxTemp;
            _minTemp = minTemp;

            
            Random random = new Random();

            foreach(var triangle in triangles)
            {
                var triangleTempPoints = triangle.Points.Select(x=>x as TemperaturePointModel);
                var avgTriangleTemp = triangle.Points.Select(x=>x as TemperaturePointModel).Average(x=>x.Temperature);
                var trianglePoints = triangle.Points.Select(x => new MapPoint(x.X, x.Y, _spatialReference));
                var trianglePolygon = new Polygon(trianglePoints, _spatialReference);
                var triangleGraphics = new Graphic(trianglePolygon, 
                                                    new SimpleFillSymbol(SimpleFillSymbolStyle.Solid, 
                                                                         GetTempColor(avgTriangleTemp), 
                                                                         new SimpleLineSymbol(SimpleLineSymbolStyle.Solid,Color.FromArgb(30,0,0,0),0.5)));
                _graphicsOverlayPolygon.Graphics.Add(triangleGraphics);
            }

            foreach (var point in pointsList)
            {
                _graphicsOverlayPoint.Graphics.Add(
                    new Graphic(
                        new MapPoint(point.Longitude, point.Latitude, _spatialReference),
                        new TextSymbol(point.Name??"",Color.Black,8,HorizontalAlignment.Center,VerticalAlignment.Bottom)
                        )
                    );
            }

            MapChanged?.Invoke(_graphicsOverlayPolygon, _graphicsOverlayPoint);
        }

        private Color GetTempColor(double temp)
        {
            byte r = (byte)((temp - _minTemp) / (_maxTemp - _minTemp) * 255.0);
            byte b = (byte)((_maxTemp - temp) / (_maxTemp - _minTemp) * 255.0);

            return Color.FromArgb(200, 255, r, b);
        }

    }
}
