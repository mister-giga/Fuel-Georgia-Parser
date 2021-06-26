namespace Fuel_Georgia_Parser.Models //r
{
    internal class Company
    {
        public string Key { get; set; }
        public string Name { get; set; }
        public string Color { get; set; }
        public Fuel[] Fuels { get; set; }
    }
}