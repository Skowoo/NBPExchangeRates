using ExchangeRatesLibrary;
using System.Xml.Linq;

namespace ConsoleApp
{
    internal class Program
    {
        static async Task Main(string[] args)
        {
            XDocument requested = await FileDownloader.GetFile("c001z120102");
            //Console.WriteLine(requested.ToString());
            List<string> list = await FileDownloader.GetFileNames(DateTime.Now, DateTime.Now);

            foreach (string name in list)
                Console.WriteLine(name);
        }
    }
}