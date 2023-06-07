﻿using System;
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

            //Time period corectness check
            if (startDate < new DateTime(2002, 1, 1) || endDate > DateTime.Now || startDate > endDate)
                throw new ArgumentOutOfRangeException("Date out of source range!");

            //Determine how many dir files should be checked
            List<string> yearsToDownload = new();
            if (startDate.Year == endDate.Year && startDate.Year == DateTime.Now.Year) //Single year (current)
            {
                yearsToDownload.Add("");
            }
            else if (startDate.Year == endDate.Year && startDate.Year != DateTime.Now.Year) //Single year in past
            {
                yearsToDownload.Add(startDate.Year.ToString());
            }                
            else if (startDate.Year != endDate.Year && endDate.Year == DateTime.Now.Year) // Multiple years till now
            {
                for (int i = 0; i < endDate.Year - startDate.Year; i++)
                    yearsToDownload.Add((startDate.Year + i).ToString());

                yearsToDownload.Add(""); //Add empty string for current year
            }
            else if (startDate.Year != endDate.Year && endDate.Year != DateTime.Now.Year) // Multiple years in past
            {
                for (int i = 0; i <= endDate.Year - startDate.Year; i++)
                    yearsToDownload.Add((startDate.Year + i).ToString());
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
                DateTime fileDate = DateTime.Parse($"20{fileName[5]}{fileName[6]}-{fileName[7]}{fileName[8]}-{fileName[9]}{fileName[10]}"); //c001z180102
                if (fileName[0] == 'c')
                    if (fileDate > startDate && fileDate < endDate)
                        fileNames.Add(fileName);
            }

            return fileNames;
        }
    }
}