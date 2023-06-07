using ExchangeRatesLibrary;
using System.Xml.Linq;

namespace ConsoleApp
{
    internal class Program
    {
        static async Task Main(string[] args)
        {
            string data = await FileDownloader.GetData(DateTime.Parse("2018-11-12"), new DateTime(2018, 11, 19), "GBP");
            await Console.Out.WriteLineAsync(data);
        }
    }
}