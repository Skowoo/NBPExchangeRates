using ExchangeRatesLibrary;
using System.Xml.Linq;

namespace ConsoleApp
{
    internal class Program
    {
        static async Task Main()
        {            
        AskForCurrency:
            Console.WriteLine("Podaj kod waluty (w formacie ISO 4217)");
            string currencyInput = Console.ReadLine().ToUpper().Trim();
            string[] validCurrencies = { "GBP", "USD", "CHF", "EUR" };
            foreach (string currency in validCurrencies)
                if (currencyInput == currency)
                    goto AskForTimeline;
            goto AskForCurrency;


        AskForTimeline:
            Console.WriteLine("Podaj okres czasu dla którego wykonać obliczenia: (przykładowy format: 2022-01-01,2022-02-01");
            string timelineInput = Console.ReadLine();
            string[] timePoints = timelineInput.Split(',', StringSplitOptions.RemoveEmptyEntries);
            if (timePoints.Length != 2 ) 
                goto AskForTimeline;

            bool startParsed = DateTime.TryParse(timePoints[0], out DateTime startDate);
            bool endParsed = DateTime.TryParse(timePoints[1], out DateTime endDate);

            if (!startParsed || !endParsed || startDate > endDate)
                goto AskForTimeline;

            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("\nDane wejściowe zostały prawidłowo odczytane. Zaczekaj na wynik.\n");
            Console.ForegroundColor = ConsoleColor.White;

            try
            {
                CurrencyInfo requestedCurrencyData = await DataObtainer.GetData(startDate, endDate, currencyInput);
                string result = ProcessData.GetCurrencyInfoString(requestedCurrencyData);
                Console.WriteLine(result);
            }           
            catch (Exception ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("Wystąpił błąd!");
                Console.WriteLine(ex.Message);
            }

            Console.ResetColor();
        }
    }
}