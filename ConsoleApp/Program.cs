using ExchangeRatesLibrary;
using System.Xml.Linq;

namespace ConsoleApp
{
    internal class Program
    {
        static async Task Main(string[] args)
        {
            string result = await ProcessData.GetCurrencyInfoString(DateTime.Parse("2022-05-15"), new DateTime(2022, 05, 25), "GBP", true);
            Console.WriteLine("\n" + result);
        }
    }
}