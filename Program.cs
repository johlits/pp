using pp.Plugins;
using pp.Interfaces;

namespace pp
{
    public class Program
    {
        static void AddInterfaces() 
        {
            Core.SetInterface(new ConsoleInterface());
        }

        static void AddPlugins()
        {
            Core.AddPlugin(new Quit());
            Core.AddPlugin(new Help());
        }

        public static async Task Init() {
            Core.ReadConfig();
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
            await Init();
        }
    }
}
