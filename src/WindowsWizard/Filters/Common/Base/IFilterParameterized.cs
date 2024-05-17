using System.Drawing;

namespace WindowsWizard.Filters.Common.Base
{
    internal interface IFilterParameterized<TPreferences> where TPreferences : struct
    {
        public Bitmap Apply(Bitmap image, in TPreferences preferences);
    }
}
