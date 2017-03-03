// -----------------------------------------------------------------------
// <copyright file="Graph.cs" company="None.">
//  By Philip R. Braica (HoshiKata@aol.com, VeryMadSci@gmail.com)
//
//  Distributed under the The Code Project Open License (CPOL)
//  http://www.codeproject.com/info/cpol10.aspx
// </copyright>
// -----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace GraphDemo
{
    /// <summary>
    /// Graph class.
    /// </summary>
    public partial class Graph : UserControl
    {
        /// <summary>
        /// Constructor.
        /// </summary>
        public Graph()
        {
            InitializeComponent();
            pictureBox1.Resize += new EventHandler(pictureBox1_Resize);
            ShowLegend = true;
            ShowAxis = true;
            ShowGrid = true;
        }

        /// <summary>
        /// Trigger redraw.
        /// </summary>
        public void TriggerRedraw()
        {
            timer1.Enabled = true;
        }

        /// <summary>
        /// On resize trigger redraw.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void pictureBox1_Resize(object sender, EventArgs e)
        {
            TriggerRedraw();
        }

        /// <summary>
        /// Timer to redraw
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void timer1_Tick(object sender, EventArgs e)
        {
        //    timer1.Enabled = false;
            doDraw();
        }

        /// <summary>
        /// Back buffer.
        /// </summary>
        protected System.Drawing.Bitmap m_back = null;
        /// <summary>
        /// Front buffer.
        /// </summary>
        protected System.Drawing.Bitmap m_front = null;

        /// <summary>
        /// Resize buffers as needed.
        /// </summary>
        protected void resizeBuffers()
        {
            int w = pictureBox1.Width;
            int h = pictureBox1.Height;
            w = w < 10 ? 10 : w;
            h = h < 10 ? 10 : h;
            System.Drawing.Bitmap old = m_back;
            if (old == null)
            {
                m_back = new Bitmap(w, h, System.Drawing.Imaging.PixelFormat.Format32bppArgb);
            }
            else
            {
                if ((m_back.Width != w) || (m_back.Height != h))
                {
                    m_back = new Bitmap(w, h, System.Drawing.Imaging.PixelFormat.Format32bppArgb);
                }
            }
            if (m_back != old)
            {
                if (old != null)
                {
                    old.Dispose();
                }
            }
        }

        /// <summary>
        /// Swap buffers.
        /// </summary>
        protected void swapBuffers()
        {
            System.Drawing.Bitmap tmp = m_front;
            m_front = m_back;
            m_back = tmp;
        }
        
        /// <summary>
        /// Do the drawing.
        /// </summary>
        protected void doDraw()
        {
            resizeBuffers();
            using (System.Drawing.Graphics g = System.Drawing.Graphics.FromImage(m_back))
            {
                paintGraph(g, m_back.Width, m_back.Height);
            }
            swapBuffers();
            pictureBox1.Image = m_front;
        }

        /// <summary>
        /// Paint the legend or not.
        /// </summary>
        public bool ShowLegend { get; set; }

        /// <summary>
        /// Paint the axis or not.
        /// </summary>
        public bool ShowAxis { get; set; }

        /// <summary>
        /// Paint the grid or not.
        /// </summary>
        public bool ShowGrid { get; set; }

        /// <summary>
        /// Paint.
        /// </summary>
        /// <param name="g"></param>
        /// <param name="w"></param>
        /// <param name="h"></param>
        protected void paintGraph(System.Drawing.Graphics g, int w, int h)
        {
            short ux = 0;
            short lx = 0;
            short uy = 0;
            short ly = 0;
            bool foundFirst = false;
            List<string> legend = new List<string>();
            List<Color> colors = new List<Color>();
            for (int i = 0; i < Lines.Count; i++)
            {
                if (Lines[i] == null) continue;
                if (Lines[i].Show == false) continue;
                if (!foundFirst)
                {
                    ux = Lines[i].MaxX;
                    lx = Lines[i].MinX;
                    uy = Lines[i].MaxY;
                    ly = Lines[i].MinY;
                    foundFirst = true;
                }
                legend.Add(Lines[i].Name);
                colors.Add(Lines[i].LineColor);
                ux = ux > Lines[i].MaxX ? ux : Lines[i].MaxX;
                lx = lx < Lines[i].MinX ? lx : Lines[i].MinX;
                uy = uy > Lines[i].MaxY ? uy : Lines[i].MaxY;
                ly = ly < Lines[i].MinY ? ly : Lines[i].MinY;
            }
            g.Clear(BackColor);//???

            Rectangle drawRectangle = new Rectangle(0, 0, w, h);
            if (ShowLegend)
            {
                int legendHeight = 5 + (int)g.MeasureString("AjtW", Font).Height;
                if (h - legendHeight > 10)
                {
                    drawRectangle = new Rectangle(0, 0, w, h - legendHeight);
                    Rectangle legendRectangle = new Rectangle(0, h - legendHeight + 2, w, legendHeight - 2);
                    drawLegend(g, legendRectangle, legend, colors);
                }
            }
            if (ShowAxis)
            {
                Font font = new Font("Arial", 9, FontStyle.Regular);
                LinearGradientBrush lgBrush = new LinearGradientBrush(drawRectangle, Color.Gray, Color.Gray, 1.2f, true);
                Pen darkPen = new Pen(Color.Gray, 2);
                Pen lightPen = new Pen(Color.Gray, 1);
                Pen axisPen = new Pen(Color.Black, 3);
                Pen buildPen = new Pen(lgBrush, 1);

                // 画纵线以及X轴坐标
                int leftX = 60; //最左边的纵线距离图像左边的距离
                g.DrawLine(axisPen, leftX, 50, leftX, drawRectangle.Height - 32); //Y轴
                g.DrawString("0", font, Brushes.Black, leftX, drawRectangle.Height - 24);
                for (int i = 1; i <= 640/16; i++)
                {
                    leftX += 16;
                    if (i%4 == 0)
                    {
                        g.DrawLine(darkPen, leftX, 50, leftX, drawRectangle.Height - 32);
                        g.DrawString((i*16).ToString(), font, Brushes.Black, leftX-10, drawRectangle.Height - 24);
                    }
                    else g.DrawLine(lightPen, leftX, 50, leftX, drawRectangle.Height - 32);
                }

                // 画横线及Y轴坐标
                int topY = 50; //最上边的横线距离图像顶部的距离
                string[] y = new string[] { "35000", "30000", "25000", "20000", "15000", "10000", "5000", "   0", "-5000" };
                for (int i = 0; i < 28; i++)
                {
                    g.DrawLine(buildPen, 60, topY, 720, topY);
                    if (i%4 == 0)
                    {
                        g.DrawLine(darkPen, 60, topY, 720, topY);
                        g.DrawString(y[i/4], font, Brushes.Black, 10, topY);
                    }
                    else g.DrawLine(lightPen, 60, topY, 720, topY);
                    topY += (drawRectangle.Height - 50 - 30) / 32;
                }
                g.DrawLine(axisPen, 20, topY, 720, topY); //X轴
                //g.DrawString("0", font, Brushes.Black, 10, topY);
                topY += (drawRectangle.Height - 50 - 30) / 32;
                for (int i = 29; i <33; i++)
                {
                    if (i%4 == 0)
                    {
                        g.DrawString(y[i/4], font, Brushes.Black, 10, topY);
                        g.DrawLine(darkPen, 60, topY, 720, topY);
                    }
                    else g.DrawLine(lightPen, 60, topY, 720, topY);
                    topY += (drawRectangle.Height - 50 - 30) / 32;
                }
                //g.DrawLine(buildPen, 60, topY, 760, topY); //最底下的横线
                //g.DrawString(y[8].ToString(), font, Brushes.Black, 10, topY);


            }
            if (!foundFirst) return;
            for (int i = 0; i < Lines.Count; i++)
            {
                if (Lines[i] == null) continue;
                if (Lines[i].Show == false) continue;
                Lines[i].Draw(g, drawRectangle, lx, ux, ly, uy);
            }
        }

        /// <summary>
        /// Draw legend.
        /// </summary>
        /// <param name="g"></param>
        /// <param name="r"></param>
        /// <param name="legend"></param>
        /// <param name="colors"></param>
        protected void drawLegend(Graphics g, Rectangle r, List<string> legend, List<Color> colors)
        {
            int cnt = legend.Count;
            int sx = r.X;
            for (int i = 0; i < legend.Count; i++)
            {
                int ex = r.X  + (r.Width * (i + 1) / cnt);
                using (SolidBrush sb = new SolidBrush(colors[i]))
                {
                    g.FillRectangle(sb, sx, r.Y, ex, r.Height);
                }
                using (SolidBrush sb2 = new SolidBrush(colors[i].GetBrightness() > 0.5? Color.Black : Color.White))
                {
                    SizeF sf = g.MeasureString(legend[i], Font);
                    PointF pf = new PointF(
                        (float)(0.5* (ex + sx - sf.Width)), 
                        (float)( r.Y + 0.5 * (sf.Height - sf.Height)));
                    g.DrawString(legend[i], Font, sb2, pf);
                }
                sx = ex;
            }
        }

        /// <summary>
        /// Text color for markers.
        /// </summary>
        public Color TextColor = Color.Black;

        /// <summary>
        /// Lines to draw.
        /// </summary>
        public List<Line> Lines = new List<Line>();

        /// <summary>
        /// Line class.
        /// </summary>
        public class Line
        {
            public Line(Color lineColor)
            {
                LineColor = lineColor;
            }

            /// <summary>
            /// Name.
            /// </summary>
            public string Name = string.Empty;
            
            /// <summary>
            /// Show it or not.
            /// </summary>
            public bool Show = true;

            /// <summary>
            /// X values if null, use index of Y.
            /// </summary>
            public short[] X = null;

            /// <summary>
            /// Y values.
            /// </summary>
            public short[] Y = null;

            /// <summary>
            /// Line color defaults to black.
            /// </summary>
            public Color LineColor ;

            /// <summary>
            /// Line width.
            /// </summary>
            public float LineWidth = 1f;

            /// <summary>
            /// Minimum X.
            /// </summary>
            public short MinX
            {
                get
                {
                    short[] xt = X;
                    if (xt == null) return 0;
                    if (xt.Length < 1) return 0;
                    short min = xt[0];
                    for (int i = 0; i < xt.Length; i++)
                    {
                        min = min < xt[i] ? min : xt[i];
                    }
                    return min;
                }
            }

            /// <summary>
            /// Maximum X.
            /// </summary>
            public short MaxX
            {
                get
                {
                    short[] xt = X;
                    if (xt == null)
                    {
                        short[] yt = Y;
                        if (yt == null) return 0;
                        return (short)yt.Length;
                    }
                    if (xt.Length < 1) return 0;
                    short max = xt[0];
                    for (int i = 0; i < xt.Length; i++)
                    {
                        max = max > xt[i] ? max : xt[i];
                    }
                    return max;
                }
            }

            /// <summary>
            /// Minimum Y.
            /// </summary>
            public short MinY
            {
                get
                {
                    short[] yt = Y;
                    if (yt == null) return 0;
                    if (yt.Length < 1) return 0;
                    short min = yt[0];
                    for (int i = 0; i < yt.Length; i++)
                    {
                        min = min < yt[i] ? min : yt[i];
                    }
                    return min;
                }
            }

            /// <summary>
            /// Maximum Y.
            /// </summary>
            public short MaxY
            {
                get
                {
                    short[] yt = Y;
                    if (yt == null) return 0;
                    if (yt.Length < 1) return 0;
                    short max = yt[0];
                    for (int i = 0; i < yt.Length; i++)
                    {
                        max = max > yt[i] ? max : yt[i];
                    }
                    return max;                
                }
            }

            ///// <summary>
            ///// Update
            ///// </summary>
            ///// <param name="y">Y value or null to use index.</param>
            //public void Update(short[] y)
            //{
            //    Update(null, y);
            //}

            /// <summary>
            /// Update.
            /// </summary>
            /// <param name="x">X values or null to use index.</param>
            /// <param name="y">Y value or null to use index.</param>
            //public void Update(double[] x, double[] y)
            //{
            //    // Thread and null safe.
            //    if (x != null)
            //    {
            //        bool resize = X == null ? true : X.Length == x.Length ? false : true;
            //        double[] xn = resize ? new double[x.Length] : X;
            //        for (int i = 0; i < x.Length; i++)
            //        {
            //            xn[i] = x[i];
            //        }
            //        X = xn;
            //    }
            //    else
            //    {
            //        x = null;
            //    }
            //    if (y != null)
            //    {
            //        bool resize = Y == null ? true : Y.Length == y.Length ? false : true;
            //        double[] yn = resize ? new double[y.Length] : Y;
            //        for (int i = 0; i < y.Length; i++)
            //        {
            //            yn[i] = y[i];
            //        }
            //        Y = yn;
            //    }
            //    else
            //    {
            //        Y = null;
            //    }                
            //}

            /// <summary>
            /// Update
            /// </summary>
            /// <param name="y">Y value or null to use index.</param>
            public void Update(short[] y)
            {
                Update(null, y);
            }

            /// <summary>
            /// Update
            /// </summary>
            /// <param name="x">X values or null to use index.</param>
            /// <param name="y">Y value or null to use index.</param>
            public void Update(short[] x, short[] y)
            {
                // Thread and null safe.
                if (x != null)
                {
                    bool resize = X == null ? true : X.Length == x.Length ? false : true;
                    short[] xn = resize ? new short[x.Length] : X;
                    for (int i = 0; i < x.Length; i++)
                    {
                        xn[i] = x[i];
                    }
                    X = xn;
                }
                else
                {
                    x = null;
                }
                if (y != null)
                {
                    bool resize = Y == null ? true : Y.Length == y.Length ? false : true;
                    short[] yn = resize ? new short[y.Length] : Y;
                    for (int i = 0; i < y.Length; i++)
                    {
                        yn[i] = y[i];
                    }
                    Y = yn;
                }
                else
                {
                    Y = null;
                }
            }

            /// <summary>
            /// Draw within r on g, assuming the lower / upper X values are lx, ux, and ly, uy for Y.
            /// </summary>
            /// <param name="g"></param>
            /// <param name="r"></param>
            /// <param name="lx"></param>
            /// <param name="ux"></param>
            /// <param name="ly"></param>
            /// <param name="uy"></param>
            public void Draw(Graphics g, Rectangle r, short lx, short ux, short ly, short uy)
            {
                short[] x = X;
                short[] y = Y;
                if (y == null) return;
                int n = x == null ? y.Length : y.Length > x.Length ? x.Length : y.Length;
                ux = ux == lx ? (short)(lx + 1) : ux;
                uy = uy == ly ? (short)(ly + 1): uy;
                double xscale = (double)r.Width / (double)(ux - lx);
                double yscale =0.015;//高600个像素点，纵坐标从-5000~35000，分8段
                using (Pen p = new Pen(LineColor, LineWidth))
                {
                    List<PointF> points = new List<PointF>();
                    for (int i = 0; i < n; i++)
                    {
                        double xi = x == null ? i : x[i];
                        double yi = y[i];
                        xi = r.X + ((xi - lx) * xscale);
                        yi=r.Y + r.Height - ((yi +5000) * yscale);
                        PointF point = new PointF(i+60, (float)yi);
                        points.Add(point);
                    }
                    g.DrawLines(p, points.ToArray());
                }
            }
        }
    }
}
