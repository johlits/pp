namespace pp
{
    public interface IInterface
    {
        void SetH1Color();
        void SetH2Color();
        void SetUserColor(string name);
        void FormatWordColor(List<string> commands, string word);
        void ResetColor();
        void Write(string line);
        void WriteLine(string line);
        void ClearInput();
        void DisplayInput();
        string ReadLine();
        void InitializeUserAndEvent();
        Task Update();
    }
}
