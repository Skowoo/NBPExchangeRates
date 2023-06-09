using ExchangeRatesLibrary;
using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using WpfApp.Resources;

namespace WpfApp
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        #region Properties
        DateTime? startDate;
        DateTime? endDate;
        string? currency;
        CurrencyInfo currencyInfo;
        #endregion

        public MainWindow()
        {
            InitializeComponent();
            CurrenciesList.ItemsSource = Enum.GetValues(typeof(AviableCurrencies)).Cast<AviableCurrencies>();
            DataObtainer.FileDoneEvent += FileDoneEventHandler;
            SetInitialState();
        }

        #region Control events

        private void StartDateCalendar_SelectedDatesChanged(object sender, SelectionChangedEventArgs e)
        {
            if (StartDateCalendar.SelectedDate != null)
            {
                EndFirstBlackout.End = (DateTime)StartDateCalendar.SelectedDate;
                startDate = StartDateCalendar.SelectedDate;
                Mouse.Capture(null);
                ManageDownloadButtonVisibility();
            }
        }

        private void EndDateCalendar_SelectedDatesChanged(object sender, SelectionChangedEventArgs e)
        {
            if (EndDateCalendar.SelectedDate != null)
            {
                StartSecondBlackout.Start = (DateTime)EndDateCalendar.SelectedDate;
                endDate = EndDateCalendar.SelectedDate;
                Mouse.Capture(null);
                ManageDownloadButtonVisibility();
            }
        }

        private void CurrenciesList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            currency = CurrenciesList.SelectedItem.ToString();
            ManageDownloadButtonVisibility();
        }

        private void StartDownloadButton_Click(object sender, RoutedEventArgs e) => ObtainData();

        private void NewQueryButton_Click(object sender, RoutedEventArgs e) => SetInitialState();

        #endregion

        #region Internal methods

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

            StartDateCalendar.DisplayDate = DateTime.Now;
            StartDateCalendar.SelectedDate = null;
            EndDateCalendar.DisplayDate = DateTime.Now;
            EndDateCalendar.SelectedDate = null;
            StartFirstBlackout.Start = new DateTime(1, 01, 01);
            StartFirstBlackout.End = new DateTime(2002, 01, 01);
            StartSecondBlackout.Start = DateTime.Now;
            StartSecondBlackout.End = new DateTime(9999, 01, 01);
            EndFirstBlackout.Start = new DateTime(1, 01, 01);
            EndFirstBlackout.End = new DateTime(2002, 01, 01);
            EndSecondBlackout.Start = DateTime.Now;
            EndSecondBlackout.End = new DateTime(9999, 01, 01);
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
            foreach (DateTime item in currencyInfo.biggestBuyExchangeDifferenceDates)
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

        #endregion
    }
}
