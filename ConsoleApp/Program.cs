using ExchangeRatesLibrary;
using System.Xml.Linq;

namespace ConsoleApp
{
    internal class Program
    {
        static async Task Main(string[] args)
        {
            List<XDocument> data = await FileDownloader.GetFilesFromPeriod(DateTime.Parse("2018-09-11"), new DateTime(2018, 11, 19));
        }
    }
}