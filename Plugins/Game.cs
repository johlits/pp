using Newtonsoft.Json;

using pp.Interfaces;

namespace pp.Plugins
{
    internal class Game : IPlugin
    {
        private readonly HttpClient _httpClient;

        public Game()
        {
            _httpClient = new HttpClient { BaseAddress = new Uri("https://palplanner.com/cardgame/") };
        }

        public bool ActivatesOn(string input)
        {
            var words = new[] { "/cards", "/hand", "/pick", "/history", "/traits", "/game" };
            return words.Any(word => input.Contains(word, StringComparison.OrdinalIgnoreCase));
        }

        private string AddSymbolToWords(string sentence, string symbol)
        {
            var words = sentence.Split(" ");
            for (var i = 0; i < words.Length; i++)
            {
                words[i] = $"{symbol}{words[i]}";
            }
            return string.Join(" ", words);
        }

        private bool CheckGameStartOrEnd(dynamic output)
        {
            if (output == null)
            {
                return false;
            }

            var len = 0;
            foreach (var o in output)
            {
                len++;
            }

            if (len == 1 && output[0].ToString() == "game_start")
            {
                _ = Core.SendMessageAsync($"Game started!", Core.getUserAlias());
                return true;
            }
            else if (len == 2 && output[0].ToString() == "game_end")
            {
                _ = Core.SendMessageAsync($"Battle report: {output[1]}", Core.getUserAlias());
                return true;
            }

            return false;
        }

        private async Task<dynamic> PostAndGetResponseAsync(string action, string cardName = "")
        {
            var formVariables = new List<KeyValuePair<string, string>>
            {
                new KeyValuePair<string, string>("player_name", Core.getUserAlias()),
                new KeyValuePair<string, string>("player_action", action),
                new KeyValuePair<string, string>("event_name", Core.getEventTitle()),
                new KeyValuePair<string, string>("card_name", cardName),
                new KeyValuePair<string, string>("can_afford", "0")
            };

            var response = await _httpClient.PostAsync("server.php", new FormUrlEncodedContent(formVariables));
            response.EnsureSuccessStatusCode();

            var responseBody = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<dynamic>(responseBody);
        }

        private void DisplayGameMessage(string input, dynamic output)
        {
            if (CheckGameStartOrEnd(output))
            {
                return;
            }

            Core.DisplayMessage(Core.getUserAlias(), $"{AddSymbolToWords(output[0].Value.ToString(), "@")} | Day {AddSymbolToWords(output[1].Value.ToString(), "@")}/{output[3].Value.ToString()} | {AddSymbolToWords(output[2].Value.ToString(), "@")}s until next day");
        }

        public void OutputCards(dynamic output, bool includeCoins)
        {
            if (CheckGameStartOrEnd(output))
            {
                return;
            }

            var first = true;
            foreach (var card in output)
            {
                if (first && includeCoins)
                {
                    first = false;
                    Core.DisplayMessage(Core.getUserAlias(), $"Coins: !{card}");
                    continue;
                }

                var name = "";
                var atk = "";
                var hp = "";
                var cost = "";
                var traits = "";
                var golden = "";
                foreach (var property in card)
                {
                    if (property.Name == "name") name = property.Value;
                    if (property.Name == "atk") atk = property.Value;
                    if (property.Name == "hp") hp = property.Value;
                    if (property.Name == "cost") cost = property.Value;
                    if (property.Name == "traits") traits = string.Join(", ", property.Value.ToString().Split(","));
                    if (property.Name == "golden") golden = property.Value;
                }

                Core.DisplayMessage(Core.getUserAlias(), $"{AddSymbolToWords(name.ToString(), "#")} ATK {AddSymbolToWords(atk.ToString(), "@")} HP {AddSymbolToWords(hp.ToString(), "@")} COST {AddSymbolToWords(cost.ToString(), "!")} Traits {AddSymbolToWords(traits.ToString(), "@")} {(golden == "1" ? AddSymbolToWords("GOLDEN", "@") : "")}");
            }
        }

        public async void Execute(string input)
        {
            Core.GetInterface().ClearInput();
            Core.DisplayMessage(Core.getUserAlias(), $"{input}");
            dynamic output;

            if (input.StartsWith("/pick "))
            {

                var parameters = input.Split(" ", 2);
                var cardName = parameters[1];

                output = await PostAndGetResponseAsync("pickcard", cardName);

                var len = 0;
                foreach (var o in output)
                {
                    len++;
                }

                if (len == 0)
                {
                    Core.DisplayMessage(Core.getUserAlias(), $"Could not pick {AddSymbolToWords(cardName.ToString(), "#")}");
                }
                else
                {
                    _ = Core.SendMessageAsync($"Picked {AddSymbolToWords(output[0].Value.ToString(), "#")}", Core.getUserAlias());
                }
            }
            else
            {
                switch (input)
                {
                    case "/traits":
                        Core.DisplayMessage(Core.getUserAlias(), "https://palplanner.com/cardgame/traits.php");
                        break;

                    case "/history":
                        Core.DisplayMessage(Core.getUserAlias(), $"https://palplanner.com/cardgame/history.php?event={Core.getEventTitle().ToLower()}");
                        break;

                    case "/game":
                        output = await PostAndGetResponseAsync("game");
                        DisplayGameMessage(input, output);
                        break;

                    case "/cards":
                        output = await PostAndGetResponseAsync("gamecards");
                        OutputCards(output, false);
                        break;

                    case "/hand":
                        output = await PostAndGetResponseAsync("playercards");
                        OutputCards(output, true);
                        break;
                }
            }
        }

        public List<string> GetCommands()
        {
            return new List<string> { "/cards", "/hand", "/pick", "/history", "/traits", "/game" };
        }

        public string GetDescription()
        {
            return "Type /cards to show available cards, /hand to show cards in your hand, /pick <card> to pick a card from the board, and /game to show game info.";
        }

        public async void Ping(int no)
        {
            if (no % 10 == 0) {
                var formVariables = new List<KeyValuePair<string, string>>{};
                await _httpClient.PostAsync("bots.php", new FormUrlEncodedContent(formVariables));
            }
            return;
        }
    }
}
