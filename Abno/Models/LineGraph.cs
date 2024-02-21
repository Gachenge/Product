using System;
using System.Collections.Generic;

namespace Abno.Models
{
    public class DataSeries
    {
        public string Name { get; set; }
        public List<DataPoint> DataPoints { get; set; }
    }

    public class DataPoint
    {
        public DataPoint(string label, double y)
        {
            
        }
    }
}
