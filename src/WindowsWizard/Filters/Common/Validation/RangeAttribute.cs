namespace FiltersForge.Filters.Common.Validation
{
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = false)]
    internal sealed class RangeAttribute<T> : Attribute where T : struct, IComparable, IComparable<T>, IEquatable<T>
    {
        private readonly T minimum;
        private readonly T maximum;
        private readonly string errorMessage;

        internal string ErrorMessage { get => string.Format(this.errorMessage, this.minimum, this.maximum); }

        internal RangeAttribute(T minimum, T maximum, string errorMessage)
        {
            if (minimum.CompareTo(maximum) > 0)
            {
                throw new ArgumentException($"{nameof(minimum)} must be less than {nameof(maximum)}.");
            }

            this.minimum = minimum;
            this.maximum = maximum;
            this.errorMessage = errorMessage;
        }

        internal bool IsValid(T value)
        {
            return value.CompareTo(this.minimum) >= 0 && value.CompareTo(this.maximum) <= 0;
        }
    }
}
