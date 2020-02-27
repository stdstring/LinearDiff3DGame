using System;
using System.Drawing;
using System.Windows.Forms;

namespace LinearDiff3DGame.OpenGLVisualizer.ColorChoose
{
    public partial class RGBPropertyBox : UserControl
    {
        public RGBPropertyBox()
        {
            InitializeComponent();
        }

        public String Header { get { return lblHeader.Text; } set { lblHeader.Text = value; } }
        public Color Color { get { return TryGetRGB(); } set { SetRGB(value); } }

        private void SetRGB(Color color)
        {
            tbRComponent.Text = color.R.ToString();
            tbGComponent.Text = color.G.ToString();
            tbBComponent.Text = color.B.ToString();
        }

        private Color TryGetRGB()
        {
            Int32 r = TryParseColorComponent(tbRComponent),
                  g = TryParseColorComponent(tbGComponent),
                  b = TryParseColorComponent(tbBComponent);
            Color color = r != -1 && g != -1 && b != -1 ? Color.FromArgb(255, r, g, b) : Color.Empty;
            return color;
        }

        private Int32 TryParseColorComponent(Control colorComponentBox)
        {
            Int32 colorComponent;
            if(!Int32.TryParse(colorComponentBox.Text, out colorComponent))
            {
                errorProvider1.SetError(colorComponentBox, "Value is not integer between 0 and 255.");
                return -1;
            }
            if(colorComponent < 0 || colorComponent > 255)
            {
                errorProvider1.SetError(colorComponentBox, "Value is not integer between 0 and 255.");
                return -1;
            }
            return colorComponent;
        }
    }
}