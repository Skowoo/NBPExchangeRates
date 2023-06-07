using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExchangeRatesLibrary
{
    public readonly struct CurrencyInfo
    {
        //General properties
        public readonly string currencyTag;
        public readonly DateTime periodStartDate;
        public readonly DateTime periodEndDate;

        //Buy price properties
        public readonly double mediumBuyPrice;
        public readonly double maxBuyPrice;
        public readonly double minBuyPrice;
        public readonly double stdDevBuyPrice;
        public readonly List<DateTime> biggestBuyExchangeDifferenceDates;
        public readonly double biggestBuyExchangeDifference;

        //Sell price properties
        public readonly double mediumSellPrice;
        public readonly double maxSellPrice;
        public readonly double minSellPrice;
        public readonly double stdDevSellPrice;
        public readonly List<DateTime> biggestSellExchangeDifferenceDates;
        public readonly double biggestSellExchangeDifference;

        public CurrencyInfo(Dictionary<DateTime, Tuple<double, double>> inputData, string currencyName)
        {
            if (inputData is null) throw new ArgumentNullException(nameof(inputData));
            if (inputData.Count < 1) throw new ArgumentOutOfRangeException(nameof(inputData));

            currencyTag = currencyName.ToUpper();
            periodStartDate = inputData.Keys.First();
            periodEndDate = inputData.Keys.Last();

            #region data calculation

            mediumBuyPrice = inputData.Values.Average(x => x.Item1);
            maxBuyPrice = inputData.Values.Max(x => x.Item1);
            minBuyPrice = inputData.Values.Min(x => x.Item1);
            stdDevBuyPrice = StdDev(inputData.Values.Select(x => x.Item1));
            biggestBuyExchangeDifferenceDates = new();
            biggestBuyExchangeDifference = 0;
            double tempDifferenceAbsoluteValue = 0;
            for (int i = 1; i < inputData.Keys.Count(); i++)
            {
                double tempResult = Math.Round(inputData.ElementAt(i).Value.Item1 - inputData.ElementAt(i - 1).Value.Item1, 4);
                if (Math.Abs(tempResult) > tempDifferenceAbsoluteValue)
                {
                    biggestBuyExchangeDifferenceDates.Clear();
                    biggestBuyExchangeDifference = tempResult;
                    tempDifferenceAbsoluteValue = Math.Abs(tempResult);
                    biggestBuyExchangeDifferenceDates.Add(inputData.ElementAt(i).Key);
                }
                else if (Math.Abs(tempResult) == tempDifferenceAbsoluteValue)
                    biggestBuyExchangeDifferenceDates.Add(inputData.ElementAt(i).Key);
            }

            mediumSellPrice = inputData.Values.Average(x => x.Item2);
            maxSellPrice = inputData.Values.Max(x => x.Item2);
            minSellPrice = inputData.Values.Min(x => x.Item2);
            stdDevSellPrice = StdDev(inputData.Values.Select(x => x.Item2));
            biggestSellExchangeDifferenceDates = new();
            biggestSellExchangeDifference = 0;
            tempDifferenceAbsoluteValue = 0;
            for (int i = 1; i < inputData.Keys.Count(); i++)
            {
                double tempResult = Math.Round(inputData.ElementAt(i).Value.Item2 - inputData.ElementAt(i - 1).Value.Item2, 4);
                if (Math.Abs(tempResult) > tempDifferenceAbsoluteValue)
                {
                    biggestSellExchangeDifferenceDates.Clear();
                    biggestSellExchangeDifference = tempResult;
                    tempDifferenceAbsoluteValue = Math.Abs(tempResult);
                    biggestSellExchangeDifferenceDates.Add(inputData.ElementAt(i).Key);
                }
                else if (Math.Abs(tempResult) == tempDifferenceAbsoluteValue)
                    biggestSellExchangeDifferenceDates.Add(inputData.ElementAt(i).Key);
            }
            #endregion

        }

        static double StdDev(IEnumerable<double> input)
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
