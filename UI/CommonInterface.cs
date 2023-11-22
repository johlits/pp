using System.Reflection;
using System.Text.Json;
using System.Net;
using Microsoft.VisualBasic;
using System.Text;

namespace pp.Interfaces
{
    public class CommonInterface : IInterface
    {
        StringBuilder sb;

        public CommonInterface(object o) {
            sb = (StringBuilder)o;
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
                Console.WriteLine(sb.ToString());
            }
        }

        public void Write(string line)
        {
            sb.Append(line);
        }

        public void WriteLine(string line)
        {
            sb.AppendLine(line);
        }
    }
}