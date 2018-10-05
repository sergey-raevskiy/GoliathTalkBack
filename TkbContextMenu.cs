using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;

namespace GoliathTalkBack
{
    interface ITkbContextMenuEvents
    {
        void OnDeviceSelected(string id);
        void OnExit();
    }

    class TkbContextMenu : ContextMenu
    {
        private Dictionary<string, MenuItem> m_Devices =
            new Dictionary<string, MenuItem>();

        private MenuItem m_NoDevices;
        private MenuItem m_DevSep;
        private ITkbContextMenuEvents m_Events;

        private void OnDeviceClick(object sender, EventArgs e)
        {
            if (m_Events != null)
            {
                var item = (MenuItem)sender;
                var id = (string)item.Tag;
                m_Events.OnDeviceSelected(id);
            }
        }

        private void OnExit(object sender, EventArgs e)
        {
            if (m_Events != null)
                m_Events.OnExit();
        }

        public TkbContextMenu()
        {
            m_NoDevices = this.MenuItems.Add("(no devices found)");
            m_NoDevices.Enabled = false;
            m_NoDevices.Visible = true;

            m_DevSep = this.MenuItems.Add("-");

            this.MenuItems.Add("Exit", OnExit);
        }

        public void AdviseEvents(ITkbContextMenuEvents events)
        {
            m_Events = events;
        }

        public void AddDevice(string displayName, string id)
        {
            var item = new MenuItem(displayName, OnDeviceClick);
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

        public void ClearDeviceSelection()
        {
            foreach (var device in m_Devices.Values)
                device.Checked = false;
        }

        public void SelectDevice(string id)
        {
            ClearDeviceSelection();
            m_Devices[id].Checked = true;
        }
    }
}
