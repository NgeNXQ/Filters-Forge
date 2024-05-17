using WindowsWizard.Filters.Common.Validation;

namespace WindowsWizard.Filters
{
    internal struct SequentialPSXPreferences
    {
        [Range<int>(2, Int32.MaxValue, "Введіть значення з коректного діапазону значень [{0}; {1}].")]
        internal int BlockSize { get; set; }
    }
}
