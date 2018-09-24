using ChatBot.Ultilities.Interfaces;
using Newtonsoft.Json;
using System;
using System.Net.Http;
using System.Text;
using System.Web.Configuration;

namespace ChatBot.Ultilities.Instances
{
    public class TranslationService : ITranslationService
    {
        private static readonly string TranslateApiUrl = WebConfigurationManager.AppSettings["TranslateApiEndpoint"];

        private static readonly string ApiKey = WebConfigurationManager.AppSettings["TranslateApiKey"];

        private static readonly string DestinationLanguage = "en";

        public string TranslateText(string message)
        {
            System.Object[] body = new System.Object[] { new { Text = message } };
            var requestBody = JsonConvert.SerializeObject(body);

            using (var client = new HttpClient())
            using (var request = new HttpRequestMessage())
            {
                request.Method = HttpMethod.Post;
                request.RequestUri = new Uri($"{TranslateApiUrl}&to={DestinationLanguage}");
                request.Content = new StringContent(requestBody, Encoding.UTF8, "application/json");
                request.Headers.Add("Ocp-Apim-Subscription-Key", ApiKey);

                var response = client.SendAsync(request).Result;
                var responseBody = response.Content.ReadAsStringAsync().Result;
                var result = JsonConvert.SerializeObject(JsonConvert.DeserializeObject(responseBody), Formatting.Indented);

                Console.OutputEncoding = UnicodeEncoding.UTF8;
                return result;
            }
        }
    }
}