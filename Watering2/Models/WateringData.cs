using System;
using System.Collections.Generic;
using System.Globalization;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Watering2.Models
{
    public class WateringData
    {
        [Key]
        public long Id { get; set; }
        public DateTime TimeStamp { get; set; }
        public double Duration { get; set; }
        public double CorrHot { get; set; }
        public double CorrCold { get; set; }
        public bool EmergencyWatering { get; set; }
        public bool NoWateringBecauseRain { get; set; }
        public double PercentageHot { get; set; }
        public double PercentageCold { get; set; }
        public int SamplesCount { get; set; }
        public int SamplesHot {get; set; }
        public int SamplesCold { get; set; }
        public double DurationRain { get; set; }
    }
}
