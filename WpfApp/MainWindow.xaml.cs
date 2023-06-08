using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using ExchangeRatesLibrary;
using WpfApp.Resources;

namespace WpfApp
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        DateTime? startDate;
        DateTime? endDate;
        string? currency;
        CurrencyInfo currencyInfo;

        public MainWindow()
        {
            InitializeComponent();

            CurrenciesList.ItemsSource = Enum.GetValues(typeof(AviableCurrencies)).Cast<AviableCurrencies>();

            DataObtainer.FileDoneEvent += FileDoneEventHandler;

            SetInitialState();
        }

        private void FileDoneEventHandler(object sender, DocumentDownloadedEvent e)
        {
            if (LoadingGrid.Visibility == Visibility.Visible)
            {
                ProgressBar_Text.Text = $"Opracowano {e.filesDone} / {e.totalFilesToDownload} plików.";
                ProgressBar.Maximum = e.totalFilesToDownload;
                ProgressBar.Value = e.filesDone;
            }
        }

        private void SetInitialState()
        {
            StartDownloadButton.Visibility = Visibility.Collapsed;

            startDate = null;
            endDate = null;
            currency = null;

            LoadingGrid.Visibility = Visibility.Collapsed;
            ResultGrid.Visibility = Visibility.Collapsed;
            InputGrid.Visibility = Visibility.Visible;

            CurrenciesList.SelectedIndex = 0;
            ProgressBar.Value = 0;
            ProgressBar_Text.Text = "";

            StartFirstBlackout.Start = new DateTime(1, 01, 01);
            StartFirstBlackout.End = new DateTime(2002, 01, 01);
            StartSecondBlackout.Start = DateTime.Now;
            StartSecondBlackout.End = new DateTime(9999, 01, 01);
            EndFirstBlackout.Start = new DateTime(1, 01, 01);
            EndFirstBlackout.End = new DateTime(2002, 01, 01);
            EndSecondBlackout.Start = DateTime.Now;
            EndSecondBlackout.End = new DateTime(9999, 01, 01);
        }

        private void StartDateCalendar_SelectedDatesChanged(object sender, SelectionChangedEventArgs e)
        {
            EndFirstBlackout.End = (DateTime)StartDateCalendar.SelectedDate;
            startDate = StartDateCalendar.SelectedDate;
            ManageDownloadButtonVisibility();
        }

        private void EndDateCalendar_SelectedDatesChanged(object sender, SelectionChangedEventArgs e)
        {
            StartSecondBlackout.Start = (DateTime)EndDateCalendar.SelectedDate;
            endDate = EndDateCalendar.SelectedDate;
            ManageDownloadButtonVisibility();
        }

        private void CurrenciesList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            currency = CurrenciesList.SelectedItem.ToString();
            ManageDownloadButtonVisibility();
        }

        private void StartDownloadButton_Click(object sender, RoutedEventArgs e)
        {
            //MessageBox.Show($"Diagnostic Data:\nstartDate:  {startDate}\nendDate:  {endDate}\ncurrency:  {currency}");
            ObtainData();
        }

        private async void ObtainData()
        {
            InputGrid.Visibility = Visibility.Collapsed;
            LoadingGrid.Visibility = Visibility.Visible;
            try
            {
                currencyInfo = await DataObtainer.GetData((DateTime)startDate, (DateTime)endDate, currency);
            }    
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                SetInitialState();
                return;
            }
            FillData();
            LoadingGrid.Visibility = Visibility.Collapsed;
            ResultGrid.Visibility = Visibility.Visible;
        }

        private void FillData()
        {
            ResultDescriptionBlock.Text = $"Dane za okres od {currencyInfo.periodStartDate:dd.MM.yyyy} do {currencyInfo.periodEndDate:dd.MM.yyyy} dla waluty: {currencyInfo.currencyTag}";

            BuyAverage_TextBox.Text = $"{currencyInfo.averageBuyPrice:0.00} PLN";
            BuyMax_TextBox.Text = $"{currencyInfo.maxBuyPrice:0.00} PLN";
            BuyMin_TextBox.Text = $"{currencyInfo.minBuyPrice:0.00} PLN";
            BuyDev_TextBox.Text = $"{currencyInfo.stdDevBuyPrice:0.00} PLN";
            BuyMaxDifference_TextBox.Text = $"{currencyInfo.biggestBuyExchangeDifference:0.00} PLN";

            SellAverage_TextBox.Text = $"{currencyInfo.averageSellPrice:0.00} PLN";
            SellMax_TextBox.Text = $"{currencyInfo.maxSellPrice:0.00} PLN";
            SellMin_TextBox.Text = $"{currencyInfo.minSellPrice:0.00} PLN";
            SellDev_TextBox.Text = $"{currencyInfo.stdDevSellPrice:0.00} PLN";
            SellMaxDifference_TextBox.Text = $"{currencyInfo.biggestSellExchangeDifference:0.00} PLN";

            BuyPriceDatesTextBlock.Text = "Najwyższa różnica kursowa wsytąpiła w dniach:\n";
            foreach ( DateTime item in currencyInfo.biggestBuyExchangeDifferenceDates )
                BuyPriceDatesTextBlock.Text += $"{item:dd.MM.yyyy} \n";

            SellPriceDatesTextBlock.Text = "Najwyższa różnica kursowa wsytąpiła w dniach:\n";
            foreach (DateTime item in currencyInfo.biggestSellExchangeDifferenceDates)
                SellPriceDatesTextBlock.Text += $"{item:dd.MM.yyyy} \n";
        }

        private void ManageDownloadButtonVisibility()
        {
            if (startDate != null && endDate != null && currency != null)
                StartDownloadButton.Visibility = Visibility.Visible;
            else 
                StartDownloadButton.Visibility = Visibility.Collapsed;
        }

        private void NewQueryButton_Click(object sender, RoutedEventArgs e) => SetInitialState();
    }
}
