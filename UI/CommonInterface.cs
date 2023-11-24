using System.Reflection;
using System.Text.Json;
using System.Net;
using Microsoft.VisualBasic;
using System.Text;

namespace pp.Interfaces
{
    public class CommonInterface : IInterface
    {
        List<Tuple<string, string, int>> chats;
        string line;
        string user;

        public CommonInterface(object o)
        {
            chats = (List<Tuple<string, string, int>>)o;
        }

        public void ClearInput()
        {
            return;
        }

        public void DisplayInput()
        {
            return;
        }

        public void DisplayVersion()
        {
            var version = Assembly.GetEntryAssembly()?.GetCustomAttribute<AssemblyInformationalVersionAttribute>()?.InformationalVersion;

            SetH1Color();
            
            WriteLine($"PalPlanner Common Version: {version}\n", null);
            foreach (var plugin in Core.GetPlugins())
            {
                SetH2Color();
                WriteLine($"* {plugin.GetDescription()}", null);
            }
            ResetColor();
            WriteLine("", null);
        }

        public void FormatWordColor(List<string> commands, string word)
        {
            return;
        }

        public void InitializeUserAndEvent()
        {
            return;
        }

        public string ReadLine()
        {
            return null;
        }

        public void ResetColor()
        {
            return;
        }

        public void SendMessage(string s)
        {
            bool pluginActivated = false;
            foreach (var plugin in Core.GetPlugins())
            {
                if (plugin.ActivatesOn(s))
                {
                    pluginActivated = true;
                    plugin.Execute(s);
                }
            }

            if (!pluginActivated)
            {
                _ = Core.SendMessageAsync(s, Core.GetUserAlias());
            }
        }

        public void SetH1Color()
        {
            return;
        }

        public void SetH2Color()
        {
            return;
        }

        public void SetUserColor(string name)
        {
            return;
        }

        public async Task Update()
        {
            while (true)
            {
                await Core.FetchMessagesAsync();
                await Task.Delay(Core.GetRefreshDelay());
            }
        }

        public void Write(string line, string? prependUser)
        {
            this.line += line;
            if (prependUser != null) {
                user = prependUser;
            }
        }

        public void WriteLine(string line, string? prependUser, Core.MC flag = Core.MC.None)
        {
            if (prependUser != null) {
                user = prependUser;
            }
            chats.Add(new Tuple<string, string, int>(string.IsNullOrEmpty(user) ? "" : user, this.line + line, (int)flag));
            this.line = "";
            this.user = "";
        }
    }
}