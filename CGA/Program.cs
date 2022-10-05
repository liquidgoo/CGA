using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CGA
{
    static class Program
    {
        /// <summary>
        ///  The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.SetHighDpiMode(HighDpiMode.SystemAware);
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new Form1());

            Vector4 v1 = new Vector4(0.1f, 0.1f, 0.2f, 1f);
            Vector4 v2 = new Vector4(0.1f, 0.1f, 2, 1f);

            VectorTransform transform = new VectorTransform();

            v1 = transform.ViewToClipFOV(v1, 1.57f, 1, 2, 6);
            v2 = transform.ViewToClipFOV(v2, 1.57f, 1, 2, 6);

        }
    }
}
