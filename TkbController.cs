using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;

namespace GoliathTalkBack
{
    class TkbController : IDisposable, ITkbContextMenuEvents, IHookCallback
    {
        private TkbTrayIcon m_Icon;
        private TkbContextMenu m_Menu;

        public TkbController(TkbTrayIcon icon, TkbContextMenu menu)
        {
            m_Icon = icon;
            m_Menu = menu;
        }

        public void Dispose()
        { }

        private void TalkBackOn()
        {
            m_Icon.SetState(true);
        }

        private void TalkBackOff()
        {
            m_Icon.SetState(false);
        }

        void ITkbContextMenuEvents.OnDeviceSelected(string id)
        {

        }

        void ITkbContextMenuEvents.OnExit()
        {
            Application.Exit();
        }

        private bool m_PauseBrkPressed;

        void IHookCallback.OnKeyDown(Keys key)
        {
            //Console.WriteLine("KeyDown: {0}", key);

            if (key == Keys.RShiftKey)
            {
                if (!m_PauseBrkPressed)
                {
                    TalkBackOn();
                }

                m_PauseBrkPressed = true;
            }
        }

        void IHookCallback.OnKeyUp(Keys key)
        {
            //Console.WriteLine("KeyUp: {0}", key);

            if (key == Keys.RShiftKey)
            {
                TalkBackOff();
                m_PauseBrkPressed = false;
            }
        }
    }
}
