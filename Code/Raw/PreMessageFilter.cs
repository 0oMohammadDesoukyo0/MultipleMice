using System.Windows.Forms;

namespace Multiple_Mice.Code.Raw
{
    public class PreMessageFilter : IMessageFilter
    {
        public bool PreFilterMessage(ref Message m)
        {
            return m.Msg == Win32.WM_MOUSEMOVE;
        }
    }
}
