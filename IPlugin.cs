namespace pp
{
    internal interface IPlugin
    {
        public List<string> GetCommands();
        string GetDescription();
        bool ActivatesOn(string input);
        void Execute(string input);
        void Ping(int no);
    }
}
