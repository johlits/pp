using Newtonsoft.Json;
using System.Net.Http.Headers;
using System.Net.Http.Json;

using pp.Interfaces;

namespace pp.Plugins
{
    internal class OpenAI : IPlugin
    {
        private static readonly HttpClient _httpClient = new HttpClient { BaseAddress = new Uri("https://api.openai.com/v1/") };

        public OpenAI(string? apiKey = null)
        {
            if (!string.IsNullOrEmpty(apiKey)) {
                _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", apiKey);
            }
            else {
                apiKey = Core.GetSecret("openaikey");
                if (!string.IsNullOrEmpty(apiKey)) {
                    _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", apiKey);
                }
            }
        }

        public async Task<string> SendPrompt(string prompt, string model)
        {
            var requestBody = new
            {
                prompt,
                model,
                max_tokens = 150,
                temperature = 0.5
            };

            var response = await _httpClient.PostAsJsonAsync("completions", requestBody);
            response.EnsureSuccessStatusCode();

            return await response.Content.ReadAsStringAsync();
        }

        public bool ActivatesOn(string input)
        {
            return input.StartsWith("@ai ");
        }

        public async void Execute(string input)
        {
            Core.DisplayMessage(Core.MC.None, Core.GetUserAlias(), Core.GetInput());

            int indexOfFirstOccurrence = input.IndexOf("@ai ");
            string prompt = input.Remove(indexOfFirstOccurrence, "@ai ".Length);

            Core.GetInterface().ClearInput();

            var model = "davinci";
            var output = JsonConvert.DeserializeObject<dynamic>(await SendPrompt(prompt, model)).choices[0].text.ToString();
            await Core.SendMessageAsync(output, model);
        }

        public List<string> GetCommands()
        {
            return new List<string> { "@ai <prompt>" };
        }

        public string GetDescription()
        {
            return "Type @ai <prompt> to prompt ChatGPT";
        }

        public void Ping(int no)
        {
            return;
        }

        public string GetPluginName()
        {
            return "openai";
        }
    }
}
