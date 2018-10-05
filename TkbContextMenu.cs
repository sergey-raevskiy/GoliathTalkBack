using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;

namespace GoliathTalkBack
{
    class TkbContextMenu : ContextMenu
    {
        private Dictionary<string, MenuItem> m_Devices =
            new Dictionary<string, MenuItem>();

        private MenuItem m_NoDevices;
        private MenuItem m_DevSep;

        public TkbContextMenu()
        {
            m_NoDevices = this.MenuItems.Add("(no devices found)");
            m_NoDevices.Enabled = false;
            m_NoDevices.Visible = true;

            m_DevSep = this.MenuItems.Add("-");

            this.MenuItems.Add("Exit");
        }

        public void AddDevice(string displayName, string id)
        {
            var item = new MenuItem(displayName);
            item.Tag = id;
            m_Devices.Add(id, item);
            int pos = this.MenuItems.IndexOf(m_DevSep);
            this.MenuItems.Add(pos, item);

            m_NoDevices.Visible = false;
        }

        public void RemoveDevice(string id)
        {
            this.MenuItems.Remove(m_Devices[id]);
            m_Devices.Remove(id);

            if (m_Devices.Count == 0)
                m_NoDevices.Visible = true;
        }
    }
}
