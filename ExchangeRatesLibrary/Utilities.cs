using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExchangeRatesLibrary
{
    /// <summary> Static class with tools used in this library </summary>
    internal static class Utilities
    {
        /// <summary>
        /// Method to calculate standard deviation
        /// </summary>
        /// <param name="input"> <i>IEnumerable double</i> collection</param>
        /// <returns><i>double</i> - standard deviation from input</returns>
        internal static double StandardDeviation(IEnumerable<double> input)
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
