using System.Collections.Generic;

namespace Abno.Models
{
    public class LineChartDataPoint
    {
        public string Label { get; set; }
        public int Y { get; set; }
        public string Name { get; set; }
        public List<LineChartDataPoint> DataPoints { get; set; }
    }
}
