using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CGA
{
    public partial class Form1 : Form
    {

        private VectorTransform transform = new VectorTransform();

        private Vector4 cameraPos = new Vector4(0, 0.25f, -0.75f, 1f);
        private Vector4 cameraTarget = new Vector4(0, 0.25f, 0, 1f);
        private Vector4 cameraUp = new Vector4(0, 1, 0, 1f);

        private Vector4 xAxis = new Vector4(1, 0, 0, 1f);
        private Vector4 yAxis = new Vector4(0, 1, 0, 1f);
        private Vector4 zAxis = new Vector4(0, 0, 1, 1f);


        Bitmap bitmap = new Bitmap(1081, 721);
        ObjReader model = new ObjReader("C:\\Users\\vanya\\source\\repos\\CGA\\CGA\\untitled.obj");
        Rectangle rect;
        public Form1()
        {
            InitializeComponent();


            pictureBox1.Image = bitmap;
            rect = new Rectangle(0, 0, bitmap.Width, bitmap.Height);
            drawModel();
            
        }

        private void drawModel()
        {
            DateTime time = DateTime.Now;
            ObjReader objReader = model.copy();
            foreach (Vector4 vertex in objReader.vertices)
            {
                transform.updateFrom(vertex, transform.LocalToWorld(vertex, xAxis, yAxis, zAxis, new Vector4(0,0,0, 1f)));
                transform.updateFrom(vertex, transform.WorldToView(vertex, cameraPos, cameraTarget, cameraUp));
                transform.updateFrom(vertex, transform.ViewToClip(vertex, 1.778f, 1, 0, 100, ProjectionMode.Pespective));

            }
            DateTime time2 = DateTime.Now;
            Debug.WriteLine((time2 - time).TotalSeconds);

            time = time2;
            System.Drawing.Imaging.BitmapData bmpData =
                bitmap.LockBits(rect, System.Drawing.Imaging.ImageLockMode.ReadWrite,
                bitmap.PixelFormat);

            
            IntPtr ptr = bmpData.Scan0;
            
            int bytes = Math.Abs(bmpData.Stride) * bitmap.Height;
            byte[] rgbValues = new byte[bytes];
            for (int i = 3; i < rgbValues.Length; i+=4)
            {
                rgbValues[i] = 255;
            }

            IRasterization rasterization = new Bresenham();
            foreach (Polygon polygon in objReader.polygons)
            {
                for (int i = 0; i < polygon.length; i++)
                {
                    Vector4 p1 = polygon.vertices[i];
                    Vector4 p2 = polygon.vertices[(i + 1) % polygon.length];
                    if (p1.X > 1 || p1.X < -1 || p1.Y > 1 || p1.Y < -1 || p1.Z > 1 || p1.Z < 0 ||
                        p2.X > 1 || p2.X < -1 || p2.Y > 1 || p2.Y < -1 || p2.Z > 1 || p2.Z < 0) continue;
                    p1 = transform.ClipToScreen(p1, 1080, 720, 0, 0);
                    p2 = transform.ClipToScreen(p2, 1080, 720, 0, 0);

                    foreach ((int, int) point in rasterization.Rasterize(p1.X, p1.Y, p2.X, p2.Y))
                    {
                        int pixel = point.Item2 * bmpData.Stride + point.Item1 * 4 ;
                        rgbValues[pixel] = 255;
                        rgbValues[pixel + 1] = 255;
                        rgbValues[pixel + 2] = 255;
                        rgbValues[pixel + 3] = 255;

                       
                    }
                }
            }
            time2 = DateTime.Now;
            Debug.WriteLine((time2 - time).TotalSeconds);
            
            System.Runtime.InteropServices.Marshal.Copy(rgbValues, 0, ptr, bytes);

            
            bitmap.UnlockBits(bmpData);
            pictureBox1.Image = bitmap;

        }
        private int x = 0;
        private int y = 0;
        private float rot = 0.005f;

        private void Form1_MouseMove(object sender, MouseEventArgs e)
        {
            if (a)
            {
                xAxis = transform.RotateY(xAxis, -(e.X - x) * rot);
                zAxis = transform.RotateY(zAxis, -(e.X - x) * rot);
                x = e.X;
                y = e.Y;
                drawModel();
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
    }
}
