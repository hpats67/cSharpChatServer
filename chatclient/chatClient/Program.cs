using System;
using System.Text;
using System.Net.Sockets;
using System.Threading;
using System.Windows.Forms;

namespace chatClient
{
    static class Program
    {
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new Form1());
        }

    }
}
