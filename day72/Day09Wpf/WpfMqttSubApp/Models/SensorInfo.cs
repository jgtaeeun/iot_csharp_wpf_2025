using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WpfMqttSubApp.Models
{
    public class SensorInfo
    {
        [Key]
        public DateTime SENSING_DT { get; set; }
        [Key]
        public string PUB_ID { get; set; }
        public decimal COUNT { get; set; }
        public float TEMP {  get; set; }
        public float HUMID { get; set; }

        public bool RAIN { get; set; }  
        public bool PERSON { get; set; }
        public bool LIGHT { get; set; }

    }
}
