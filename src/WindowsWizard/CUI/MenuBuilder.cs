using System.Linq;
using System.Collections.Generic;

namespace WindowsWizard.CUI
{
    internal sealed class MenuBuilder
    {
        private string title;
        private readonly LinkedList<MenuOption> items;

        internal MenuBuilder()
        {
            this.title = String.Empty;
            this.items = new LinkedList<MenuOption>();
        }

        internal MenuBuilder AddItem(string optionLabel, Action optionAction)
        {
            this.items.AddLast(new MenuOption(optionLabel, optionAction));
            return this;
        }

        internal MenuBuilder AddItem(MenuOption optionItem)
        {
            this.items.AddLast(optionItem);
            return this;
        }

        internal MenuBuilder AddTitle(string title)
        {
            this.title = title;
            return this;
        }

        internal Menu Build()
        {
            Menu menu = new Menu(this.title, this.items.ToArray());
            this.items.Clear();
            return menu;
        }
    }
}
