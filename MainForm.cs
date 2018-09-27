using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Net;
using System.Text;
using System.Windows.Forms;

namespace GoliathTalkBack
{
    public partial class MainForm : Form, IHookCallback, IAntelopeBeaconListenerCallback
    {
        public MainForm()
        {
            //NotifyIcon

            InitializeComponent();
        }

        private void trayIcon_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            if (!Visible)
            {

                Visible = true;
            }
        }

        protected override void OnClosing(CancelEventArgs e)
        {
            e.Cancel = true;
            Hide();
            base.OnClosing(e);
        }

        void IHookCallback.OnKeyDown(Keys key)
        {
            Console.WriteLine(key);
        }

        void IHookCallback.OnKeyUp(Keys key)
        {
            
        }

        private void trayIcon_MouseDown(object sender, MouseEventArgs e)
        {
            Console.WriteLine("Mouse down: {0}", e.Button);
        }

        private void trayIcon_MouseUp(object sender, MouseEventArgs e)
        {
            Console.WriteLine("Mouse up: {0}", e.Button);
        }

        void IAntelopeBeaconListenerCallback.OnBeaconRecieved(IPEndPoint remoteEndPoint, AntelopeBeacon beacon)
        {
            Console.WriteLine("Beacon: {0} {1}", remoteEndPoint, beacon);
        }
    }
}
