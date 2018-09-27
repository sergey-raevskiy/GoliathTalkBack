using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Net;
using System.Net.Sockets;
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

            if (key == Keys.X)
            {
                UdpClient udpclient = new UdpClient();

                IPAddress multicastaddress = IPAddress.Parse("239.192.5.8");
                udpclient.JoinMulticastGroup(multicastaddress);
                IPEndPoint remoteep = new IPEndPoint(multicastaddress, 5008);

                var buffer =
                    Encoding.ASCII.GetBytes(
                        "{uuid: '35382F95-396A-4E2D-92D2-3F4977B0BB6E', properties: {product_id: '0x023'} }");
                udpclient.Send(buffer, buffer.Length, remoteep);
                Console.WriteLine("Sent");
            }
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

        void IAntelopeBeaconListenerCallback.OnNewServerDiscovered(IPEndPoint remoteEndPoint, AntelopeBeacon beacon)
        {
            Console.WriteLine("New server discovered: {0} {1}", remoteEndPoint, beacon);
        }
    }
}
