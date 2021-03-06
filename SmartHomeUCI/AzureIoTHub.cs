﻿using System;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Azure.Devices.Client;

static class AzureIoTHub
{
    //
    // Note: this connection string is specific to the device "RPI1". To configure other devices,
    // see information on iothub-explorer at http://aka.ms/iothubgetstartedVSCS
    //
    const string deviceConnectionString = "HostName=SmartHomeUCI.azure-devices.net;DeviceId=RPI1;SharedAccessKey=eUiJthcaixg6WMcVZEVq3ZF8W2wCykyBNMQXBzQtpcY=";

    //
    // To monitor messages sent to device "RPI1" use iothub-explorer as follows:
    //    iothub-explorer monitor-events --login HostName=SmartHomeUCI.azure-devices.net;SharedAccessKeyName=service;SharedAccessKey=6bhOd0HrBwowZyayXcRMzggriIvexUqsULTweVK0h4I= "RPI1"
    //

    // Refer to http://aka.ms/azure-iot-hub-vs-cs-wiki for more information on Connected Service for Azure IoT Hub

    public static async Task SendDeviceToCloudMessageAsync(string msg)
    {
        var deviceClient = DeviceClient.CreateFromConnectionString(deviceConnectionString, TransportType.Mqtt);

#if WINDOWS_UWP
        var str = "{\"deviceId\":\"RPI1\",\"messageId\":1,\"text\":"+msg+"}";
#else
        var str = "{\"deviceId\":\"RPI1\",\"messageId\":1,\"text\":\"Hello, Cloud from a C# app!\"}";
#endif
        var message = new Message(Encoding.ASCII.GetBytes(str));

        await deviceClient.SendEventAsync(message);
    }

    public static async Task<string> ReceiveCloudToDeviceMessageAsync()
    {
        var deviceClient = DeviceClient.CreateFromConnectionString(deviceConnectionString, TransportType.Mqtt);

        while (true)
        {
            var receivedMessage = await deviceClient.ReceiveAsync();

            if (receivedMessage != null)
            {
                var messageData = Encoding.ASCII.GetString(receivedMessage.GetBytes());
                await deviceClient.CompleteAsync(receivedMessage);
                return messageData;
            }

            await Task.Delay(TimeSpan.FromSeconds(1));
        }
    }
}
