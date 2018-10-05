using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Windows.Forms;

namespace GoliathTalkBack
{
    delegate void Action();

    class TkbController : IDisposable, ITkbContextMenuEvents, IHookCallback, IAntelopeBeaconListenerCallback, IAntelopeClientEvents
    {
        private TkbTrayIcon m_Icon;
        private TkbContextMenu m_Menu;
        private UiInvoke m_UiIvoke;
        private AntelopeClient m_Client;

        public TkbController(TkbTrayIcon icon, TkbContextMenu menu, UiInvoke uiInvoke)
        {
            m_Icon = icon;
            m_Menu = menu;
            m_UiIvoke = uiInvoke;
            m_Client = new AntelopeClient(this);
        }

        public void Dispose()
        { }

        private void TalkBackOn()
        {
            if (m_Client.IsConnected)
            {
                m_Client.SendMessage("[\"set_tbk_enable\", [0, 1], {}]");
            }
        }

        private void TalkBackOff()
        {
            if (m_Client.IsConnected)
            {
                m_Client.SendMessage("[\"set_tbk_enable\", [0, 0], {}]");
            }
        }

        private Dictionary<string, AntelopeBeacon> m_Beacons =
            new Dictionary<string, AntelopeBeacon>();

        private void NewServerDiscovered(AntelopeBeacon beacon)
        {
            if (beacon.Properties.ProductId != "0xa150")
                return;

            m_Beacons.Add(beacon.Uuid, beacon);
            m_Menu.AddDevice(string.Format("Goliath {0}", beacon.Properties.SerialNumber), beacon.Uuid);

            if (!m_Client.IsConnected)
            {
                m_Icon.ShowBaloon("New device found", string.Format("Connecting to {0}:{1}", beacon.Ip, beacon.Port), ToolTipIcon.None);
                m_Client.Connect(beacon.Ip, beacon.Port, beacon.Uuid);
            }
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

            if (key == Keys.RControlKey)
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

            if (key == Keys.RControlKey)
            {
                TalkBackOff();
                m_PauseBrkPressed = false;
            }
        }

        void IAntelopeBeaconListenerCallback.OnNewServerDiscovered(System.Net.IPEndPoint remoteEndPoint, AntelopeBeacon beacon)
        {
            m_UiIvoke.Invoke(() => NewServerDiscovered(beacon));
        }

        void IAntelopeClientEvents.OnConnected(string cookie)
        {
            var beacon = m_Beacons[cookie];
            m_Icon.ShowBaloon("Connection successfull", string.Format("Successfully connected to {0}:{1}", beacon.Ip, beacon.Port), ToolTipIcon.None);
            m_Menu.SelectDevice(cookie);
        }

        void IAntelopeClientEvents.OnError(string cookie, string error)
        {
            var beacon = m_Beacons[cookie];
            m_Icon.ShowBaloon("Connection failed", string.Format("Failed to connected to {0}:{1}: {2}", beacon.Ip, beacon.Port, error), ToolTipIcon.Error);
        }

        void IAntelopeClientEvents.OnReport(string cookie, AntelopeReport report)
        {
        }
    }
}
