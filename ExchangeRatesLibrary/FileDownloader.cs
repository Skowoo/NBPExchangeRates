using System;
using System.Net;
using System.Reflection.Metadata.Ecma335;
using System.Xml.Linq;

namespace ExchangeRatesLibrary
{
    public class FileDownloader
    {
        public static async Task<XDocument> GetFile(string requestedName)
        {
            string text;
            using (HttpClient client = new())
            {
                using (HttpResponseMessage response = await client.GetAsync($"https://www.nbp.pl/kursy/xml/{requestedName}.xml"))
                    using (Stream streamToReadFrom = await response.Content.ReadAsStreamAsync())
                        using (StreamReader reader = new StreamReader(streamToReadFrom))
                            text = reader.ReadToEnd();
            }
            return XDocument.Parse(text);
        }
    }
}