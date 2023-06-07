using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExchangeRatesLibrary
{
    public class ProcessData
    {
        public static string GetCurrencyInfoString(CurrencyInfo data)
        {
            //Create output string 
            StringBuilder output = new StringBuilder();
            output.AppendLine($"Dane dla waluty {data.currencyTag} w dniach od {data.periodStartDate:dd.MM.yyyy} do {data.periodEndDate:dd.MM.yyyy}");

            //Build buy prices
            output.AppendLine($"Średni kurs kupna w podanym przedziale czasowym: {data.mediumBuyPrice:0.00} PLN");
            output.AppendLine($"Maksymalny kurs kupna w podanym przedziale czasowym: {data.maxBuyPrice:0.00} PLN");
            output.AppendLine($"Minimalny kurs kupna w podanym przedziale czasowym: {data.minBuyPrice:0.00} PLN");
            output.AppendLine($"Odchylenie standardowe kursu kupna w podanym przedziale czasowym: {data.stdDevBuyPrice:0.00} PLN");
            StringBuilder topBuyDifferenceDatesStringBuilder = new();
            foreach ( var date in data.biggestBuyExchangeDifferenceDates)
                topBuyDifferenceDatesStringBuilder.Append(date.ToString("dd.MM.yyyy") + " ");
            output.AppendLine($"Największa różnica kursowa w cenie kupna: {data.biggestBuyExchangeDifference} PLN. Wystąpiła w dniach: {topBuyDifferenceDatesStringBuilder.ToString()}");
            output.AppendLine();

            //Build sell prices
            output.AppendLine($"Średni kurs sprzedaży w podanym przedziale czasowym: {data.mediumSellPrice:0.00} PLN");
            output.AppendLine($"Maksymalny kurs sprzedaży w podanym przedziale czasowym: {data.maxSellPrice:0.00} PLN");
            output.AppendLine($"Minimalny kurs sprzedaży w podanym przedziale czasowym: {data.minSellPrice:0.00} PLN");
            output.AppendLine($"Odchylenie standardowe kursu sprzedaży w podanym przedziale czasowym: {data.stdDevSellPrice:0.00} PLN");
            StringBuilder topSellDifferenceDatesStringBuilder = new();
            foreach (var date in data.biggestSellExchangeDifferenceDates)
                topSellDifferenceDatesStringBuilder.Append(date.ToString("dd.MM.yyyy") + " ");
            output.AppendLine($"Największa różnica kursowa w cenie sprzedaży: {data.biggestSellExchangeDifference} PLN. Wystąpiła w dniach: {topSellDifferenceDatesStringBuilder.ToString()}");
            output.AppendLine();

            return output.ToString();
        }
    }
}
