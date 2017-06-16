using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Navigation;
using System.Threading.Tasks;
using Windows.Devices.Gpio;



// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace SmartHomeUCI
{

    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {

        public static int prevHour=0;
        public int count = 0;
        DateTime localDate = DateTime.Now;
        Windows.Storage.StorageFolder storageFolder =
        Windows.Storage.ApplicationData.Current.LocalFolder;
        Windows.Storage.StorageFile TempFile;
        DispatcherTimer _timer;
        public static double optimal_temp;
        double out_temp, in_temp;
        Adc temp = new Adc();
        StringTable content = new StringTable();
        List<string> content_list = new List<string>();
        Temperature Main_Temp = new Temperature();
        public GpioPin Fan_pin,Window_pin,User_pin;



        public MainPage()
        {
            this.InitializeComponent();
            CreateFile();


        }
       
        private async void CreateFile()
        {
            TempFile = await storageFolder.CreateFileAsync("Weather.json", Windows.Storage.CreationCollisionOption.OpenIfExists);
            _timer = new DispatcherTimer();
            _timer.Interval = TimeSpan.FromSeconds(1);
            _timer.Tick += GetWeatherData;
            int hour = localDate.Hour;
            Debug.WriteLine(hour);
            Init_Values();
            RequestResponse.InvokeRequestResponseService(content_list).Wait();
            Main_Temp.Init_Temp();
            var gpio = GpioController.GetDefault();
            Fan_pin=gpio.OpenPin(27);
            Window_pin=gpio.OpenPin(22);
            User_pin = gpio.OpenPin(24);
            SetBg(hour);
            
            _timer.Start();
        }
        private void Init_Values()
        {
            content_list.Clear();
            content_list.Add(DateTime.Now.Date.ToString("dd/MM/yyyy"));
            Debug.WriteLine(content_list[0]);
            content_list.Add(DateTime.Now.ToString("HH:mm:ss"));
            Debug.WriteLine(content_list[1]);
            content_list.Add("20");
            content_list.Add("22");
            content_list.Add("40");
            content_list.Add("10");
            content_list.Add("25");
            content_list.Add("45");
            content_list.Add(((int)DateTime.Now.DayOfWeek).ToString());
            Debug.WriteLine(content_list[8]);
            content_list.Add("27");
           
        }
        private void SetBg(int hour)
        {
            if (hour < 6 || hour > 18)
            {
                Main.Background = new ImageBrush { ImageSource = new BitmapImage(new Uri(this.BaseUri, "Assets/Night_Home.jpg")), Stretch = Stretch.Fill };
            }
            else
            {
                Main.Background = new ImageBrush { ImageSource = new BitmapImage(new Uri(this.BaseUri, "Assets/Day_Home.jpg")), Stretch = Stretch.Fill };
            }
        }
        private void SetMessage(int hour)
        {
            SetBg(hour);
            if(hour < 6)
            {
                Welcome.Text = "Good Night";
                Welcome.Foreground = new SolidColorBrush(Windows.UI.Colors.White);
                User.Foreground = new SolidColorBrush(Windows.UI.Colors.White);
                Temp.Foreground = new SolidColorBrush(Windows.UI.Colors.White);
                Time.Foreground = new SolidColorBrush(Windows.UI.Colors.White);
                Value.Foreground = new SolidColorBrush(Windows.UI.Colors.White);
                LowTemp.Foreground = new SolidColorBrush(Windows.UI.Colors.White);
                HighTemp.Foreground = new SolidColorBrush(Windows.UI.Colors.White);

            }
            else if ( hour < 12)
            {
                Welcome.Text = "Good Morning";
                Welcome.Foreground = new SolidColorBrush(Windows.UI.Colors.Black);
                User.Foreground = new SolidColorBrush(Windows.UI.Colors.Black);
                Temp.Foreground = new SolidColorBrush(Windows.UI.Colors.Black);
                Time.Foreground = new SolidColorBrush(Windows.UI.Colors.Black);
                Value.Foreground = new SolidColorBrush(Windows.UI.Colors.White);
                LowTemp.Foreground = new SolidColorBrush(Windows.UI.Colors.White);
                HighTemp.Foreground = new SolidColorBrush(Windows.UI.Colors.White);
            }
            else if(hour < 18)
            {
                Welcome.Text = "Good Afternoon";
                Welcome.Foreground = new SolidColorBrush(Windows.UI.Colors.Black);
                User.Foreground = new SolidColorBrush(Windows.UI.Colors.Black);
                Temp.Foreground = new SolidColorBrush(Windows.UI.Colors.Black);
                Time.Foreground = new SolidColorBrush(Windows.UI.Colors.Black);
                Value.Foreground = new SolidColorBrush(Windows.UI.Colors.Black);
                LowTemp.Foreground = new SolidColorBrush(Windows.UI.Colors.White);
                HighTemp.Foreground = new SolidColorBrush(Windows.UI.Colors.White);
            }
            else
            {
                Welcome.Text = "Good Evening";
                Welcome.Foreground = new SolidColorBrush(Windows.UI.Colors.White);
                User.Foreground = new SolidColorBrush(Windows.UI.Colors.White);
                Temp.Foreground = new SolidColorBrush(Windows.UI.Colors.White);
                Time.Foreground = new SolidColorBrush(Windows.UI.Colors.White);
                Value.Foreground = new SolidColorBrush(Windows.UI.Colors.White);
                LowTemp.Foreground = new SolidColorBrush(Windows.UI.Colors.White);
                HighTemp.Foreground = new SolidColorBrush(Windows.UI.Colors.White);

            }
        }

        double DegToTemp(double deg)
        {
            return (Math.Round(60 + (deg / 6.0)));
        }
            private double F(double temp) { return Math.Round(((9.0 / 5.0) * temp + 32), 5); }
            private double C(double temp) { return Math.Round((temp - 32)*(5.0/9.0), 5); }


            private async void GetWeatherData(object sender,object e)
        {
            int zipcode = 92612;
            DateTime localTime = DateTime.Now;
                
                if (localTime.Minute % 15 == 0 || count ==0)
                {
                    RootObject myWeather = await OpenWeatherMapProxy.GetWeather(zipcode);
                    Temp.Text = myWeather.main.temp.ToString() + "°F";
                    out_temp = myWeather.main.temp;
                   
                    var serializer = new DataContractJsonSerializer(typeof(RootObject));
                    using (var stream = await storageFolder.OpenStreamForWriteAsync("Weather.json",Windows.Storage.CreationCollisionOption.ReplaceExisting))
                    {
                       serializer.WriteObject(stream,myWeather);
                    }
                content_list.Clear();
                content_list.Add(DateTime.Now.Date.ToString("dd/MM/yyyy"));
                content_list.Add(DateTime.Now.ToString("HH:mm:ss"));
                content_list.Add(Main_Temp.In_Temp[0].ToString());
                content_list.Add(C(myWeather.main.temp).ToString());
                content_list.Add(Main_Temp.In_Temp[1].ToString());
                content_list.Add("0");
                content_list.Add(C(myWeather.main.temp).ToString());
                content_list.Add(myWeather.main.humidity.ToString());
                content_list.Add(((int)DateTime.Now.DayOfWeek).ToString());
                content_list.Add(DegToTemp(Math.Round(Dial.Value)).ToString());
                RequestResponse.InvokeRequestResponseService(content_list).Wait();
                Temp1.Text = Temperature.temp_optimal;
                count = 1;
                }
                else
                {
                try
                {
                    var serializer = new DataContractJsonSerializer(typeof(RootObject));
                    using (var stream = await storageFolder.OpenStreamForReadAsync("Weather.json"))
                    {
                        RootObject myWeather = (RootObject)serializer.ReadObject(stream);
                        Temp.Text = myWeather.main.temp.ToString() + "°F";
                       
                    }
                }
                catch
                {
                    Debug.WriteLine("Exception");
                }
                }
                if(prevHour != localTime.Hour)
                {
                SetMessage(localTime.Hour);
               // Debug.WriteLine(localTime.Hour);
                prevHour = localTime.Hour;
                }
            Time.Text = localTime.ToString("ddd,MM/dd hh:mm tt");
            Value.Text = DegToTemp(Math.Round(Dial.Value)).ToString();
            if (User_pin.Read() == GpioPinValue.High)
            {
                if (out_temp < optimal_temp)
                {
                    if (out_temp < Main_Temp.In_Temp[0])
                    {
                        Window_pin.Write(GpioPinValue.Low);
                    }
                    else if (Window_pin.Read() == GpioPinValue.Low)
                    {
                        Window_pin.Write(GpioPinValue.High);
                    }

                }
                else
                {
                    if (Main_Temp.In_Temp[0] > optimal_temp)
                    {
                        Fan_pin.Write(GpioPinValue.Low);
                    }
                    else if (Fan_pin.Read() == GpioPinValue.Low)
                    {
                        Fan_pin.Write(GpioPinValue.High);
                    }

                }
            }
            //temp.ReadPin();
            //emp1.Text = temp.temp.ToString();
            Debug.WriteLine("ok");
            


        }

        private void Energy_Click(object sender, RoutedEventArgs e)
        {
            this.Frame.Navigate(typeof(Energy));
        }

        private void Weather_Click(object sender, RoutedEventArgs e)
        {
            this.Frame.Navigate(typeof(BlankPage1));
        }
    } 

}
