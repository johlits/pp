using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using pp.Interfaces;

namespace pp.Plugins
{
    internal class Quit : IPlugin
    {
        public bool ActivatesOn(string input)
        {
            return input == "/quit" || input == "/q";
        }

        public void Execute(string input)
        {
            Environment.Exit(0);
        }

        public string GetDescription()
        {
            return "Type /quit or /q to exit program.";
        }

        public List<string> GetCommands()
        {
            return new List<string> { "/quit", "/q" };
        }

        public void Ping(int no)
        {
            return;
        }
    }
}
