using System.Net;
using System.Reflection.Metadata.Ecma335;
using System.Xml.Linq;

namespace ExchangeRatesLibrary
{
    public class FileDownloader
    {
        private static string directoryPath = "C:\\WINDOWS\\Temp\\ExchangeApp";

        public FileDownloader() 
        {
            Directory.CreateDirectory(directoryPath);
        }

        public XDocument GetFile(string requestedName)
        {
            string text;
            using (var client = new WebClient())
            {
                text = client.DownloadString("https://www.nbp.pl/kursy/xml/c001z120102.xml");
            }
            return XDocument.Parse(text);
        }
    }
}