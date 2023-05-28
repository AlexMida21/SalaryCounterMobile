using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SalaryCounter.Models
{
    public class TaskVisible
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public decimal? Rate { get; set; }
        public string Employers { get; set; }
        public DateTime ResultTime { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public bool DR { get; set; } = true;
        public string? StartTime2 { get; set; }
        public string? EndTime2 { get; set; }
        public string? ResultTime2 { get; set; }
    }
}
