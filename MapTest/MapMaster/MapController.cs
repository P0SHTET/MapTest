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
        public delegate void MapChangedEvent(GraphicsOverlay map);
        public event MapChangedEvent MapChanged;

        private GraphicsOverlay _graphicsOverlay;
        private readonly SpatialReference _spatialReference = new SpatialReference(4326);

        private double _minTemp = -100;
        private double _maxTemp = 100;

        public MapController()
        {
            _graphicsOverlay = new GraphicsOverlay();
        }

        public void UpdateGraphics(IEnumerable<TemperaturePointModel> pointsList, double maxTemp, double minTemp)
        {
            _graphicsOverlay.ClearSelection();

            var delanurator = new Delaunator(pointsList.ToArray());
            var triangles = delanurator.GetTriangles();

            _maxTemp = maxTemp;
            _minTemp = minTemp;

            var list = pointsList.Select(x => new MapPoint(x.Longitude, x.Latitude, _spatialReference));

            foreach(var triangle in triangles)
            {
                var triangleTempPoints = triangle.Points as IEnumerable<TemperaturePointModel>;
                var avgTriangleTemp = triangleTempPoints.Average(x => x.Temperature);
                var trianglePoints = triangle.Points.Select(x => new MapPoint(x.Y, x.X, _spatialReference));
                var trianglePolygon = new Polygon(trianglePoints, _spatialReference);
                var triangleGraphics = new Graphic(trianglePolygon, 
                                                    new SimpleFillSymbol(SimpleFillSymbolStyle.Solid, 
                                                    GetTempColor(avgTriangleTemp), null));
                _graphicsOverlay.Graphics.Add(triangleGraphics);
            }



            var bufferPolygons = GeometryEngine.BufferGeodetic(new Multipoint(list,
                                                                              _spatialReference), 
                                                               10, 
                                                               LinearUnits.Kilometers);
            

            foreach(var point in pointsList)
            {
                _graphicsOverlay.Graphics.Add(
                    new Graphic(GeometryEngine.BufferGeodetic(
                            new MapPoint(point.Longitude, 
                                         point.Latitude, 
                                         _spatialReference),
                            15,
                            LinearUnits.Kilometers), 
                        new SimpleFillSymbol(SimpleFillSymbolStyle.Solid, GetTempColor(point.Temperature),
                        new SimpleLineSymbol())));
            }

            foreach (var point in pointsList)
            {
                _graphicsOverlay.Graphics.Add(
                    new Graphic(
                        new MapPoint(point.Longitude, point.Latitude, _spatialReference),
                        new TextSymbol(point.Name,GetTempColor(point.Temperature),8,HorizontalAlignment.Center,VerticalAlignment.Bottom)
                        )
                    );
            }

            MapChanged?.Invoke(_graphicsOverlay);
        }

        private Color GetTempColor(double temp)
        {
            byte r = (byte)((temp - _minTemp) / (_maxTemp - _minTemp) * 255.0);
            byte b = (byte)((_maxTemp - temp) / (_maxTemp - _minTemp) * 255.0);

            return Color.FromArgb(155, r, 0, b);
        }

    }
}
