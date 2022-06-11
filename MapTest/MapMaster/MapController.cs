using Esri.ArcGISRuntime.Geometry;
using Esri.ArcGISRuntime.Symbology;
using Esri.ArcGISRuntime.UI;
using ExcelUtils;
using GeometryUtils;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;

namespace MapTest.MapMaster
{
    public class MapController
    {
        public delegate void MapChanged(GraphicsOverlay polygons, GraphicsOverlay points);
        public event MapChanged MapChangedEvent;

        private readonly GraphicsOverlay _graphicsOverlayPolygon;
        private readonly GraphicsOverlay _graphicsOverlayPoint;
        private readonly SpatialReference _spatialReference = new SpatialReference(4326);

        private double _minTemp = -100;
        private double _maxTemp = 100;

        public MapController()
        {
            _graphicsOverlayPoint = new GraphicsOverlay();
            _graphicsOverlayPolygon = new GraphicsOverlay();
        }

        public void UpdateGraphics(IEnumerable<TemperaturePointModel> pointsList, double maxTemp, double minTemp)
        {
            _graphicsOverlayPoint.Graphics.Clear();
            _graphicsOverlayPolygon.Graphics.Clear();

            if (pointsList.Count() > 2)
            {
                var delanurator = new Delaunator(pointsList.ToArray());
                var polygons = delanurator.GetVoronoiCells();

                _maxTemp = maxTemp;
                _minTemp = minTemp;


                Random random = new Random();

                foreach (var polygon in polygons)
                {
                    var triangleTempPoints = polygon.Points.Select(x => x as TemperaturePointModel);
                    var polygonPoints = polygon.Points.Select(x => new MapPoint(x.X, x.Y, _spatialReference));
                    var polygonGeometry = new Polygon(polygonPoints, _spatialReference);
                    double? avgPolygonTemperature = null;
                    foreach (var point in pointsList)

                        if (GeometryEngine.Intersects(polygonGeometry, new MapPoint(point.Longitude, point.Latitude, _spatialReference)))
                            avgPolygonTemperature = point.Temperature;


                    var polygonGraphic = new Graphic(polygonGeometry,
                                                        new SimpleFillSymbol(SimpleFillSymbolStyle.Solid,
                                                                             GetTempColor(avgPolygonTemperature),
                                                                             new SimpleLineSymbol(SimpleLineSymbolStyle.Solid, Color.FromArgb(30, 0, 0, 0), 0.5)));
                    _graphicsOverlayPolygon.Graphics.Add(polygonGraphic);
                }
            }

            foreach (var point in pointsList)
            {
                _graphicsOverlayPoint.Graphics.Add(
                    new Graphic(
                        new MapPoint(point.Longitude, point.Latitude, _spatialReference),
                        new TextSymbol(point.Name ?? "", Color.Black, 8, HorizontalAlignment.Center, VerticalAlignment.Bottom)
                        )
                    );
            }
            

            MapChangedEvent?.Invoke(_graphicsOverlayPolygon, _graphicsOverlayPoint);
        }

        private Color GetTempColor(double? temp)
        {
            if (temp is null) return Color.FromArgb(100, 0, 0, 0);


            double x = (double)((temp - _minTemp) / (_maxTemp - _minTemp));

            byte red, green, blue;
            if (x > 0.5) 
            {
                x = (x - 0.5) * 2.0;
                red = (byte)(x * 255);
                green = (byte)((1 - x) * 255);
                blue = 0;
            }
            else
            {
                x *= 2;
                red = 0;
                green = (byte)(x * 255);
                blue = (byte)((1 - x) * 255);
            }
            return Color.FromArgb(100, red,green,blue);
        }

    }
}
