using pp.Plugins;
using pp.Interfaces;
using System.Text;

namespace pp
{
    public class Program
    {
        public void AddPlugins()
        {
            Core.AddPlugin(new Quit());
            Core.AddPlugin(new Help());
            Core.AddPlugin(new Download());
            Core.AddPlugin(new Game());
            Core.AddPlugin(new OpenAI());
        }

        public void AddInterfaces(object o) 
        {
            if (o == null) {
                Core.SetInterface(new ConsoleInterface());
            }
            else {
                Core.SetInterface(new CommonInterface(o));
            }
        }

        public IInterface GetUI() 
        {
            return Core.GetInterface();
        }

        public void Init(object output = null, string user = null, string event_name = null) 
        {
            Core.ReadConfig(user, event_name);
            AddInterfaces(output);
            AddPlugins();
        }

        public void SetUserAlias(string alias) {
            Core.SetUserAlias(alias);
        }

        public void SetEventTitle(string title) {
            Core.SetEventTitle(title);
        }
        public bool FirstFetch() {
            return Core.GetFirstFetch();
        }
        public int ChatCount() {
            return Core.GetInterface().GetChatCount();
        }

        public async Task Start() {
            Core.GetInterface().DisplayVersion();
            Core.GetInterface().InitializeUserAndEvent();
            Core.DisplayUserAndEvent();
            Core.GetInterface().DisplayInput();
            await Core.GetInterface().Update();
        }

        static async Task Main()
        {
            var pp = new Program();
            pp.Init();
            await pp.Start();
        }
    }
}
