namespace pp
{
    public interface IInterface
    {
        void SetH1Color();
        void SetH2Color();
        void SetUserColor(string name);
        void FormatWordColor(List<string> commands, string word);
        void ResetColor();
        void Write(string line, string? prependUser);
        void WriteLine(string line, string? prependUser, Core.MC flag = Core.MC.None);
        void ClearInput();
        void DisplayInput();
        string ReadLine();
        void InitializeUserAndEvent();
        void SendMessage(string s);
        void DisplayVersion();
        int GetChatCount();
        Task Update();
    }
}
