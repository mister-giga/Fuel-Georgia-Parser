using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fuel_Georgia_Parser.Models
{
    class Company
    {
        public string Key { get; set; }
        public string Name { get; set; }
        public string Color { get; set; }
        public Fuel[] Fuels { get; set; }
    }
}
