using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CGA
{
    public partial class Form1 : Form
    {

        private VectorTransform transform = new VectorTransform();

        private Vector4 cameraPos = new Vector4(0, 0.25f, 1f, 1f);
        private Vector4 cameraTarget = new Vector4(0, 0.25f, 0, 1f);
        private Vector4 cameraUp = new Vector4(0, 1, 0, 1f);

        private Vector4 xAxis = new Vector4(1, 0, 0, 1f);
        private Vector4 yAxis = new Vector4(0, 1, 0, 1f);
        private Vector4 zAxis = new Vector4(0, 0, 1, 1f);

        float alpha = 0;
        float beta = 0;

        Bitmap bitmap;
        ObjReader model = new ObjReader("untitled.obj");
        Rectangle rect;
        public Form1()
        {
            InitializeComponent();

            bitmap = new Bitmap(this.Width + 1, this.Height + 1);

            pictureBox1.Image = bitmap;
            rect = new Rectangle(0, 0, bitmap.Width, bitmap.Height);
            drawModel();
        }
        ObjReader objReader;
        private void transformVertex(int i, ParallelLoopState state)
        {
            objReader.vertices[i] = transform.LocalToWorld(objReader.vertices[i], xAxis, yAxis, zAxis, new Vector4(0, 0, 0, 1f));
            objReader.vertices[i] = transform.WorldToView(objReader.vertices[i], cameraPos, cameraTarget, cameraUp);
            objReader.vertices[i] = transform.ViewToClip(objReader.vertices[i], 1.778f, 1, 0, 100, ProjectionMode.Pespective);

        }
        private void drawPoly(int i, ParallelLoopState state)
        {
            Polygon polygon = objReader.polygons[i];
            for (int j = 0; j < polygon.length; j++)
            {
                Vector4 p1 = objReader.vertices[polygon.ind[j]];
                Vector4 p2 = objReader.vertices[polygon.ind[(j + 1) % polygon.length]];

                if (p1.X > 1 || p1.X < -1 || p1.Y > 1 || p1.Y < -1 || p1.Z > 1 || p1.Z < 0 ||
                    p2.X > 1 || p2.X < -1 || p2.Y > 1 || p2.Y < -1 || p2.Z > 1 || p2.Z < 0) continue;

                p1 = transform.ClipToScreen(p1, this.Width, this.Height, 0, 0);
                p2 = transform.ClipToScreen(p2, this.Width, this.Height, 0, 0);

                foreach ((int, int) point in rasterization.Rasterize(p1.X, p1.Y, p2.X, p2.Y))
                {
                    int pixel = point.Item2 * bmpData.Stride + point.Item1 * 4;
                    rgbValues[pixel] = 255;
                    rgbValues[pixel + 1] = 255;
                    rgbValues[pixel + 2] = 255;
                    rgbValues[pixel + 3] = 255;
                }
            }
        }
        byte[] rgbValues;
        IRasterization rasterization = new Bresenham();
        System.Drawing.Imaging.BitmapData bmpData;
        private void drawModel()
        {
            objReader = model.copy();

            DateTime t1 = DateTime.Now;
            Parallel.For(0, objReader.vertices.Length, transformVertex);

            DateTime t2 = DateTime.Now;
            Debug.WriteLine((t2 - t1).TotalSeconds);

             bmpData =
                bitmap.LockBits(rect, System.Drawing.Imaging.ImageLockMode.ReadWrite,
                bitmap.PixelFormat);
            IntPtr ptr = bmpData.Scan0;
            
            rgbValues = new byte[Math.Abs(bmpData.Stride) * bmpData.Height];
            for (int i = 3; i < rgbValues.Length; i+=4)
            {
                rgbValues[i] = 255;
            }

            Parallel.For(0, objReader.polygons.Count, drawPoly);

            System.Runtime.InteropServices.Marshal.Copy(rgbValues, 0, ptr, rgbValues.Length);
            bitmap.UnlockBits(bmpData);
            pictureBox1.Refresh();
        }
        
        private int x = 0;
        private int y = 0;
        private float rot = 0.002f;
        private float radius = 1;
        private void Form1_MouseMove(object sender, MouseEventArgs e)
        {

            if (a)
            {
                DateTime t1 = DateTime.Now;
                alpha += (e.X - x) * rot;
                beta += (e.Y - y) * rot;
                if (beta > MathF.PI / 2)
                    beta = MathF.PI / 2;
                if (beta < -MathF.PI / 2)
                    beta = -MathF.PI / 2;
                float sina = MathF.Sin(alpha);
                float sinb = MathF.Sin(beta);
                float cosa = MathF.Cos(alpha);
                float cosb = MathF.Cos(beta);
                cameraPos = new Vector4(sina * cosb, sinb + 0.25f, cosa * cosb, 1) * radius;
                float temp = sinb;
                sinb = cosb;
                cosb = -temp;
                cameraUp = new Vector4(sina * cosb, sinb , cosa * cosb, 1);
                x = e.X;
                y = e.Y;
                drawModel();
                DateTime t2 = DateTime.Now;
                Debug.WriteLine((t2 - t1).TotalSeconds);
            }
        }
        bool a = false;
        private void pictureBox1_MouseDown(object sender, MouseEventArgs e)
        {
            x = e.X;
            y = e.Y;
            a = true;
        }

        private void pictureBox1_MouseUp(object sender, MouseEventArgs e)
        {
            a = false;
        }

        private void Form1_Resize(object sender, System.EventArgs e)
        {
            Control control = (Control)sender;

            bitmap = new Bitmap(control.Size.Width + 1, control.Size.Height + 1);
            pictureBox1.Image = bitmap;
            rect = new Rectangle(0, 0, bitmap.Width, bitmap.Height);

            drawModel();
        }

        private void pictureBox1_Paint(object sender, PaintEventArgs e)
        {
            e.Graphics.DrawImage(bitmap, 0, 0);
        }
    }
}
