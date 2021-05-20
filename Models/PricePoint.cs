using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fuel_Georgia_Parser.Models
{
    class PricePoint
    {
        public DateTime Date { get; set; }
        public decimal Price { get; set; }
        public decimal Change { get; set; }
    }
}
