using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Text;

namespace GoliathTalkBack
{
    class AntelopeReport
    {

    }

    interface IAntelopeClientEvents
    {
        void OnConnected(string cookie);
        void OnError(string cookie, string error);
        void OnReport(string cookie, AntelopeReport report);
    }

    class AntelopeClient : IDisposable
    {
        private TcpClient m_Client;
        private string m_Cookie;
        private IAntelopeClientEvents m_Events;

        public AntelopeClient(IAntelopeClientEvents events)
        {
            m_Events = events;
        }

        public void Connect(string host, int port, string cookie)
        {
            if (m_Client != null)
            {
                try
                {
                    m_Client.Close();
                }
                catch
                {
                    // Ignore all errors here
                }

                m_Client = null;
            }

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
