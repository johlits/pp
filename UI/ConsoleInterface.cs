using System.Reflection;
using System.Text.Json;
using System.Net;

namespace pp.Interfaces
{
    public class ConsoleInterface : IInterface
    {
        public async Task CheckForInput()
        {
            while (true)
            {
                if (Console.KeyAvailable)
                {
                    ConsoleKeyInfo key = Console.ReadKey(intercept: true);
                    ProcessKeyPress(key);
                }

                await Task.Delay(Core.GetInputRefreshRate());
            }
        }

        public int GetInputLines(int offset = 0)
        {
            return ($"{Core.getUserAlias()}: {Core.GetInput()}".Length - 2 + offset) / Console.WindowWidth + 1;
        }

        public void ProcessKeyPress(ConsoleKeyInfo key)
        {
            var offset = 0;
            if (key.Key == ConsoleKey.Enter)
            {
                bool pluginActivated = false;
                foreach (var plugin in Core.GetPlugins())
                {
                    if (plugin.ActivatesOn(Core.GetInput()))
                    {
                        pluginActivated = true;
                        plugin.Execute(Core.GetInput());
                    }
                }

                if (!pluginActivated)
                {
                    _ = Core.SendMessageAsync(Core.GetInput(), Core.getUserAlias());
                }
            }
            else if (key.Key == ConsoleKey.Backspace)
            {
                if (Core.GetInput().Length > 0) Core.SetInput(Core.GetInput().Substring(0, Core.GetInput().Length - 1));
                offset = 2;
            }
            else
            {
                Core.SetInput(Core.GetInput() + key.KeyChar);
            }

            if (!Core.GetIsBlockingInput())
            {
                ClearLines(GetInputLines(offset));
                DisplayInput();
            }
        }

        public void ClearLines(int lines)
        {
            int currentLineCursor = Console.CursorTop;
            Console.SetCursorPosition(0, Console.CursorTop + 1 - lines);

            for (var i = 0; i < lines; i++)
            {
                Console.Write(new string(' ', Console.WindowWidth));
            }

            Console.SetCursorPosition(0, currentLineCursor - lines + 1);
        }

        public string ReadLine()
        {
            return Console.ReadLine();
        }

        public void ResetColor()
        {
            Console.ResetColor();
        }

        public void SetForegroundColor(ConsoleColor color)
        {
            Console.ForegroundColor = color;
        }

        public void Write(string line)
        {
            Console.Write(line);
        }

        public void WriteLine(string line)
        {
            Console.WriteLine(line);
        }

        public ConsoleColor GetWordColor(List<string> commands, string word)
        {
            return word.StartsWith("#") ? GetHashtagColor() : (word.StartsWith("@") ? GetAtColor() : (word.StartsWith("!") ? GetMarkColor() : (commands.Contains(word) && word.StartsWith("/") ? GetCmdColor() : ConsoleColor.Gray)));
        }

        public ConsoleColor GetUserColor(string username)
        {
            int hash = username.GetHashCode();
            int index = Math.Abs(hash) % Enum.GetNames(typeof(ConsoleColor)).Length;
            return (ConsoleColor)index;
        }

        public ConsoleColor GetHashtagColor()
        {
            return ConsoleColor.Green;
        }

        public ConsoleColor GetAtColor()
        {
            return ConsoleColor.Yellow;
        }

        public ConsoleColor GetMarkColor()
        {
            return ConsoleColor.Red;
        }

        public ConsoleColor GetCmdColor()
        {
            return ConsoleColor.Blue;
        }
        public ConsoleColor GetH1Color()
        {
            return ConsoleColor.Yellow;
        }
        public ConsoleColor GetH2Color()
        {
            return ConsoleColor.Cyan;
        }

        public void FormatWordColor(List<string> commands, string word)
        {
            SetForegroundColor(GetWordColor(commands, word));
        }

        public void SetH1Color()
        {
            SetForegroundColor(GetH1Color());
        }

        public void SetH2Color()
        {
            SetForegroundColor(GetH2Color());
        }

        public void SetUserColor(string name)
        {
            SetForegroundColor(GetUserColor(name));
        }

        public async Task Update()
        {
            var inputTask = Task.Run(() => CheckForInput());

            while (!inputTask.IsCompleted)
            {
                await Core.FetchMessagesAsync();
                await Task.Delay(Core.GetRefreshDelay());
            }
        }

        public void ClearInput()
        {
            ClearLines(GetInputLines());
            Core.SetInput("");
        }

        public void DisplayInput()
        {
            Write($"{Core.getUserAlias()}: {Core.GetInput()}");
        }
    }
}