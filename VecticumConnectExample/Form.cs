using Newtonsoft.Json.Linq;
using RestSharp;
using System;
using System.Threading.Tasks;
using System.Net;

namespace VecticumConnect
{
    
    public partial class Form : System.Windows.Forms.Form
    {
        public class Api
        {
            public static string url = "https://app.vecticum.com/api/v1/";
            public static string client_id = ""; // Enter Your Vecticum Client ID 
            public static string client_secret = ""; // Enter Your Vecticum Secret Key 
        }

        public Form()
        {
            InitializeComponent();
            
        }

        private async void btnConnect_Click(object sender, EventArgs e)
        {
            string token = await Login(); // Authorize
            JArray businessTrips = await GetData(token, "ClRdJg3K7apj4x84Ny5y"); // Get Business trips
            JArray invoices = await GetData(token, "Z9OVQWEWH7bYmO1ydt7O"); // Get Invoices
        }

        private async Task<string> Login()
        {
            JObject authorization = new JObject(
                new JProperty("client_id", Api.client_id),
                new JProperty("client_secret", Api.client_secret)
                );

            var client = new RestClient(Api.url + "oauth/token");
            client.Timeout = -1;

            var request = new RestRequest(Method.POST);
            request.AddHeader("Content-Type", "application/json");
            request.AddHeader("Authorization", authorization.ToString().Replace("\r\n", ""));
            IRestResponse response = await client.ExecuteAsync<RestResponse>(request);

            if (response.StatusCode != HttpStatusCode.OK)
            {
                throw new  Exception("Authorization failed");
            }

            JObject json = JObject.Parse(response.Content);
            Console.WriteLine(response.Content);
            return json.GetValue("token").ToString();
        }

        private async Task<JArray> GetData(string token, string objectType)
        {

            var client = new RestClient(Api.url + objectType);
            client.Timeout = -1;
            var request = new RestRequest(Method.GET);
            request.AddHeader("Content-Type", "application/json");
            request.AddHeader("Authorization", "Bearer "+ token);
            request.AddParameter("application/json", "\r\n", ParameterType.RequestBody);
            IRestResponse response = await client.ExecuteAsync<RestResponse>(request);

            if (response.StatusCode != HttpStatusCode.OK)
            {
                throw new Exception("Get Data failed");
            }

            Console.WriteLine(response.Content);

            JArray result = JArray.Parse(response.Content);
            return result;
        }
    }
}
