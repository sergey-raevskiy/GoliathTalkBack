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
                using (var icon = new TkbTrayIcon(menu))
                using (var controller  = new TkbController(icon, menu))
                using (new KeyboardHook(controller))
                {
                    menu.AdviseEvents(controller);

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
