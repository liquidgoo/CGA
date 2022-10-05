using System;
using System.Collections.Generic;
using System.Linq;
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

            Vector3 v1 = new Vector3(0.1f, 0.1f, 0.2f);
            Vector3 v2 = new Vector3(0.1f, 0.1f, 2);

            VectorTransform transform = new VectorTransform();

            v1 = transform.ViewToClipFOV(v1, 1.57f, 1, 2, 6);
            v2 = transform.ViewToClipFOV(v2, 1.57f, 1, 2, 6);

        }
    }
}
