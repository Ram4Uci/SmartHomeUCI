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



// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace SmartHomeUCI
{
    
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        DispatcherTimer _timer;
        
        const float seaLevelPressure = 1022.00f;
        BuildAzure.IoT.Adafruit.BME280.BME280Sensor _bme280;
        public MainPage()
        {
            this.InitializeComponent();
           
        }
        protected override async void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            _timer = new DispatcherTimer();
            _timer.Interval = TimeSpan.FromSeconds(1);
            _timer.Tick += ReadSensor;
            _bme280 = new BuildAzure.IoT.Adafruit.BME280.BME280Sensor();
            await _bme280.Initialize();
           
            _timer.Start();

        }
        private double F(double temp) { return Math.Round(((9.0 / 5.0) * temp + 32),5); }
        private double H(double alti) {  return Math.Round((3.2808 * alti),4); }
        private async void ReadSensor(object sender,object e)
        {
            var temp = await _bme280.ReadTemperature();
            var humidity = await _bme280.ReadHumidity();
            var pressure = await _bme280.ReadPressure();
            var altitude = await _bme280.ReadAltitude(seaLevelPressure);
            if (TempC.Visibility == Visibility.Visible)
                Temp.Text = "Temperature =" + temp.ToString();
            else
                Temp.Text = "Temperature =" + F(temp).ToString();
            Humi.Text = "Humidity =" + humidity.ToString()+ " %";
            Pres.Text = "Pressure =" + pressure.ToString()+ " Pa";
            if (AltiM.Visibility == Visibility.Visible)
                Alti.Text = "Altitude =" + altitude.ToString();
            else
                Alti.Text = "Altitude =" + H(altitude).ToString();
           // await AzureIoTHub.SendDeviceToCloudMessageAsync(Temp.Text);
           // await AzureIoTHub.SendDeviceToCloudMessageAsync(Pres.Text);
           // await AzureIoTHub.SendDeviceToCloudMessageAsync(Humi.Text);
           // await AzureIoTHub.SendDeviceToCloudMessageAsync(Alti.Text);

            Debug.WriteLine("Temp: {0} deg C", temp);
            Debug.WriteLine("Humidity: {0} %", humidity);
            Debug.WriteLine("Pressure: {0} Pa", pressure);
            Debug.WriteLine("Altitude: {0} m", altitude);
        }
        
        private void TempClick(object sender, DoubleTappedRoutedEventArgs e)
        {

            if (TempF.Visibility == Visibility.Collapsed)
            {
                TempC.Visibility = Visibility.Collapsed;
                TempF.Visibility = Visibility.Visible;
                ReadSensor(sender, e);
            }
            else
            {
                TempF.Visibility = Visibility.Collapsed;
                TempC.Visibility = Visibility.Visible;
                ReadSensor(sender, e);

            }

        }

        private void AltiClick(object sender, DoubleTappedRoutedEventArgs e)
        {
            if (AltiF.Visibility == Visibility.Collapsed)
            {
                AltiM.Visibility = Visibility.Collapsed;
                AltiF.Visibility = Visibility.Visible;
                ReadSensor(sender, e);
            }
            else
            {
                AltiF.Visibility = Visibility.Collapsed;
                AltiM.Visibility = Visibility.Visible;
                ReadSensor(sender, e);

            }

        }
    }
}
