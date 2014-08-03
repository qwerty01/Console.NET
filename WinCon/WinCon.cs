using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WinConsole
{
    public partial class WinCon : Form
    {
        [StructLayout(LayoutKind.Sequential)]
        protected struct RECT
        {
            public int Left;
            public int Top;
            public int Right;
            public int Bottom;
        }
        protected const int WM_SIZING = 0x0214;

        public int CharWidth { get; protected set; }
        public int CharHeight { get; protected set; }
        public int ScreenWidth { get; set; }
        public int ScreenHeight { get; set; }
        public WinCon()
        {
            InitializeComponent();
            Width = ScreenWidth * CharWidth;
            Height = ScreenHeight * CharHeight;
            CharWidth = 8;
            CharHeight = 12;
            ScreenWidth = 80;
            ScreenHeight = 25;
        }

        protected override void WndProc(ref Message m)
        {
            if (m.Msg == WM_SIZING)
            {
                RECT r;
                r = (RECT)Marshal.PtrToStructure(m.LParam, typeof(RECT));
                r.Bottom = (r.Bottom - r.Top) / CharHeight * CharHeight + r.Top;
                r.Right = (r.Right - r.Left) / CharWidth * CharWidth + r.Left;
                ScreenHeight = (r.Bottom - r.Top) / CharHeight;
                ScreenWidth = (r.Right - r.Left) / CharWidth;
                Marshal.StructureToPtr(r, m.LParam, true);
            }
            base.WndProc(ref m);
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            Graphics g = e.Graphics;
            Pen p = new Pen(Brushes.White);
            for (int i = 1; i < (ScreenHeight > ScreenWidth ? ScreenHeight : ScreenWidth); i += 1)
            {
                if( i < ScreenWidth)
                    g.DrawLine(p, new Point(i * CharWidth - 1, 0), new Point(i * CharWidth - 1, Height));
                if (i < ScreenHeight)
                    g.DrawLine(p, new Point(0, i * CharHeight - 1), new Point(Width, i * CharHeight - 1));
            }
        }

        protected override void OnPaintBackground(PaintEventArgs e)
        {
            base.OnPaintBackground(e);
        }
    }
}
