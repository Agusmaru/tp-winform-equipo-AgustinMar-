using System.ComponentModel;
using System.Windows.Forms;

namespace TPWinFormApp_AgustinMaru
{
    internal static class DesignHelper
    {
        public static bool EnModoDiseno(Control control)
        {
            if (LicenseManager.UsageMode == LicenseUsageMode.Designtime)
                return true;

            while (control != null)
            {
                if (control.Site != null && control.Site.DesignMode)
                    return true;

                control = control.Parent;
            }

            return false;
        }
    }
}
