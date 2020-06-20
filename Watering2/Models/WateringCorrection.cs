using System;
using System.Collections.Generic;
using System.Text;

namespace Watering2.Models
{
    public class WateringCorrection
    {
        public double CorrFactorHot { get; set; }
        public double CorrFactorCold { get; set; }
        public double WateringDuration { get; set; }
        public int RainDuration { get; set; }
    }
}
