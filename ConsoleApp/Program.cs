using ExchangeRatesLibrary;
using System.Xml.Linq;

namespace ConsoleApp
{
    internal class Program
    {
        static async Task Main(string[] args)
        {
            string result = await ProcessData.GetCurrencyInfoString(DateTime.Parse("2022-12-19"), new DateTime(2023, 01, 6), "JPY");
            Console.WriteLine("\n" + result);
        }
    }
}