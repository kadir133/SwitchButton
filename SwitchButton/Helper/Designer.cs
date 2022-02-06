namespace System.Windows.Forms.Design
{
    internal class SwitchControlDesigner : ControlDesigner
    {
        public override SelectionRules SelectionRules { get { return SelectionRules.AllSizeable | SelectionRules.Visible | SelectionRules.Moveable; } }
    }

    internal class LabelSwitchControlDesigner : ControlDesigner
    {
        public override SelectionRules SelectionRules { get { return SelectionRules.LeftSizeable | SelectionRules.RightSizeable | SelectionRules.Visible | SelectionRules.Moveable; } }
    }
}