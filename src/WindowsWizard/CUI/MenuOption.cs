namespace WindowsWizard.CUI
{
    internal readonly struct MenuOption
    {
        internal static readonly MenuOption Blank;

        internal string OptionLabel { get; }

        internal Action OptionAction { get; }

        static MenuOption()
        {
            MenuOption.Blank = new MenuOption();
        }

        internal MenuOption(string optionLabel, Action optionAction)
        {
            this.OptionLabel = optionLabel;
            this.OptionAction = optionAction;
        }

        public static bool operator ==(MenuOption optionA, MenuOption optionB)
        {
            return string.Equals(optionA.OptionLabel, optionB.OptionLabel, StringComparison.Ordinal) && optionA.OptionAction == optionB.OptionAction;
        }

        public static bool operator !=(MenuOption optionA, MenuOption optionB)
        {
            return !(optionA == optionB);
        }

        public override readonly bool Equals(object? obj)
        {
            if (obj is not MenuOption)
            {
                return false;
            }

            MenuOption other = (MenuOption)obj;

            return string.Equals(this.OptionLabel, other.OptionLabel, StringComparison.Ordinal) && this.OptionAction == other.OptionAction;
        }

        public override int GetHashCode()
        {
            return (this.OptionLabel.GetHashCode() ^ this.OptionAction.GetHashCode());
        }
    }
}
