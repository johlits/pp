using pp.Plugins;
using pp.Interfaces;
using System.Text;

namespace pp
{
    public class Program
    {
        public void AddInterfaces(object output) 
        {
            Core.SetInterface(new ConsoleInterface());
        }

        public void AddPlugins()
        {
            Core.AddPlugin(new Quit());
            Core.AddPlugin(new Help());
        }

        public IInterface GetUI() 
        {
            return Core.GetInterface();
        }

        public async Task Init(object output = null, string user = null, string event_name = null) 
        {
            Core.ReadConfig(user, event_name);
            AddInterfaces(output);
            AddPlugins();
            Core.DisplayVersion();
            Core.InitializeUserAndEvent();
            Core.DisplayUserAndEvent();
            Core.DisplayInput();
            await Core.GetInterface().Update();
        }

        static async Task Main()
        {
            var pp = new Program();
            await pp.Init(new StringBuilder(""));
        }
    }
}
