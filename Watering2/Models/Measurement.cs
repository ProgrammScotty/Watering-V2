using System;
using System.Collections.Generic;
using System.Globalization;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Watering2.Models
{
    public partial class Measurement
    {
        [Key]
        public int Id { get; set; }
        public DateTime TimeStamp { get; set; }
        public double Temperature { get; set; }
        public double Humidity { get; set; }
        public double Pressure { get; set; }
        public bool Raining { get; set; }
        public double DewPoint { get; set; }
        public bool RainCorrected { get; set; }
    }
}
