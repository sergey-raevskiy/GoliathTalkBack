using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;

namespace GoliathTalkBack
{
    class TkbTrayIcon : IDisposable
    {
        private NotifyIcon m_Icon;

        public TkbTrayIcon(ContextMenu menu)
        {
            m_Icon = new NotifyIcon();
            m_Icon.ContextMenu = menu;
            m_Icon.Icon = MainForm.s_icoTalkBackOff;
            m_Icon.Visible = true;
            m_Icon.Text = "Goliath TalkBack";
        }

        public void SetState(bool on)
        {
            m_Icon.Icon = on ? MainForm.s_icoTalkBackOn : MainForm.s_icoTalkBackOff;
        }

        public void Dispose()
        {
            if (m_Icon != null)
                m_Icon.Dispose();
        }
    }
}
