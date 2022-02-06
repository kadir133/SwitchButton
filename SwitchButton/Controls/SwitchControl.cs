using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms.Design;

namespace System.Windows.Forms
{
    public enum SwitchState { On, Off }

    [Designer(typeof(SwitchControlDesigner)), DefaultEvent("SwitchStateChanged")]
    public class SwitchControl : Control
    {
        public event SliderChangeEventHandler SwitchStateChanged;
        protected override Size DefaultSize { get { return new Size(35, 18); } }

        public SwitchControl()
        {
            DoubleBuffered = true;
            base.Cursor = Cursors.Hand;

            CreateSwitch();

            timerOfCircle.Tick += TimerOfCircle_Tick;
            timerOfCircle.Interval = 1;
        }

        RoundedRectangleF SwitchRectangle;
        RectangleF circle;
        private SwitchState switchState = SwitchState.Off;
        private Color backgroundOnColor = SystemColors.MenuHighlight;
        private Color backgroundOfColor = SystemColors.Control;
        private Color circleOnColor = Color.White;
        private Color circleOfColor = Color.Black;
        private Color borderColor = Color.Black;
        private string onText = "On";
        private string offText = "Off";
        private bool showText = false;
        private bool fontAutoSize = true;
        readonly Timer timerOfCircle = new Timer();
        readonly Padding rectanglePadding = new Padding(2);
        readonly float BorderWidth = 1.2f;
        bool userClicked = false;

        internal virtual Size SwitchSize { get { return Size; } }
        internal PointF SwitchStartPosition = new PointF(0, 0);

        [Category("(CustomControl)"), DefaultValue("On")]
        public string OnStateText { get { return onText; } set { onText = value; Invalidate(); } }

        [Category("(CustomControl)"), DefaultValue("Off")]
        public string OffStateText { get { return offText; } set { offText = value; Invalidate(); } }

        [Category("(CustomControl)"), DefaultValue(false)]
        public virtual bool ShowText { get { return showText; } set { showText = value; Invalidate(); } }

        [Category("(CustomControl)"), DefaultValue(true)]
        public virtual bool FontAutoSize { get { return fontAutoSize; } set { fontAutoSize = value; Invalidate(); } }

        [Category("(CustomControl)")]
        public SizeF CircleSize { get { return circle.Size; } }

        //increment for sliding animation
        [Category("(CustomControl)"), DefaultValue(3f)]
        public float Increment { get; set; } = 3f;

        [Category("(CustomControl)"), DefaultValue(false)]
        public bool IsOn
        {
            get
            {
                return switchState != SwitchState.Off;
            }
            set
            {
                if (value != IsOn)
                {
                    SwitchState = (value ? SwitchState.On : SwitchState.Off);
                }
            }
        }

        [Category("(CustomControl)"), DefaultValue(SwitchState.Off)]
        public SwitchState SwitchState
        {
            get { return switchState; }
            set
            {
                if (switchState != value)
                {
                    if (userClicked)
                    {
                        userClicked = false;
                        var args = new SwitchStateChangeEventArgs(value);
                        SwitchStateChanged?.Invoke(this, args);
                        if (args.Cancel)
                            return;
                    }

                    timerOfCircle.Stop();
                    switchState = value;
                    timerOfCircle.Start();
                    Application.DoEvents();
                }
            }
        }

        [Category("(CustomControl)")]
        public Color BackgroundOnColor
        {
            get { return backgroundOnColor; }
            set
            {
                backgroundOnColor = value;
                Invalidate();
            }
        }
        [Category("(CustomControl)")]
        public Color BackgroundOfColor
        {
            get { return backgroundOfColor; }
            set
            {
                backgroundOfColor = value;
                Invalidate();
            }
        }
        [Category("(CustomControl)")]
        public Color CircleOnColor
        {
            get { return circleOnColor; }
            set
            {
                circleOnColor = value;
                Invalidate();
            }
        }
        [Category("(CustomControl)")]
        public Color CircleOfColor
        {
            get { return circleOfColor; }
            set
            {
                circleOfColor = value;
                Invalidate();
            }
        }
        [Category("(CustomControl)")]
        public Color BorderColor
        {
            get { return borderColor; }
            set
            {
                borderColor = value;
                Invalidate();
            }
        }

        internal void CreateSwitch()
        {
            SwitchRectangle = new RoundedRectangleF(SwitchSize.Width, SwitchSize.Height, SwitchSize.Height / 2, BorderWidth, SwitchStartPosition);
            circle = GetCircle(IsOn ? GetCircleLeftAtOnState() : GetCircleLeftAtOffState());
        }

        RectangleF GetCircle(float x)
        {
            return new RectangleF(x, SwitchStartPosition.Y + rectanglePadding.Top + BorderWidth, GetDiameter(), GetDiameter());
        }

        float GetDiameter()
        {
            return SwitchSize.Height - rectanglePadding.Vertical - (BorderWidth * 3);
        }

        float GetCircleLeftAtOnState()//returns left point
        {
            return SwitchStartPosition.X + SwitchSize.Width - rectanglePadding.Right - (BorderWidth * 2) - GetDiameter();
        }

        float GetCircleLeftAtOffState()//returns left point
        {
            return SwitchStartPosition.X + rectanglePadding.Left + BorderWidth;
        }

        private void CheckSize()
        {
            var minWidth = circle.Size.Width + rectanglePadding.Horizontal + (BorderWidth * 3);
            if (Size.Width < minWidth)
                Size = new Size((int)minWidth, Size.Height);
        }

        private Font FindBestFitFont(Graphics g, string text, Font font, SizeF proposedSize)
        {
            int best_size = 72;

            // See how much room we have, allowing a bit
            float wid = proposedSize.Width - 3;
            float hgt = proposedSize.Height - 3;

            // Make a Graphics object to measure the text.
            for (int i = 1; i <= 72; i++)
            {
                using (Font test_font = new Font(font.FontFamily, i))
                {
                    // See how much space the text would
                    // need, specifying a maximum width.
                    SizeF text_size = g.MeasureString(text, test_font);
                    if ((text_size.Width > wid) || (text_size.Height > hgt))
                    {
                        best_size = i - 1;
                        break;
                    }
                }
            }

            if (best_size <= 0)
                best_size = 1;

            // Use that font size.
            return new Font(font.Name, best_size, font.Style, font.Unit, font.GdiCharSet, font.GdiVerticalFont);
        }

        //creates slide animation
        void TimerOfCircle_Tick(object sender, EventArgs e)
        {
            float x = circle.X;
            if (IsOn)//switch the circle to the left
            {
                if (x + Increment < GetCircleLeftAtOnState())
                {
                    x += Increment;
                    circle = GetCircle(x);

                    Invalidate();
                }
                else
                {
                    x = GetCircleLeftAtOnState();
                    circle = GetCircle(x);

                    Invalidate();
                    timerOfCircle.Stop();
                }
            }
            else //switch the circle to the left with animation
            {
                if (x - Increment > GetCircleLeftAtOffState())
                {
                    x -= Increment;
                    circle = GetCircle(x);

                    Invalidate();
                }
                else
                {
                    x = GetCircleLeftAtOffState();
                    circle = GetCircle(x);

                    Invalidate();
                    timerOfCircle.Stop();
                }
            }
        }

        internal void PaintBase(PaintEventArgs e)
        {
            base.OnPaint(e);
        }

        internal void PaintSwitch(PaintEventArgs e)
        {
            e.Graphics.SmoothingMode = SmoothingMode.HighQuality;

            var backColor = Enabled ? (IsOn ? backgroundOnColor : backgroundOfColor) : ColorTranslator.FromHtml("#CFCFCF");
            var circleClr = Enabled ? (IsOn ? circleOnColor : circleOfColor) : ColorTranslator.FromHtml("#B3B3B3");
            var circleBrdr = Enabled ? circleClr : Color.DarkGray;

            using (SolidBrush brush = new SolidBrush(backColor))
                e.Graphics.FillPath(brush, SwitchRectangle.Path);
            using (Pen pen = new Pen(borderColor, BorderWidth))
                e.Graphics.DrawPath(pen, SwitchRectangle.Path);

            if (showText)
            {
                var textColor = Enabled ? ForeColor : Color.DarkGray;
                var writableWidth = Width - circle.Width - rectanglePadding.Horizontal - (BorderWidth * 4);
                var text = IsOn ? OnStateText : OffStateText;

                if (FontAutoSize)
                    Font = FindBestFitFont(e.Graphics, text, Font, new SizeF(writableWidth, circle.Height));

                int height = TextRenderer.MeasureText(text, Font).Height;
                float textTop = (SwitchSize.Height - height) / 2f;
                var textLeft = IsOn ? rectanglePadding.Left : GetDiameter() + (BorderWidth * 2) + rectanglePadding.Left;
                e.Graphics.DrawString(text, Font, new SolidBrush(textColor), textLeft, textTop);
            }

            using (SolidBrush circleBrush = new SolidBrush(circleClr))
                e.Graphics.FillEllipse(circleBrush, circle);
            using (Pen circlePen = new Pen(circleBrdr, BorderWidth))
                e.Graphics.DrawEllipse(circlePen, circle);
        }

        #region Overrides of base objects

        [Browsable(false)]
        public override Color BackColor => Parent?.BackColor ?? SystemColors.Control;

        [Browsable(false)]
        public override Cursor Cursor { get { return Cursors.Hand; } }

        protected override void OnPaint(PaintEventArgs e)
        {
            PaintSwitch(e);
            PaintBase(e);
        }

        protected override void OnMouseClick(MouseEventArgs e)
        {
            if (e.Button != MouseButtons.Left)
                return;
            userClicked = true;
            IsOn = !IsOn;
            base.OnMouseClick(e);
        }

        protected override void OnEnabledChanged(EventArgs e)
        {
            Invalidate();
            base.OnEnabledChanged(e);
        }

        protected override void OnSizeChanged(EventArgs e)
        {
            Invalidate();
            CreateSwitch();
            base.OnSizeChanged(e);
            CheckSize();
        }
        #endregion

        #region Serialization of some properties
        public bool ShouldSerializeText()
        {
            return !Text.Equals(string.Empty);
        }
        public bool ShouldSerializeCursor()
        {
            return !Cursor.Equals(Cursors.Hand);
        }
        public bool ShouldSerializeBackgroundOnColor()
        {
            return !backgroundOnColor.Equals(SystemColors.MenuHighlight);
        }
        public bool ShouldSerializeBackgroundOfColor()
        {
            return !backgroundOfColor.Equals(SystemColors.Control);
        }
        public bool ShouldSerializeCircleOnColor()
        {
            return !circleOnColor.Equals(Color.White);
        }
        public bool ShouldSerializeCircleOfColor()
        {
            return !circleOfColor.Equals(Color.Black);
        }
        public bool ShouldSerializeBorderColor()
        {
            return !borderColor.Equals(Color.Black);
        }
        #endregion
    }
}