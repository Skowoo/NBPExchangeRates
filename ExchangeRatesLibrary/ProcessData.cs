using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExchangeRatesLibrary
{
    public class ProcessData
    {
        public static async Task<string> GetCurrencyInfoString(DateTime startDate, DateTime endDate, string currencyTag, bool displayLog = false)
        {
            Dictionary<DateTime, Tuple<double, double>> data = await DataObtainer.GetData(startDate, endDate, currencyTag);
            currencyTag = currencyTag.ToUpper();

            //Calculate all variables
            double mediumBuyPrice = data.Values.Average(x => x.Item1);
            double maxBuyPrice = data.Values.Max(x => x.Item1);
            double minBuyPrice = data.Values.Min(x => x.Item1);
            double stdDevBuyPrice = StdDev(data.Values.Select(x => x.Item1));
            List<DateTime> biggestBuyExchangeDifferenceDates = new();
            double biggestBuyExchangeDifference = 0;
            double tempDifferenceAbsoluteValue = 0;
            for (int i = 1; i < data.Keys.Count(); i++)
            {
                double tempResult = Math.Round(data.ElementAt(i).Value.Item1 - data.ElementAt(i - 1).Value.Item1, 4);
                if (Math.Abs(tempResult) > tempDifferenceAbsoluteValue)
                {
                    biggestBuyExchangeDifferenceDates.Clear();
                    biggestBuyExchangeDifference = tempResult;
                    tempDifferenceAbsoluteValue = Math.Abs(tempResult);
                    biggestBuyExchangeDifferenceDates.Add(data.ElementAt(i).Key);
                }
                else if (Math.Abs(tempResult) == tempDifferenceAbsoluteValue)
                    biggestBuyExchangeDifferenceDates.Add(data.ElementAt(i).Key);
            }            

            double mediumSellPrice = data.Values.Average(x => x.Item2);
            double maxSellPrice = data.Values.Max(x => x.Item2);
            double minSellPrice = data.Values.Min(x => x.Item2);
            double stdDevSellPrice = StdDev(data.Values.Select(x => x.Item2));
            List<DateTime> biggestSellExchangeDifferenceDates = new();
            double biggestSellExchangeDifference = 0;
            tempDifferenceAbsoluteValue = 0;
            for (int i = 1; i < data.Keys.Count(); i++)
            {
                double tempResult = Math.Round(data.ElementAt(i).Value.Item2 - data.ElementAt(i - 1).Value.Item2, 4);
                if (Math.Abs(tempResult) > tempDifferenceAbsoluteValue)
                {
                    biggestSellExchangeDifferenceDates.Clear();
                    biggestSellExchangeDifference = tempResult;
                    tempDifferenceAbsoluteValue = Math.Abs(tempResult);
                    biggestSellExchangeDifferenceDates.Add(data.ElementAt(i).Key);
                }
                else if (Math.Abs(tempResult) == tempDifferenceAbsoluteValue)
                    biggestSellExchangeDifferenceDates.Add(data.ElementAt(i).Key);
            }

            //Create output string 
            StringBuilder output = new StringBuilder();
            output.AppendLine($"Dane dla waluty {currencyTag} w dniach od {startDate:dd.MM.yyyy} do {endDate:dd.MM.yyyy}");

            if ( displayLog )
                foreach ( var log in data )
                    output.AppendLine($"Data: {log.Key:dd.MM.yyyy}. Kurs kupna: {log.Value.Item1:0.00} PLN, kurs sprzedaży: {log.Value.Item2:0.00} PLN");

            //Build part for buy prices----------------------------
            output.AppendLine($"Średni kurs kupna w podanym przedziale czasowym: {mediumBuyPrice:0.00} PLN");
            output.AppendLine($"Maksymalny kurs kupna w podanym przedziale czasowym: {maxBuyPrice:0.00} PLN");
            output.AppendLine($"Minimalny kurs kupna w podanym przedziale czasowym: {minBuyPrice:0.00} PLN");
            output.AppendLine($"Odchylenie standardowe kursu kupna w podanym przedziale czasowym: {stdDevBuyPrice:0.00} PLN");
            StringBuilder topBuyDifferenceDatesStringBuilder = new();
            foreach ( var date in biggestBuyExchangeDifferenceDates)
                topBuyDifferenceDatesStringBuilder.Append(date.ToString("dd.MM.yyyy") + " ");
            output.AppendLine($"Największa różnica kursowa w cenie kupna: {biggestBuyExchangeDifference} PLN. Wystąpiła w dniach: {topBuyDifferenceDatesStringBuilder.ToString()}");
            output.AppendLine();

            //Build part for sell prices----------------------------
            output.AppendLine($"Średni kurs sprzedaży w podanym przedziale czasowym: {mediumSellPrice:0.00} PLN");
            output.AppendLine($"Maksymalny kurs sprzedaży w podanym przedziale czasowym: {maxSellPrice:0.00} PLN");
            output.AppendLine($"Minimalny kurs sprzedaży w podanym przedziale czasowym: {minSellPrice:0.00} PLN");
            output.AppendLine($"Odchylenie standardowe kursu sprzedaży w podanym przedziale czasowym: {stdDevSellPrice:0.00} PLN");
            StringBuilder topSellDifferenceDatesStringBuilder = new();
            foreach (var date in biggestSellExchangeDifferenceDates)
                topSellDifferenceDatesStringBuilder.Append(date.ToString("dd.MM.yyyy") + " ");
            output.AppendLine($"Największa różnica kursowa w cenie sprzedaży: {biggestSellExchangeDifference} PLN. Wystąpiła w dniach: {topSellDifferenceDatesStringBuilder.ToString()}");
            output.AppendLine();

            return output.ToString();
        }

        private static double StdDev(IEnumerable<double> input)
        {
            double output = 0;
            int count = input.Count();
            if (count > 1)
            {
                double avg = input.Average();
                double sum = input.Sum(d => (d - avg) * (d - avg));
                output = Math.Sqrt(sum / count);
            }
            return output;
        }
    }
}
