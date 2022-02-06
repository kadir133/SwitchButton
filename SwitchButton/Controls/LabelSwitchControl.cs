using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms.Design;

namespace System.Windows.Forms
{
    [Designer(typeof(LabelSwitchControlDesigner))]
    public class LabelSwitchControl : SwitchControl, ISupportInitialize
    {
        private bool initialized = false;
        protected override Size DefaultSize => new Size(175, 20);
        internal override Size SwitchSize => base.DefaultSize;

        [Category("(CustomControl)")]
        public new string Text { get { return base.Text; } set { base.Text = value; CalculateHeight(); Invalidate(); } }

        [Category("(CustomControl)"), DefaultValue(false)]
        public bool WrapText { get { return wrap; } set { wrap = value; CalculateHeight(); Invalidate(); } }
        private bool wrap;

        public LabelSwitchControl()
        {
            DoubleBuffered = true;
            OnSizeChange();
        }

        void OnSizeChange()
        {
            SwitchStartPosition.X = (Width - SwitchSize.Width) - 2;//sağdan 3 padding vermek için
            SwitchStartPosition.Y = (Height - SwitchSize.Height) / 2;
            CreateSwitch();
            CalculateHeight();
        }

        void CalculateHeight()
        {
            if (initialized == false) return;
            Size textSize = TextRenderer.MeasureText(Text, Font);
            double lineCount = 1;
            if (wrap)
            {
                var writableWidth = SwitchStartPosition.X - 5;
                lineCount = Math.Ceiling(textSize.Width / writableWidth);
            }
            Height = (int)lineCount * textSize.Height + 7;
        }

        protected override void OnEnabledChanged(EventArgs e)
        {
            Invalidate();
            base.OnEnabledChanged(e);
        }

        protected override void OnSizeChanged(EventArgs e)
        {
            base.OnSizeChanged(e);
            OnSizeChange();
        }

        protected override void OnFontChanged(EventArgs e)
        {
            base.OnFontChanged(e);
            CalculateHeight();
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            if (wrap)
            {
                var writableWidth = SwitchStartPosition.X - 3;
                var stringRec = new RectangleF(3, 3, writableWidth, Height - 6);
                StringFormat format = new StringFormat { LineAlignment = StringAlignment.Center };
                e.Graphics.DrawString(Text, Font, new SolidBrush(ForeColor), stringRec, format);
            }
            else
            {
                e.Graphics.DrawString(Text, Font, new SolidBrush(ForeColor), 3, 2.5f);
            }

            PaintSwitch(e);
            PaintBase(e);
        }

        public void BeginInit()
        {
            initialized = false;
        }

        public void EndInit()
        {
            initialized = true;
            CalculateHeight();
        }

        [Browsable(false), DefaultValue(null)]
        public new string OnStateText { get; }
        [Browsable(false), DefaultValue(null)]
        public new string OffStateText { get; }
        [Browsable(false), DefaultValue(false)]
        public override bool ShowText => false;
        [Browsable(false), DefaultValue(false)]
        public override bool FontAutoSize => false;
    }
}