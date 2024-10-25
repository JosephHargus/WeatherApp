using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Net.Http;
using System.Runtime.CompilerServices;
using System.Security.RightsManagement;
using Microsoft.VisualBasic;
using static System.Net.Mime.MediaTypeNames;
using System.Diagnostics;
using System.IO;
using Microsoft.Win32;
using Newtonsoft.Json;
using System.Windows.Media.Animation;

namespace WeatherApp
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    /// 

    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            DataContext = this;
            InitializeWeatherData();
        }

        public Boolean UpdateWeatherDataDisplay(WeatherData data)
        {
            if (data != null && data.Location != null && data.Current != null && data.Forecast != null && data.Alerts != null)
            {
                UpdateLocation(data.Location);
                UpdateCurrentConditions(data.Current);
                UpdateDailyForecast(data.Forecast);
                UpdateHourlyForecast(data.Forecast);
                UpdateAlerts(data.Alerts);
                return true;
            }
            else return false;
        }

        private async void InitializeWeatherData()
        {
            WeatherData weatherData = new WeatherData();
            try
            {
                await weatherData.GetWeatherDataAsync();
            }
            catch (Exception ex)
            {
                LocationTextBox.Text = "Error occurred while fetching weather data";
            }

            UpdateWeatherDataDisplay(weatherData);
        }

        private void UpdateDailyForecast(Forecast forecast)
        {
            //make arrays of controls, to make updating the controls simpler
            TextBlock[] dayTexts = {Day1, Day2, Day3, Day4, Day5 };
            System.Windows.Controls.Image[] images = {DailyImage1, DailyImage2, DailyImage3, DailyImage4, DailyImage5};
            TextBlock[] highTexts = {High1, High2, High3, High4, High5};
            TextBlock[] lowTexts = {Low1, Low2, Low3, Low4, Low5};

            for (int currentDay = 0; currentDay < 5; currentDay++)
            {
                Forecastday fday = forecast.ForecastdayArray[currentDay];

                // set day title text block
                dayTexts[currentDay].Text = fday.Date.Substring(5);

                // set high and low text blocks
                highTexts[currentDay].Text = "High: " + fday.Day.MaxtempF.ToString() + "°F";
                lowTexts[currentDay].Text = "Low: " + fday.Day.MintempF.ToString() + "°F";

                // set images
                images[currentDay].Source = LoadBitmap(fday.Day.Condition.Icon.Split(".com")[1]);
                images[currentDay].Width = 64;
                images[currentDay].Height = 64;
            }
        }

        private void UpdateHourlyForecast(Forecast forecast)
        {
            //arrays of controls, to make updating the controls simpler
            TextBlock[] hourTexts = { Hour1, Hour2, Hour3, Hour4, Hour5 };
            System.Windows.Controls.Image[] images = { HourlyImage1, HourlyImage2, HourlyImage3, HourlyImage4, HourlyImage5 };
            TextBlock[] temps = {Temp1,  Temp2, Temp3, Temp4, Temp5};
            TextBlock[] precips = {Precip1, Precip2, Precip3, Precip4, Precip5};

            //get the current hour
            int currentHour = DateTime.Now.Hour;
            List<Hour> hours = forecast.ForecastdayArray[0].HourArray;
            
            for (int i = 0; i < 5; i++)
            {
                // set hour title text block
                hourTexts[i].Text = hours[currentHour].Time.Split(" ")[1];
                if (currentHour == 12) hourTexts[i].Text += " PM";
                else if (currentHour == 0) hourTexts[i].Text = "12:00 AM";
                else if (currentHour > 12) hourTexts[i].Text = (currentHour - 12).ToString() + ":00 PM";
                else hourTexts[i].Text += " AM";

                //set image
                images[i].Source = LoadBitmap(hours[currentHour].Condition.Icon.Split(".com")[1]);
                images[i].Width = 64;
                images[i].Height = 64;

                //set temp and precip
                temps[i].Text = hours[currentHour].TempF.ToString() + "°F";
                precips[i].Text = hours[currentHour].ChanceOfRain.ToString() + "%";

                //increment the current hour
                currentHour++;
                if(currentHour >= 24)
                {
                    //clock reset back to 0
                    currentHour = 0;
                    hours = forecast.ForecastdayArray[1].HourArray;
                }
            }
        }

        // this method is adapted from CreatureKingdom assignment
        private BitmapImage LoadBitmap(String assetsRelativePath, double decodeWidth = 64)
        {
            String[] strings = assetsRelativePath.Split("/");
            assetsRelativePath = "";
            foreach(String s in strings)
            {
                assetsRelativePath += s;
                if(strings.Last() != s)
                {
                    assetsRelativePath += @"\";
                }
            }

            BitmapImage theBitmap = new BitmapImage();
            theBitmap.BeginInit();
            String path = Environment.CurrentDirectory + assetsRelativePath;
            theBitmap.UriSource = new Uri(path, UriKind.Absolute);
            theBitmap.DecodePixelWidth = (int)decodeWidth;
            theBitmap.EndInit();

            return theBitmap;
        }

        private void UpdateAlerts(Alerts alerts)
        {
            AlertsListBox.Items.Clear();

            for(int i = 0; i < alerts.AlertArray.Count(); i++)
            {
                AlertsListBox.Items.Add(alerts.AlertArray[i].ToString());
            }
        }

        private void UpdateLocation(Location location)
        {
            LocationTextBox.Text = location.ToString();
        }

        private void UpdateCurrentConditions(Current current)
        {
            Temp.Text = current.TempF.ToString() + "°F";
            Condition.Text = current.Condition.Text;
            WindMPH.Text = current.WindMph.ToString() + "mph";
            WindDir.Text = current.WindDir;
            Precip.Text = current.PrecipIn.ToString() + " inches";
            Humidity.Text = current.Humidity.ToString() + "%";
            FeelsLike.Text = current.FeelslikeF.ToString() + "°F";
        }

        private void RefreshBtn_Click(object sender, RoutedEventArgs e)
        {
            InitializeWeatherData();
        }

        private void DocumentationBtn_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("This weather app was created by Joseph Hargus.\nFinal project for IT4400","Information", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void CreditsBtn_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show(@"Powered by https://www.weatherapi.com/", "Credits", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void LocationBtn_Click(object sender, RoutedEventArgs e)
        {
            LocationChangeWindow locationWindow = new LocationChangeWindow(this);
            locationWindow.ShowDialog();
        }
    }
}