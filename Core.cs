using System.Reflection;
using System.Text.Json;
using System.Net;

using pp.Interfaces;

namespace pp
{
    static class Core
    {
        private static string input = string.Empty;
        private static bool isBlockingInput = false;
        private static bool isBlockingFetch = false;
        private static int lastDisplayedChatId = -1;
        private static string? serverHost = "https://www.palplanner.com/chat/server.php";
        private static string? eventTitle = null;
        private static string? userAlias = null;
        private static int refreshDelay = 0;
        private static int initialRefreshDelay = 1000;
        private static int incrementalRefreshDelay = 1000;
        private static int inputRefreshRate = 10;
        private static int pingNo = 1;
        private static List<string> ignoredUsers = new List<string>();
        private static List<IPlugin> plugins = new List<IPlugin>();
        private static List<string> commands = new List<string>();
        private static IInterface ui;

        public static IInterface GetInterface() {
            return ui;
        }

        public static void SetInterface(IInterface ui) {
            Core.ui = ui;
        }
        public static bool GetIsBlockingInput() 
        {
            return isBlockingInput;
        }
        public static List<IPlugin> GetPlugins() 
        {
            return plugins;
        }
        public static int GetRefreshDelay()
        {
            return refreshDelay;
        }
        public static int GetInputRefreshRate()
        {
            return inputRefreshRate;
        }
        public static void SetUserAlias(string alias) {
            userAlias = alias;
        }
        public static string GetUserAlias()
        {
            return userAlias;
        }

        public static void SetEventTitle(string title) {
            eventTitle = title;
        }

        public static string GetEventTitle()
        {
            return eventTitle;
        }

        public static void SetInput(string input)
        {
            Core.input = input;
        }

        public static string GetInput()
        {
            return Core.input;
        }

        public static List<string> GetCommands()
        {
            return commands;
        }

        public static void AddPlugin(IPlugin plugin)
        {
            if (!plugins.Contains(plugin))
            {
                plugins.Add(plugin);
                commands.AddRange(plugin.GetCommands());
            }
        }

        public static void DisplayVersion()
        {
            var version = Assembly.GetEntryAssembly()?.GetCustomAttribute<AssemblyInformationalVersionAttribute>()?.InformationalVersion;

            ui.SetH1Color();
            

            ui.WriteLine($"PalPlanner CLI Version: {version}\n");
            foreach (var plugin in plugins)
            {
                ui.SetH2Color();
                ui.WriteLine($"* {plugin.GetDescription()}");
            }
            ui.ResetColor();
            ui.WriteLine("");
        }

        public static string GetFormattedWord(string word)
        {
            if (Uri.TryCreate(word, UriKind.Absolute, out Uri uriResult) && (uriResult.Scheme == Uri.UriSchemeHttp || uriResult.Scheme == Uri.UriSchemeHttps))
            {
                return $"<URL>{word.TrimStart('#', '@', '!')}</URL> ";
            }
            else
            {
                return $"{word.TrimStart('#', '@', '!')} ";
            }
        }

        public static async Task SendMessageAsync(string message, string user)
        {
            if (string.IsNullOrWhiteSpace(message)) return;

            string url = $"{serverHost}?event_name={eventTitle}&last_displayed_chat_id={lastDisplayedChatId}&user_name={user}&user_comment={Uri.EscapeDataString(message)}";
            using (var httpClient = new HttpClient())
            {
                try
                {
                    var response = await httpClient.GetAsync(url);
                    if (response.IsSuccessStatusCode)
                    {
                        ui.ClearInput();
                        refreshDelay = initialRefreshDelay;
                        await FetchMessagesAsync();
                    }
                    else
                    {
                        DisplayMessage(null, $"Error: {response.StatusCode} - {response.ReasonPhrase}");
                    }
                }
                catch (Exception ex)
                {
                    DisplayMessage(null, $"Error sending message: {ex.Message}");
                }
            }
        }

        public static string Prompt(string prompt)
        {
            ui.Write(prompt);
            return ui.ReadLine();
        }

        public static void DisplayUserAndEvent()
        {
            ui.Write($"Welcome to ");
            ui.SetH1Color();
            ui.Write($"{eventTitle}");
            ui.ResetColor();
            ui.Write($", ");
            ui.SetH1Color();
            ui.Write($"{userAlias}");
            ui.ResetColor();
            ui.Write($"!\n");
        }

        public static void ReadConfig(string? user, string? event_name)
        {
            string filePath = "config.cfg";

            if (File.Exists(filePath))
            {
                try
                {
                    foreach (var line in File.ReadLines(filePath))
                    {
                        if (line.Contains('='))
                        {
                            var parts = line.Split(new[] { '=' }, 2);
                            switch (parts[0])
                            {
                                case "host": serverHost = string.IsNullOrEmpty(parts[1]) ? null : parts[1]; break;
                                case "user": userAlias = string.IsNullOrEmpty(parts[1]) ? null : parts[1]; break;
                                case "event": eventTitle = string.IsNullOrEmpty(parts[1]) ? null : parts[1]; break;
                                case "initialRefresh": initialRefreshDelay = int.Parse(parts[1]); break;
                                case "incrementalRefresh": incrementalRefreshDelay = int.Parse(parts[1]); break;
                                case "inputRefresh": inputRefreshRate = int.Parse(parts[1]); break;
                                case "ignore": ignoredUsers.AddRange(parts[1].Split(",")); break;
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    ui.WriteLine($"Error reading config file: {ex.Message}");
                }
            }

            if (user != null) {
                userAlias = user;
            }
            if (event_name != null) {
                eventTitle = event_name;
            }
        }

        public static void DisplayMessage(string? user, string message)
        {
            message = WebUtility.HtmlDecode(message);

            isBlockingInput = true;
            ui.ClearInput();

            if (user != null)
            {
                ui.SetUserColor(user);
                ui.Write($"{user}");
                ui.ResetColor();
                ui.Write($": ");
            }

            foreach (var word in message.Split(" "))
            {
                ui.FormatWordColor(GetCommands(), word);
                ui.Write(GetFormattedWord(word));
                ui.ResetColor();
            }

            ui.WriteLine("");
            //ui.ClearLines(1);

            ui.DisplayInput();
            isBlockingInput = false;
        }

        public static async Task FetchMessagesAsync()
        {
            if (isBlockingFetch) return;
            isBlockingFetch = true;

            refreshDelay += incrementalRefreshDelay;

            using (var httpClient = new HttpClient())
            {
                try
                {
                    var url = $"{serverHost}?event_name={eventTitle}&last_displayed_chat_id={lastDisplayedChatId}";
                    var response = await httpClient.GetAsync(url);

                    if (response.IsSuccessStatusCode)
                    {
                        var jsonDocument = JsonDocument.Parse(await response.Content.ReadAsStringAsync());
                        var root = jsonDocument.RootElement;

                        if (root.ValueKind == JsonValueKind.Array)
                        {
                            foreach (var element in root.EnumerateArray())
                            {
                                int chatId = -1;
                                string username = "";
                                string chat = "";

                                foreach (var property in element.EnumerateObject())
                                {
                                    var (propertyName, propertyValue) = (property.Name, property.Value);
                                    switch (propertyName)
                                    {
                                        case "chat_id": chatId = int.Parse(propertyValue.ToString()); break;
                                        case "user_name": username = propertyValue.ToString(); break;
                                        case "user_comment": chat = propertyValue.ToString(); break;
                                    }
                                }

                                refreshDelay = initialRefreshDelay;
                                lastDisplayedChatId = Math.Max(lastDisplayedChatId, chatId);

                                if (!ignoredUsers.Contains(username)) DisplayMessage(username, chat);
                            }
                        }
                    }
                    else
                    {
                        DisplayMessage(null, $"Error: {response.StatusCode} - {response.ReasonPhrase}");
                    }
                }
                catch (Exception ex)
                {
                    DisplayMessage(null, $"Error fetching messages: {ex.Message}");
                }
            }

            foreach (var plugin in plugins) {
                plugin.Ping(pingNo);
            }
            pingNo++;

            isBlockingFetch = false;
        }
    }
}
