using ExchangeRatesLibrary;
using System.Xml.Linq;

namespace ConsoleApp
{
    internal class Program
    {
        static void Main(string[] args)
        {
            FileDownloader fdwn = new FileDownloader();
            XDocument requested = fdwn.GetFile("f");
            Console.WriteLine(requested.ToString());
        }
    }
}