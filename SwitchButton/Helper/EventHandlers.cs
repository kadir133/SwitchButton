using System.ComponentModel;
using System.Windows.Forms;

namespace System
{
    public delegate void SliderChangeEventHandler(object sender, SwitchStateChangeEventArgs e);
}

namespace System.ComponentModel
{
    public class SwitchStateChangeEventArgs : CancelEventArgs
    {
        public bool NewValue => NewSwitchState == SwitchState.On;
        public SwitchState NewSwitchState { get; set; }
        public SwitchStateChangeEventArgs(SwitchState newSwitchState)
        {
            NewSwitchState = newSwitchState;
        }
    }
}
