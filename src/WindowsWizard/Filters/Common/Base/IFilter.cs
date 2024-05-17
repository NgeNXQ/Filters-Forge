using System.Drawing;

namespace WindowsWizard.Filters.Common.Base
{
    internal interface IFilter
    {
        public Bitmap Apply(Bitmap image);
    }
}