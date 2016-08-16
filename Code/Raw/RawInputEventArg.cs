using System;

namespace Multiple_Mice.Code.Raw
{
    public class RawInputEventArg : EventArgs
    {
        public RawInputEventArg(MouseEvent arg)
        {
            MouseEvent = arg;
        }
        
        public MouseEvent MouseEvent { get; private set; }
    }
}
