using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace SmartHomeUCI
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class Temperature : Page
    {
        public static string temp_optimal = String.Empty;
        public List<double> In_Temp = new List<double>();
        public Temperature()
        {
            this.InitializeComponent();
            
        }

        DispatcherTimer _timer;
        BuildAzure.IoT.Adafruit.BME280.BME280Sensor _bme280;
        const float seaLevelPressure = 1022.00f;

       
        public async void Init_Temp()
        {
            _timer = new DispatcherTimer();
            _timer.Interval = TimeSpan.FromSeconds(1);
            _timer.Tick += ReadSensor;
            _bme280 = new BuildAzure.IoT.Adafruit.BME280.BME280Sensor();
            await _bme280.Initialize();
            _timer.Start();
        }
        
    
        private double F(double temp) { return Math.Round(((9.0 / 5.0) * temp + 32), 5); }
        private async void ReadSensor(object sender, object e)
        {
            var temp =await _bme280.ReadTemperature();
            var humidity = await _bme280.ReadHumidity();
            //var humidity = 45.0;
            In_Temp.Clear();
            In_Temp.Add(temp);
            In_Temp.Add(Math.Round(humidity, 4));

            Debug.WriteLine("Temp: {0} deg F", F(temp));
            Debug.WriteLine("Humidity: {0} %", humidity);

        }

    }
}
