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
        private static Icon GetIcon(string name)
        {
            var resources = new ComponentResourceManager(typeof(MainForm));
            return (Icon)(resources.GetObject(name));
        }

        public static readonly Icon s_icoTalkBackOff = GetIcon("trayIcon.Icon");
        private static readonly Icon s_icoTalkBackOn = GetIcon("talkbackOn");

        public MainForm()
        {
            InitializeComponent();
            trayIcon.Icon = s_icoTalkBackOff;
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
                using (UdpClient udpclient = new UdpClient())
                {
                    IPAddress multicastaddress = IPAddress.Parse("239.192.5.8");
                    udpclient.JoinMulticastGroup(multicastaddress);
                    IPEndPoint remoteep = new IPEndPoint(multicastaddress, 5008);
                    
                    var buffer =
                        Encoding.ASCII.GetBytes(
                            "{\"interval\": 500.0, \"type\": \"_antelope_control._tcp.local.\", \"name\": \"Goliath, SN: 2202717080302 uuid: 0274ead2-f57e-4f72-af71-b2a289c11dbe address: 192-168-2-5:2021._antelope_control._tcp.local.\", \"ip\": \"192.168.2.5\", \"properties\": {\"connection_type\": \"USB\", \"vendor_id\": \"0x23e5\", \"serial_number\": \"2202717080302\", \"hardware_version\": \"11.0\", \"server_version\": \"1.4.8\", \"firmware_version\": \"3.61\", \"mode\": \"app\", \"device_name\": \"Goliath\", \"product_id\": \"0xa150\"}, \"uuid\": \"0274ead2-457e-4f72-af71-b2a289c11dbe\", \"protocol\": \"TCP\", \"port\": 2021}");
                    udpclient.Send(buffer, buffer.Length, remoteep);
                    Console.WriteLine("Sent");
                }
            }
            else if (key == Keys.T)
            {
                trayIcon.Icon = s_icoTalkBackOn;
            }
        }

        void IHookCallback.OnKeyUp(Keys key)
        {
            if (key == Keys.T)
            {
                trayIcon.Icon = s_icoTalkBackOff;
            }
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
