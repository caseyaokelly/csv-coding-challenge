using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace csv_coding_challenge.Models
{
    public class CombinedData
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string TrackingID { get; set; }
        public string Creative { get; set; }
        public int Clicks { get; set; }
        public int Impressions { get; set; }
        public DateTime DateStamp { get; set; }
    }
}
