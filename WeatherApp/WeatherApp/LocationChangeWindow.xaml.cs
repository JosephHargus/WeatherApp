using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
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
using System.Windows.Shapes;

namespace WeatherApp
{
    /// <summary>
    /// Interaction logic for LocationChangeWindow.xaml
    /// </summary>
    public partial class LocationChangeWindow : Window
    {
        private MainWindow mainWindow;
        private List<String> savedLocations;
        private const String filePath = "SavedLocations.json";

        public LocationChangeWindow(MainWindow mainWindow)
        {
            InitializeComponent();
            this.mainWindow = mainWindow;

            savedLocations = new List<String>();
            LoadSavedLocations();

            LocationListBox.ItemsSource = savedLocations;
            LocationListBox.Items.Refresh();
        }

        private void DelLocationBtn_Click(object sender, RoutedEventArgs e)
        {
            savedLocations.Remove(LocationListBox.SelectedItem.ToString());
            LocationListBox.Items.Refresh();
            SaveSavedLocations();
        }

        private async void AddLocationBtn_Click(object sender, RoutedEventArgs e)
        {
            //check that user has entered a new location
            if(NewLocationTextBox.Text == null || NewLocationTextBox.Text.Equals("") || NewLocationTextBox.Text.Equals("New Location"))
            { return; }


            //get data
            WeatherData weatherData = new WeatherData();
            await weatherData.GetWeatherDataAsync(NewLocationTextBox.Text);
            Location newLoc = weatherData.Location;

            //add location to list, unless already added
            if (!LocationListBox.Items.Contains(newLoc.ToString()))
            {
                savedLocations.Add(newLoc.ToString());
                LocationListBox.Items.Refresh();
            }

            SaveSavedLocations();
        }

        private async void LocationListBox_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            WeatherData weatherData = new WeatherData();
            try
            {
                //get data
                await weatherData.GetWeatherDataAsync(LocationListBox.SelectedItem.ToString());
            }
            catch
            {
                mainWindow.LocationTextBox.Text = "Error occurred while fetching weather data";
            }

            if (weatherData.Location != null && weatherData.Forecast != null)
            {
                //update window
                mainWindow.UpdateWeatherDataDisplay(weatherData);
            }

            //close location change window
            Close();
        }

        private void LoadSavedLocations()
        {
            String json;
            try
            {
                json = File.ReadAllText(filePath);
            }
            catch (FileNotFoundException)
            {
                // there are no previously saved locations
                return;
            }
            catch
            {
                MessageBox.Show("Error loading saved locations");
                return;
            }

            if (json != null && json.Length > 0)
            {
                List<String> locationsList = JsonConvert.DeserializeObject<List<String>>(json);
                if (locationsList != null) savedLocations = locationsList;
            }
        }

        public void SaveSavedLocations()
        {
            String json = JsonConvert.SerializeObject(savedLocations);

            try
            {
                File.WriteAllText(filePath, json);
            }
            catch
            {
                MessageBox.Show("Error saving locations", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void NewLocationTextBox_KeyDown(object sender, KeyEventArgs e)
        {
            if(e.Key == Key.Enter)
            {
                AddLocationBtn_Click(sender, e);
            }
        }
    }
}
