using System;
using System.Collections.Generic;
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

                using (var form = new MainForm())
                using (new KeyboardHook(form))
                {
                    Application.Run(form);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Exception in Main(): {0}", ex);
            }
        }
    }
}
