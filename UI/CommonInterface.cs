using System.Reflection;
using System.Text.Json;
using System.Net;
using Microsoft.VisualBasic;
using System.Text;

namespace pp.Interfaces
{
    public class CommonInterface : IInterface
    {
        List<string> chats;
        string line;

        public CommonInterface(object o) {
            chats = (List<string>)o;
        }

        public void ClearInput()
        {
            return;
        }

        public void DisplayInput()
        {
            return;
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

        public void WriteLine(string line)
        {
            chats.Add(this.line + line);
            this.line = "";
        }
    }
}