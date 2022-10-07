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

        private Vector4 cameraPos = new Vector4(0, 0.25f, -1f, 1f);
        private Vector4 cameraTarget = new Vector4(0, 0.25f, 0, 1f);
        private Vector4 cameraUp = new Vector4(0, 1, 0, 1f);

        private Vector4 xAxis = new Vector4(1, 0, 0, 1f);
        private Vector4 yAxis = new Vector4(0, 1, 0, 1f);
        private Vector4 zAxis = new Vector4(0, 0, 1, 1f);

        float alpha = 0;
        float beta = 0;

        Bitmap bitmap;
        ObjReader model = new ObjReader("C:\\Users\\vanya\\source\\repos\\CGA\\CGA\\untitled.obj");
        Rectangle rect;
        public Form1()
        {
            InitializeComponent();

            bitmap = new Bitmap(this.Width + 1, this.Height + 1);

            pictureBox1.Image = bitmap;
            rect = new Rectangle(0, 0, bitmap.Width, bitmap.Height);
            drawModel();
            
        }

        private void drawModel()
        {
            DateTime time = DateTime.Now;
            ObjReader objReader = model.copy();
            for (int i = 0;  i < objReader.vertices.Count; i++)
            {
                Vector4 vertex = objReader.vertices[i];
                vertex = transform.updateFrom(vertex, transform.LocalToWorld(vertex, xAxis, yAxis, zAxis, new Vector4(0,0,0, 1f)));
                vertex = transform.updateFrom(vertex, transform.WorldToView(vertex, cameraPos, cameraTarget, cameraUp));
                vertex = transform.updateFrom(vertex, transform.ViewToClip(vertex, 1.778f, 1, 0, 100, ProjectionMode.Pespective));
                objReader.vertices[i] = vertex;
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
            for (int i = 0; i < objReader.polygons.Count; i++)
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

                 /*   polygon.vertices[j] = p1;
                    polygon.vertices[(j + 1) % polygon.length] = p2;*/
                    /*  objReader.vertices[polygon.ind[j]] = p1;
                      objReader.vertices[polygon.ind[(j + 1) % polygon.length]] = p2;*/

                    foreach ((int, int) point in rasterization.Rasterize(p1.X, p1.Y, p2.X, p2.Y))
                    {
                        int pixel = point.Item2 * bmpData.Stride + point.Item1 * 4 ;
                        rgbValues[pixel] = 255;
                        rgbValues[pixel + 1] = 255;
                        rgbValues[pixel + 2] = 255;
                        rgbValues[pixel + 3] = 255;

                       
                    }
                }
                objReader.polygons[i] = polygon;
            }
            time2 = DateTime.Now;
            Debug.WriteLine((time2 - time).TotalSeconds);
            
            System.Runtime.InteropServices.Marshal.Copy(rgbValues, 0, ptr, bytes);

            Debug.WriteLine("Vertex: " + objReader.vertices[0]);
            Debug.WriteLine("Polygon vertex: " + objReader.polygons[0].vertices[0]);
            bitmap.UnlockBits(bmpData);
            pictureBox1.Image = bitmap;

        }
        private int x = 0;
        private int y = 0;
        private float rot = 0.0005f;

        private void Form1_MouseMove(object sender, MouseEventArgs e)
        {
            if (a)
            {
                alpha += (e.X - x) * rot;
                beta += (e.Y - y) * rot;
                float sina = MathF.Sin(alpha);
                float sinb = MathF.Sin(beta);
                float cosa = MathF.Cos(alpha);
                float cosb = MathF.Cos(beta);
                zAxis = new Vector4(sina * cosb, sinb, cosa * cosb, 1f);
                 sina = MathF.Sin(alpha + MathF.PI /2);
                 sinb = MathF.Sin(beta);
                 cosa = MathF.Cos(alpha + MathF.PI / 2);
                 cosb = MathF.Cos(beta);
                xAxis = new Vector4(sina * cosb, sinb, cosa * cosb, 1f);
                sina = MathF.Sin(alpha );
                sinb = MathF.Sin(beta + MathF.PI / 2);
                cosa = MathF.Cos(alpha );
                cosb = MathF.Cos(beta + MathF.PI / 2);
                yAxis = new Vector4(sina * cosb, sinb, cosa * cosb, 1f);
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

        private void Form1_Resize(object sender, System.EventArgs e)
        {
            Control control = (Control)sender;

            bitmap = new Bitmap(control.Size.Width + 1, control.Size.Height + 1);
            pictureBox1.Image = bitmap;
            rect = new Rectangle(0, 0, bitmap.Width, bitmap.Height);

            drawModel();
        }
    }
}
