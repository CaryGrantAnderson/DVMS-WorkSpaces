using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace LnkFileExplorer
{
    public partial class About : Form
    {
        public About()
        {
            InitializeComponent();
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            //  Do nothing here!
        }

        protected override void OnPaintBackground(PaintEventArgs e)
        {
            Graphics gfx = e.Graphics;
            gfx.DrawImage(Images.SplashScreen, new Rectangle(0, 0, Width, Height));
        }

        private void About_MouseClick(object sender, MouseEventArgs e)
        {
            Visible = false;
        }

    }
}
