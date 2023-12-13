using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Component.Application.AI
{
    public class ChatGPTService
    {
        private const string apiKey = "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";
        private const string Model = "gpt-3.5-turbo-1106";

        public static async Task<string> CallApi(string request)
        {

            string apiEndpoint = "https://api.openai.com/v1/chat/completions";

            var message = new[]
            {
                new { role = "system", content = "You are a helpful assistant." },
                new { role = "user", content = request }
            };

            var data = new
            {
                model = Model,
                messages = message
            };

            string jsonConverted = JsonConvert.SerializeObject(data);
            var content = new StringContent(jsonConverted, Encoding.UTF8, "application/json");

            using (HttpClient client = new HttpClient())
            {
                client.DefaultRequestHeaders.Add("Authorization", "Bearer " + apiKey);
                var response = await client.PostAsync(apiEndpoint, content);
                string responseContent = await response.Content.ReadAsStringAsync();
                var jsonResult = JObject.Parse(responseContent);
                return jsonResult["choices"][0]["message"]["content"].Value<string>();
            }
        }
        public static async Task<string> GetGPTResult(string request)
        {
            return await CallApi(request);
        }
    }
}
