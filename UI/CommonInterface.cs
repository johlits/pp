using System.Reflection;
using System.Text.Json;
using System.Net;
using Microsoft.VisualBasic;
using System.Text;

namespace pp.Interfaces
{
    public class CommonInterface : IInterface
    {
        List<Tuple<string, int>> chats;
        string line;

        public CommonInterface(object o)
        {
            chats = (List<Tuple<string, int>>)o;
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
            
            WriteLine($"PalPlanner Common Version: {version}\n");
            foreach (var plugin in Core.GetPlugins())
            {
                SetH2Color();
                WriteLine($"* {plugin.GetDescription()}");
            }
            ResetColor();
            WriteLine("");
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

        public void Write(string line)
        {
            this.line += line;
        }

        public void WriteLine(string line, Core.MC flag = Core.MC.None)
        {
            chats.Add(new Tuple<string, int>(this.line + line, (int)flag));
            this.line = "";
        }
    }
}