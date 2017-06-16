using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Core;
using Windows.Devices.Enumeration;
using Windows.Devices.Gpio;
using Windows.Devices.I2c;
using Windows.UI.Xaml.Navigation;

namespace SmartHomeUCI
{
    class Adc
    {
        private I2cDevice _converter;
        public double temp;
        
        public GpioPin _inGpioPin;

        public async Task InitAsync()
        {
            var i2CSettings = new I2cConnectionSettings(0x48)
            {
                BusSpeed = I2cBusSpeed.FastMode,
                SharingMode = I2cSharingMode.Shared
            };

            var i2C1 = I2cDevice.GetDeviceSelector("I2C1");

            var devices = await DeviceInformation.FindAllAsync(i2C1);

            _converter = await I2cDevice.FromIdAsync(devices[0].Id, i2CSettings);
         
            _converter.Write(new byte[] { 0x01, 0xc4, 0xe0 });
            _converter.Write(new byte[] { 0x02, 0x00, 0x00 });
            _converter.Write(new byte[] { 0x03, 0xff, 0xff });

            var gpio = GpioController.GetDefault();
            _inGpioPin = gpio.OpenPin(5);
            _inGpioPin.ValueChanged += InGpioPinOnValueChanged;


        }
        public void ReadPin()
        {
            // Read conversion register
            // Lire le registre de conversion
            var bytearray = new byte[2];
            var bytearray1 = new byte[2];
            _converter.Write(new byte[] { 0x01, 0xc4, 0xe0 });
            _converter.WriteRead(new byte[] { 0x0 }, bytearray);

            // Convert to int16
            // Converti en int16
            if (BitConverter.IsLittleEndian)
                Array.Reverse(bytearray);

            var value = BitConverter.ToInt16(bytearray, 0);

            // Voltage = (value * gain)/372767
            // Volt = (value * gain)/372767
            var volt = (value * 2.048) / 32767.0;

            // Temperature = (volt - 0.5) * 100
            temp = (volt - .5) * 100;

            Debug.WriteLine($"Volt : {volt}  temp : {temp:f2}");

            _converter.Write(new byte[] { 0x01, 0xd4, 0xe0 });

            _converter.WriteRead(new byte[] { 0x0 }, bytearray1);

            // Convert to int16
            // Converti en int16
            if (BitConverter.IsLittleEndian)
                Array.Reverse(bytearray1);

            var value1 = BitConverter.ToInt16(bytearray1, 0);

            // Voltage = (value * gain)/372767
            // Volt = (value * gain)/372767
            var volt1 = (value1 * 2.048) / 32767.0;

            // Temperature = (volt - 0.5) * 100
            var temp1 = (volt1 - .5) * 100;

            Debug.WriteLine($"Volt1 : {volt1}  temp : {temp1:f2}");

          


        }


        private void InGpioPinOnValueChanged(GpioPin sender, GpioPinValueChangedEventArgs args)
        {
            // Read conversion register
            // Lire le registre de conversion
            var bytearray = new byte[2];
            var bytearray1 = new byte[2];
            _converter.Write(new byte[] { 0x01, 0xc4, 0xe0 });
            _converter.WriteRead(new byte[] { 0x0 }, bytearray);

            // Convert to int16
            // Converti en int16
            if (BitConverter.IsLittleEndian)
                Array.Reverse(bytearray);

            var value = BitConverter.ToInt16(bytearray, 0);

            // Voltage = (value * gain)/372767
            // Volt = (value * gain)/372767
            var volt = (value * 2.048) / 32767.0;

            // Temperature = (volt - 0.5) * 100
            temp = (volt - .5) * 100;

            Debug.WriteLine($"Volt : {volt}  temp : {temp:f2}");

            _converter.Write(new byte[] { 0x01, 0xd4, 0xe0 });

            _converter.WriteRead(new byte[] { 0x0 }, bytearray1);

            // Convert to int16
            // Converti en int16
            if (BitConverter.IsLittleEndian)
                Array.Reverse(bytearray1);

            var value1 = BitConverter.ToInt16(bytearray1, 0);

            // Voltage = (value * gain)/372767
            // Volt = (value * gain)/372767
            var volt1 = (value1 * 2.048) / 32767.0;

            // Temperature = (volt - 0.5) * 100
            var temp1 = (volt1 - .5) * 100;

            Debug.WriteLine($"Volt1 : {volt1}  temp : {temp1:f2}");

            


        }

    }
}
