using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Watering2.Models
{
    public partial class Error
    {
        [Key]
        public int Id { get; set; }
        public DateTime TimeStamp { get; set; }
        public string Message { get; set; }
    }
}
