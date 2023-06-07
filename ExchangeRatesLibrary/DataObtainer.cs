using System;
using System.Net;
using System.Reflection.Metadata.Ecma335;
using System.Text;
using System.Xml.Linq;

namespace ExchangeRatesLibrary
{
    /// <summary> Class holds methods responsible for obtaining necessary data from NBP web service </summary>
    public static class DataObtainer
    {
        private const int daysToDownloadOffset = 5;

        /// <summary>
        /// Method returns data of specfied currency from specified period
        /// </summary>
        /// <param name="startDate"> DateTime - beggining of time period </param>
        /// <param name="endDate"> DateTime - end of time period </param>
        /// <param name="currency"> string - ISO 4217 code of currency </param>
        /// <returns> CurencyInfo struct with calculated data from currency price in given time period </returns>
        public static async Task<CurrencyInfo> GetData(DateTime startDate, DateTime endDate, string currency)
        {
            var documentList = await GetFilesFromPeriod(startDate, endDate);

            Dictionary<DateTime, Tuple<double, double>> buySellValues = new();

            foreach (var document in documentList)
            {
                DateTime listingDate = DateTime.Parse(document.Descendants("data_notowania").Single().Value);

                if (listingDate.Date < startDate.Date || listingDate.Date > endDate.Date) continue;

                double buyValue = Double.Parse(
                    document.Descendants("pozycja")
                    .Single(poz => poz.Element("kod_waluty").Value == currency.ToUpper())
                    .Element("kurs_kupna").Value);

                double sellValue = Double.Parse(
                    document.Descendants("pozycja")
                    .Single(poz => poz.Element("kod_waluty").Value == currency.ToUpper())
                    .Element("kurs_sprzedazy").Value);

                buySellValues.Add(listingDate, Tuple.Create(buyValue, sellValue));
            }
            return new (buySellValues, currency);
        }

        private static async Task<List<XDocument>> GetFilesFromPeriod(DateTime startDate, DateTime endDate)
        {
            List<XDocument> outputList = new();

            List<string> documentNames = await GetFileNames(startDate, endDate);
            foreach (string documentName in documentNames)
                outputList.Add(await GetFile(documentName));

            return outputList;
        }

        private static async Task<XDocument> GetFile(string requestedName)
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

        private static async Task<List<string>> GetFileNames(DateTime startDate, DateTime endDate)
        {
            List<string> fileNames = new ();

            DateTime firstFileDate = new DateTime(2002, 1, 1).Date;
            DateTime currentDate = DateTime.Now;

            //Time period corectness check
            if (startDate < firstFileDate || endDate > currentDate || startDate > endDate)
                throw new ArgumentOutOfRangeException("Date out of source range!");

            //Define dates for file names to be listed
            DateTime startDownloadDate = startDate.AddDays(-daysToDownloadOffset);
            if (startDownloadDate < firstFileDate) startDownloadDate = startDate;
            DateTime endDownloadDate = endDate.AddDays(daysToDownloadOffset);
            if (endDownloadDate > currentDate) endDownloadDate = currentDate;

            //Determine how many dir files should be checked
            List<string> yearsToDownload = new();
            if (startDownloadDate.Year == endDownloadDate.Year && startDownloadDate.Year == DateTime.Now.Year) //Single year (current)
            {
                yearsToDownload.Add("");
            }
            else if (startDownloadDate.Year == endDownloadDate.Year && startDownloadDate.Year != DateTime.Now.Year) //Single year in past
            {
                yearsToDownload.Add(startDownloadDate.Year.ToString());
            }                
            else if (startDownloadDate.Year != endDownloadDate.Year && endDownloadDate.Year == DateTime.Now.Year) // Multiple years till now
            {
                for (int i = 0; i < endDownloadDate.Year - startDownloadDate.Year; i++)
                    yearsToDownload.Add((startDownloadDate.Year + i).ToString());

                yearsToDownload.Add(""); //Add empty string for current year
            }
            else if (startDownloadDate.Year != endDownloadDate.Year && endDownloadDate.Year != DateTime.Now.Year) // Multiple years in past
            {
                for (int i = 0; i <= endDownloadDate.Year - startDownloadDate.Year; i++)
                    yearsToDownload.Add((startDownloadDate.Year + i).ToString());
            }

            //Create string of demanded files names in new lines
            string text = "";
            foreach (string year in yearsToDownload)
            {
                using (HttpClient client = new())
                {
                    using (HttpResponseMessage response = await client.GetAsync($"https://static.nbp.pl/dane/kursy/xml/dir{year}.txt"))
                    using (Stream streamToReadFrom = await response.Content.ReadAsStreamAsync())
                    using (StreamReader reader = new StreamReader(streamToReadFrom))
                        text += reader.ReadToEnd();
                }
            }

            //Split source string by new lines
            string[] separatedFileNames = text.Split(Environment.NewLine, StringSplitOptions.RemoveEmptyEntries);

            //Add to output lists ONLY file names which starts from C and are in requested time period
            foreach (string fileName in separatedFileNames)
            {
                DateTime fileDate = DateTime.Parse($"20{fileName.Substring(5 , 2 )}-{fileName.Substring(7, 2)}-{fileName.Substring(9, 2)}");
                if (fileName[0] == 'c')
                    if (startDownloadDate.Date <= fileDate.Date && fileDate.Date <= endDownloadDate.Date)
                        fileNames.Add(fileName.Trim());                        
            }

            return fileNames;
        }
    }
}