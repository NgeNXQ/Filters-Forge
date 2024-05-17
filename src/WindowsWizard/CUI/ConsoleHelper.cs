#if !WINDOWS
#error Platform is not supported!
#warning Please ensure that you are compiling and running this solution in the Windows environment.
#endif

using System.IO;
using System.Text;

namespace WindowsWizard.CUI
{
    internal static class ConsoleHelper
    {
        private const int EXIT_SUCCESS = 0;

        private static string asciiArt;
        private static int cursorOffsetTop;

        internal static string InputInvitationSeparator { get; set; }

        internal static bool ShouldBeTerminated { private set; get; }

        static ConsoleHelper()
        {
            Console.OutputEncoding = Encoding.UTF8;

            ConsoleHelper.asciiArt = String.Empty;
            ConsoleHelper.ShouldBeTerminated = false;
            ConsoleHelper.InputInvitationSeparator = ">>";
        }

        internal static void Start()
        {
            if (!string.IsNullOrEmpty(ConsoleHelper.asciiArt))
            {
                Console.WriteLine($"{ConsoleHelper.asciiArt}\n");
                ConsoleHelper.cursorOffsetTop = Console.CursorTop;
            }
        }

        internal static void Shutdown()
        {
            ConsoleHelper.ShouldBeTerminated = true;
            Environment.Exit(ConsoleHelper.EXIT_SUCCESS);
        }

        internal static string ReadInput()
        {
            Console.Write($"{ConsoleHelper.InputInvitationSeparator} ");
            return Console.ReadLine() ?? string.Empty;
        }

        internal static void ClearCurrentLine()
        {
            int currentCursorPosition = Console.CursorTop - 1;

            Console.SetCursorPosition(0, currentCursorPosition);
            Console.Write(new string(' ', Console.WindowWidth));
            Console.SetCursorPosition(0, currentCursorPosition);
        }

        internal static void ClearOutputBuffer()
        {
            int currentCursorPosition = Console.CursorTop;

            for (int i = ConsoleHelper.cursorOffsetTop; i < currentCursorPosition; ++i)
            {
                Console.SetCursorPosition(0, i);
                Console.Write(new string(' ', Console.WindowWidth));
            }

            Console.SetCursorPosition(0, ConsoleHelper.cursorOffsetTop);
        }

        internal static void ShowMenu(Menu menu)
        {
            if (menu == null)
            {
                throw new ArgumentNullException($"{nameof(menu)} is null.");
            }

            ConsoleHelper.ClearOutputBuffer();

            Console.Title = menu.Title;
            Console.Write(menu);
        }

        internal static void LoadAsciiArt(string relativePathAsciiArt)
        {
            if (!string.IsNullOrEmpty(relativePathAsciiArt))
            {
                if (File.Exists(relativePathAsciiArt))
                {
                    ConsoleHelper.asciiArt = File.ReadAllText(relativePathAsciiArt);
                }
                else
                {
                    throw new ArgumentException($"{nameof(relativePathAsciiArt)} {relativePathAsciiArt} does not exist.");
                }
            }
        }

        internal static MenuOption ReadMenuInput(Menu menu, Action invalidInputHandler)
        {
            string input;
            MenuOption option;

            do
            {
                input = ConsoleHelper.ReadInput();
                option = menu.ProcessInput(input);

                if (option == MenuOption.Blank)
                {
                    invalidInputHandler?.Invoke();
                }
            } while (option == MenuOption.Blank);

            return option;
        }
    }
}
