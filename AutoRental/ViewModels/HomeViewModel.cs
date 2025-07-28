using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows.Input;
using System.Windows;
using BusinessObjects;
using Microsoft.EntityFrameworkCore;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;

namespace AutoRental.ViewModels
{
    public class WeatherInfo
    {
        public string Condition { get; set; } // "Sunny", "Rainy", ...
        public double Temperature { get; set; }
        public int Humidity { get; set; }
        public double WindSpeed { get; set; }
        public string Description { get; set; }
    }

    public class CarDisplay
    {
        public Car Car { get; set; }
        public string MainImageUrl { get; set; }
    }

    public class HomeViewModel : INotifyPropertyChanged
    {
        public Car SuggestedCar { get; set; }
        public WeatherInfo TodayWeather { get; set; }
        public ICommand BookCarCommand { get; set; }
        private List<Car> _allCars;

        public HomeViewModel()
        {
            LoadWeatherAndCars();
            BookCarCommand = new RelayCommand(BookCar);
        }

        private async void LoadWeatherAndCars()
        {
            TodayWeather = await GetTodayWeatherAsync("Da Nang");
            OnPropertyChanged(nameof(TodayWeather));
            var context = new AutoRentalPrnContext();
            _allCars = context.Cars.ToList();
            FilterAndSuggestCar();
        }

        private void FilterAndSuggestCar()
        {
            var filtered = FilterCarsByWeather(_allCars, TodayWeather);
            if (filtered.Any())
                SuggestedCar = GetRandomCar(filtered);
            else if (_allCars.Any())
                SuggestedCar = GetRandomCar(_allCars);
            OnPropertyChanged(nameof(SuggestedCar));
        }

        private void BookCar()
        {
            MessageBox.Show($"Bạn đã chọn đặt xe: {SuggestedCar?.CarModel}");
            // TODO: Mở màn hình đặt xe, truyền thông tin xe
        }

        public async Task<WeatherInfo> GetTodayWeatherAsync(string city = "Da Nang")
        {
            string apiKey = "14ed9cbcfc1f6006726b7df06bc777cd"; // <-- API key của bạn
            string url = $"https://api.openweathermap.org/data/2.5/weather?q={city}&appid={apiKey}&units=metric&lang=vi";

            using (var client = new HttpClient())
            {
                var response = await client.GetStringAsync(url);
                var data = JObject.Parse(response);

                return new WeatherInfo
                {
                    Condition = data["weather"][0]["main"].ToString(),
                    Temperature = (double)data["main"]["temp"],
                    Humidity = (int)data["main"]["humidity"],
                    WindSpeed = (double)data["wind"]["speed"],
                    Description = data["weather"][0]["description"].ToString()
                };
            }
        }

        public List<Car> FilterCarsByWeather(List<Car> cars, WeatherInfo weather)
        {
            if (weather == null)
                return cars;
            if (weather.Condition == "Rain")
                return cars.Where(c => c.Seats >= 4).ToList();
            if (weather.Condition == "Clear")
                return cars.Where(c => c.Seats <= 5).ToList();
            return cars;
        }

        public Car GetRandomCar(List<Car> cars)
        {
            var rnd = new Random();
            int idx = rnd.Next(cars.Count);
            return cars[idx];
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string name)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }
} 