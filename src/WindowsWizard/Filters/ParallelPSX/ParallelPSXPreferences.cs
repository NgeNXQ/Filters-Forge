using WindowsWizard.Filters.Common.Validation;

namespace WindowsWizard.Filters
{
    internal struct ParallelPSXPreferences
    {
        [Range<int>(2, int.MaxValue, "Введіть значення з коректного діапазону значень [{0}; {1}].")]
        internal int BlockSize { get; set; }

        [Range<int>(1, int.MaxValue, "Введіть значення з коректного діапазону значень [{0}; {1}].")]
        internal int ThreadsCount { get; set; }
    }
}
