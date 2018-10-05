using System;
using System.Windows.Forms;

namespace GoliathTalkBack
{
    class UiInvoke : Control
    {
        public delegate void Action();

        public void Invoke(Action action)
        {
            base.Invoke(action);
        }
    }

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
                using (var uiInvoke = new UiInvoke())
                using (var controller  = new TkbController(icon, menu, uiInvoke))
                using (new KeyboardHook(controller))
                using (new AntelopeBeaconListener(controller))
                {
                    uiInvoke.CreateControl();

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
