using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using pp.Interfaces;

namespace pp.Plugins
{
    internal class Help : IPlugin
    {
        public string GetDescription()
        {
            return "Type /help for commands.";
        }

        public bool ActivatesOn(string input)
        {
            return input == "/help";
        }

        public void Execute(string input)
        {
            Core.DisplayMessage(Core.GetUserAlias(), string.Join(", ", Core.GetCommands()));
            Core.GetInterface().ClearInput();
        }

        public List<string> GetCommands()
        {
            return new List<string> { "/help" };
        }

        public void Ping(int no)
        {
            return;
        }
    }
}
