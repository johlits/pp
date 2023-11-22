using pp.Plugins;

namespace pp
{
    class Program
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

        static async Task Main()
        {
            Core.ReadConfig();
            AddInterfaces();
            AddPlugins();
            Core.DisplayVersion();
            Core.InitializeUserAndEvent();
            Core.DisplayUserAndEvent();
            Core.DisplayInput();
            await Core.GetInterface().Update();
        }
    }
}
