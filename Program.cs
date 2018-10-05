using System;
using System.Windows.Forms;

namespace GoliathTalkBack
{
    static class Program
    {
        [STAThread]
        static void Main()
        {
            try
            {
                Application.EnableVisualStyles();
                Application.SetCompatibleTextRenderingDefault(false);

                // todo: avoid race condition?

                using (var menu = new TkbContextMenu())
                using (new TkbTrayIcon(menu))
                {
                    Application.Run();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Exception in Main(): {0}", ex);
            }
        }
    }
}
