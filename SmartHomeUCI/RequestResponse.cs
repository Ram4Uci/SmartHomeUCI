using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace SmartHomeUCI
{
    public class StringTable
    {
        public string[] ColumnNames { get; set; }
        public string[,] Values { get; set; }
    }
    public class Value
    {
        public List<string> ColumnNames { get; set; }
        public List<string> ColumnTypes { get; set; }
        public List<List<string>> Values { get; set; }
    }

    public class Output1
    {
        public string type { get; set; }
        public Value value { get; set; }
    }

    public class Results
    {
        public Output1 output1 { get; set; }
    }

    public class Item
    {
        public Results Results { get; set; }
    }
    public class RequestResponse
    {

        /* public static void MainResponse()
         {
             InvokeRequestResponseService().Wait();
         }
         */
        public static Temperature temp = new Temperature();
        private static double F(double temp) { return Math.Round(((9.0 / 5.0) * temp + 32), 5); }
        public static async Task InvokeRequestResponseService(List<string> ValIn)
        {
            
            using (var client = new HttpClient())
            {
                 
                var scoreRequest = new
                {

                    Inputs = new Dictionary<string, StringTable>() {
                        {
                            "input1",
                            new StringTable()
                            {
                                ColumnNames = new string[] {"Date", "Time", "Temperature_Sensor", "Weather_Temperature", "Humidity_Sensor", "Lighting_Sensor", "Temperature_Exterior_Sensor", "Humidity_Exterior_Sensor", "Day_Of_Week", "Temperature_Set"},
                                Values =  new string[,]{ {ValIn[0],ValIn[1],ValIn[2],ValIn[3],ValIn[4],ValIn[5],ValIn[6],ValIn[7],ValIn[8],ValIn[9]}, }

                            }
                        },
                    },
                    GlobalParameters = new Dictionary<string, string>()
                    {
                    }
                };
                const string apiKey = "FoDlCEAc+i4xnU9zL8AGLXaqtXmsxUhwvaQGNECvyYxtaCBZE2ml7Tc4KcAkM1aj/HFcIRD0CBLDvomHjoofbw=="; // Replace this with the API key for the web service
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", apiKey);

                client.BaseAddress = new Uri("https://ussouthcentral.services.azureml.net/workspaces/4370dbf620d8433186f4b756542996d1/services/9469ce43ac0c4c42b37aaee5506c8c4d/execute?api-version=2.0&details=true");

                // WARNING: The 'await' statement below can result in a deadlock if you are calling this code from the UI thread of an ASP.Net application.
                // One way to address this would be to call ConfigureAwait(false) so that the execution does not attempt to resume on the original context.
                // For instance, replace code such as:
                //      result = await DoSomeTask()
                // with the following:
                //      result = await DoSomeTask().ConfigureAwait(false)


                HttpResponseMessage response = await client.PostAsJsonAsync("", scoreRequest).ConfigureAwait(false);

                if (response.IsSuccessStatusCode)
                {
                    string result = await response.Content.ReadAsStringAsync();
                    Debug.WriteLine(result);
                    var list = JsonConvert.DeserializeObject<Item>(result);
                    MainPage.optimal_temp = F(Convert.ToDouble(list.Results.output1.value.Values[0][10]));
                    Temperature.temp_optimal  = "Optimal Temperature = "+ MainPage.optimal_temp.ToString() ;
                    
                }
                else
                {
                    Debug.WriteLine(string.Format("The request failed with status code: {0}", response.StatusCode));

                    // Print the headers - they include the requert ID and the timestamp, which are useful for debugging the failure
                    Debug.WriteLine(response.Headers.ToString());

                    string responseContent = await response.Content.ReadAsStringAsync();
                    Debug.WriteLine(responseContent);
                }
            }
        }
    }
}
