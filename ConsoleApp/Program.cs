using ExchangeRatesLibrary;
using System.Xml.Linq;

namespace ConsoleApp
{
    internal class Program
    {
        static async Task Main(string[] args)
        {
            string data = await FileDownloader.GetData(DateTime.Parse("2023-05-15"), new DateTime(2023, 05, 19), "GBP", true);
            await Console.Out.WriteLineAsync(data);
        }
    }
}