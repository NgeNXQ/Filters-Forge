using System.Text;
using System.Collections.Generic;

namespace WindowsWizard.CUI
{
    internal sealed class Menu
    {
        private static readonly StringBuilder stringBuilder;

        internal static char OptionSeparator { get; set; }

        private readonly string formattedMenu;
        private readonly Dictionary<int, MenuOption> options;

        public string Title { get; }

        static Menu()
        {
            Menu.OptionSeparator = '.';
            Menu.stringBuilder = new StringBuilder();
        }

        internal Menu(string title, IEnumerable<MenuOption> options)
        {
            this.Title = title;

            Menu.stringBuilder.Clear();
            this.options = new Dictionary<int, MenuOption>();

            int optionsCount = 0;

            foreach (MenuOption item in options)
            {
                ++optionsCount;
                this.options.Add(optionsCount, item);
                Menu.stringBuilder.Append($"{optionsCount}{Menu.OptionSeparator} {item.OptionLabel}\n");
            }

            Menu.stringBuilder.Append('\n');

            this.formattedMenu = Menu.stringBuilder.ToString();
        }

        internal MenuOption this[int key]
        {
            get
            {
                if (this.options.TryGetValue(key, out MenuOption menuItem))
                {
                    return menuItem;
                }
                else
                {
                    throw new ArgumentException($"{nameof(key)} {key} does not exist.");
                }
            }
        }

        internal MenuOption ProcessInput(string input)
        {
            if (int.TryParse(input, out int key))
            {
                if (this.options.TryGetValue(key, out MenuOption menuItem))
                {
                    Console.Title = menuItem.OptionLabel;
                    return menuItem;
                }
            }

            return MenuOption.Blank;
        }

        public override string ToString()
        {
            return this.formattedMenu;
        }
    }
}
