using pp.Plugins;
using pp.Interfaces;

namespace pp
{
    public class Program
    {
        public void AddInterfaces() 
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

        public async Task Init(string? user, string? event_name) 
        {
            Core.ReadConfig(user, event_name);
            AddInterfaces();
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
            await pp.Init(null, null);
        }
    }
}
