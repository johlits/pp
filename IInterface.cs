namespace pp
{
    internal interface IInterface
    {
        void SetH1Color();
        void SetH2Color();
        void SetUserColor(string name);
        void FormatWordColor(List<string> commands, string word);
        void ResetColor();
        void Write(string line);
        void WriteLine(string line);
        void ClearInput();
        string ReadLine();
        Task Update();
    }
}
