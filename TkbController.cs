using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;

namespace GoliathTalkBack
{
    class TkbController : IDisposable, ITkbContextMenuEvents
    {
        private TkbTrayIcon m_Icon;
        private TkbContextMenu m_Menu;

        public TkbController(TkbTrayIcon icon, TkbContextMenu menu)
        {

        }

        public void Dispose()
        { }

        void ITkbContextMenuEvents.OnDeviceSelected(string id)
        {

        }

        void ITkbContextMenuEvents.OnExit()
        {
            Application.Exit();
        }
    }
}
