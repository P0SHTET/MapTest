using GeometryUtils.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExcelUtils
{
    public class TemperaturePointModel : IPoint
    {
        public TemperaturePointModel()
        {

        }
        public TemperaturePointModel(string name, double x, double y, double temperature)
        {
            Name = name;
            Longitude = x;
            Latitude = y;
            Temperature = temperature;
        }

        public string? Name { get; set; }
        public double Longitude { get; set; }
        public double Latitude { get; set; }
        public double Temperature { get; set; }
        public double X { get => Longitude; set => Longitude = value; }
        public double Y { get => Latitude; set => Latitude = value; }
    }
}
