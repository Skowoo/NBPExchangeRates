using ExchangeRatesLibrary;
using System.Xml.Linq;

namespace ConsoleApp
{
    internal class Program
    {
        static async Task Main(string[] args)
        {
            Dictionary<DateTime, Tuple<double, double>> data = await DataObtainer.GetData(DateTime.Parse("2023-05-15"), new DateTime(2023, 05, 19), "GBP");
        }
    }
}