using System;
using System.ComponentModel.DataAnnotations;

namespace WebController.Models
{
    public class SensorReading
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string AssetId { get; set; }

        [Required]
        public DateTime Timestamp { get; set; }

        public double MotorAmps { get; set; }

        public double Temperature { get; set; }

        public double Vibration { get; set; }

        public string Status { get; set; }
    }
}