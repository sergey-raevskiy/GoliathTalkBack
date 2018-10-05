using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace GoliathTalkBack
{
    class AntelopeReport
    {

    }

    interface IAntelopeClientEvents
    {
        void OnConnected(string cookie);
        void OnDisconnected(string cookie);
        void OnError(string cookie, string error);
        void OnReport(string cookie, AntelopeReport report);
    }

    class AntelopeClient : IDisposable
    {
        private TcpClient m_Client;
        private string m_Cookie;
        private IAntelopeClientEvents m_Events;
        private Thread m_ReportReadThread;
        private ManualResetEvent m_ReportReadThreadStop = new ManualResetEvent(false);

        static byte[] ReadFull(NetworkStream ns, int len)
        {
            var buf = new byte[len];
            int off = 0;

            while (len > 0)
            {
                int read = ns.Read(buf, off, len);
                len -= read;
                off += read;
            }

            return buf;
        }

        private void ReportReadThreadFunc()
        {
            var stream = m_Client.GetStream();

            while (!m_ReportReadThreadStop.WaitOne(0))
            {
                try
                {
                    var header = ReadFull(stream, 4);
                    var packetLen =
                        (header[0] << 24) +
                        (header[1] << 16) +
                        (header[2] << 8) +
                        (header[3] << 0);

                    var body = ReadFull(stream, packetLen - 4);
                }
                catch (Exception ex)
                {
                    break;
                }

                // todo: marshal to ui
                m_Events.OnReport(m_Cookie, new AntelopeReport());
            }
        }

        public AntelopeClient(IAntelopeClientEvents events)
        {
            m_Events = events;
        }

        public void Disconnect()
        {
            if (m_Client != null)
            {
                m_ReportReadThreadStop.Set();

                try
                {
                    m_Client.Close();
                }
                catch
                {
                    // Ignore all errors here
                }

                m_ReportReadThread.Join();
                m_Client = null;

                m_Events.OnDisconnected(m_Cookie);
            }
        }

        public void Connect(string host, int port, string cookie)
        {
            Disconnect();

            m_Client = new TcpClient();

            try
            {
                m_Client.Connect(host, port);
                m_Cookie = cookie;
                m_Events.OnConnected(cookie);
            }
            catch (Exception ex)
            {
                m_Client = null;
                m_Events.OnError(cookie, ex.Message);
            }

            if (m_Client != null)
            {
                m_ReportReadThreadStop.Reset();
                m_ReportReadThread = new Thread(ReportReadThreadFunc);
                m_ReportReadThread.Start();
            }
        }

        public bool IsConnected
        {
            get { return m_Client != null; }
        }

        public void SendMessage(string msg)
        {
            var packet = new List<byte>();
            packet.Add(0);
            packet.Add(0);
            packet.Add(0);
            packet.Add(0);
            packet.AddRange(Encoding.ASCII.GetBytes(msg));

            // Fixup header
            packet[0] = (byte)((packet.Count >> 24) & 0xff);
            packet[1] = (byte)((packet.Count >> 16) & 0xff);
            packet[2] = (byte)((packet.Count >> 08) & 0xff);
            packet[3] = (byte)((packet.Count >> 00) & 0xff);

            try
            {
                // todo: write full?
                m_Client.GetStream().Write(packet.ToArray(), 0, packet.Count);
            }
            catch (Exception ex)
            {
                m_Client = null;
                m_Events.OnError(m_Cookie, ex.Message);
            }
        }

        public void Dispose()
        {
            if (m_Client != null)
                m_Client.Close();
        }
    }
}
