using System.Drawing;

namespace FiltersForge.Filters.Common.Base
{
    internal interface IFilter
    {
        public Bitmap Apply(Bitmap image);
    }
}