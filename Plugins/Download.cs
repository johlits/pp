namespace pp.Plugins
{
    internal class Download : IPlugin
    {
        public string GetDescription()
        {
            return "Type /download <File> to download a plugin.";
        }

        public bool ActivatesOn(string input)
        {
            return input.StartsWith("/download ");
        }

        private async Task DownloadFileAsync(string url, string destinationPath)
        {
            using (HttpClient client = new HttpClient())
            {
                try
                {
                    using (HttpResponseMessage response = await client.GetAsync(url, HttpCompletionOption.ResponseHeadersRead))
                    {
                        response.EnsureSuccessStatusCode();

                        using (Stream streamToReadFrom = await response.Content.ReadAsStreamAsync())
                        {
                            using (Stream streamToWriteTo = File.Open(destinationPath, FileMode.Create))
                            {
                                await streamToReadFrom.CopyToAsync(streamToWriteTo);
                            }
                        }
                    }
                }
                catch (HttpRequestException ex)
                {
                    Core.DisplayMessage(Core.MC.None, Core.GetUserAlias(), $"Error downloading the file: {ex.Message}");
                }
            }
        }

        public async void Execute(string input)
        {
            var filename = input.Split(" ")[1];
            string fileUrl = $"https://palplanner.com/cli/plugins/{filename}.cs";
            string currentDirectory = AppDomain.CurrentDomain.BaseDirectory;
            string projectDirectory = Path.Combine(currentDirectory, "..", "..", "..");
            string projectFolderPath = projectDirectory + "\\Plugins";
            string fileName = $"{filename}.cs";

            await DownloadFileAsync(fileUrl, Path.Combine(projectFolderPath, fileName));

            Core.DisplayMessage(Core.MC.None, Core.GetUserAlias(), $"File {filename}.cs downloaded and saved to: {projectFolderPath}. To install the plugin, add it in Program.cs and recompile project. ");
            Core.GetInterface().ClearInput();
        }

        public List<string> GetCommands()
        {
            return new List<string> { "/download" };
        }

        public void Ping(int no)
        {
            return;
        }

        public string GetPluginName()
        {
            return "download";
        }
    }
}