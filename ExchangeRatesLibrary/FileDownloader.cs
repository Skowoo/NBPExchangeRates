using System;
using System.Net;
using System.Reflection.Metadata.Ecma335;
using System.Xml.Linq;

namespace ExchangeRatesLibrary
{
    public static class FileDownloader
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

        public static async Task<List<string>> GetFileNames(DateTime startDate, DateTime endDate)
        {
            List<string> fileNames = new ();

            if (startDate < new DateTime(2000, 1, 1) || endDate > DateTime.Now)
                throw new ArgumentOutOfRangeException("Date out of source range!");

            int currentYear = Int32.Parse(DateTime.Now.Year.ToString().Remove(0, 2));
            int startYear = Int32.Parse(startDate.Year.ToString().Remove(0, 2));
            int startMonth = startDate.Month;
            int startDay = startDate.Day;
            int endYear = Int32.Parse(endDate.Year.ToString().Remove(0, 2));
            int endMonth = endDate.Month;
            int endDay = endDate.Day;

            string dirFileName = "";

            string text;
            using (HttpClient client = new())
            {
                using (HttpResponseMessage response = await client.GetAsync($"https://static.nbp.pl/dane/kursy/xml/dir{dirFileName}.txt"))
                using (Stream streamToReadFrom = await response.Content.ReadAsStreamAsync())
                using (StreamReader reader = new StreamReader(streamToReadFrom))
                    text = reader.ReadToEnd();
            }

            string[] separatedFileNames = text.Split(Environment.NewLine);

            foreach (string fileName in separatedFileNames)
                if (fileName[0] == 'c')
                    fileNames.Add(fileName);

            return fileNames;
        }
    }
}