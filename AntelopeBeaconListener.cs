using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using Newtonsoft.Json;

namespace GoliathTalkBack
{
    internal class AntelopeBeaconProperties
    {
        [JsonProperty("product_id")]
        public string ProductId { get; set; }

        [JsonProperty("serial_number")]
        public string SerialNumber { get; set; }
    }

    class AntelopeBeacon
    {
        [JsonProperty("uuid")]
        public string Uuid { get; set; }

        [JsonProperty("properties")]
        public AntelopeBeaconProperties Properties { get; set; }

        [JsonProperty("ip")]
        public string Ip { get; set; }

        [JsonProperty("port")]
        public int Port { get; set; }

        public override string ToString()
        {
            return JsonConvert.SerializeObject(this);
        }
    }

    interface IAntelopeBeaconListenerCallback
    {
        void OnNewServerDiscovered(IPEndPoint remoteEndPoint, AntelopeBeacon beacon);
    }

    class AntelopeBeaconListener : IDisposable
    {
        private static readonly IPAddress AntelopeMulticastGroupAddress = IPAddress.Parse("239.192.5.8");
        private static readonly int AntelopeMulticastGroupPort = 5008;

        private Thread m_Thread;
        private ManualResetEvent m_StopEvent = new ManualResetEvent(false);
        private IAntelopeBeaconListenerCallback m_Callback;

        public AntelopeBeaconListener(IAntelopeBeaconListenerCallback callback)
        {
            m_Callback = callback;
            m_Thread = new Thread(ReceiveThread);
            m_Thread.Start();
        }

        private Dictionary<string, AntelopeBeacon> m_KnownBeacons
            = new Dictionary<string, AntelopeBeacon>();

        private void OnBeaconRecieved(IPEndPoint remoteEndPoint, AntelopeBeacon beacon)
        {
            if (m_KnownBeacons.ContainsKey(beacon.Uuid))
                return;

            m_KnownBeacons.Add(beacon.Uuid, beacon);
            m_Callback.OnNewServerDiscovered(remoteEndPoint, beacon);
        }

        private void ReceiveThread()
        {
            try
            {
                using (var client = new UdpClient())
                {
                    client.ExclusiveAddressUse = false;
                    client.Client.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, true);
                    client.Client.Bind(new IPEndPoint(IPAddress.Any, AntelopeMulticastGroupPort));
                    client.JoinMulticastGroup(AntelopeMulticastGroupAddress);

                    while (true)
                    {
                        var remoteEndPoint = new IPEndPoint(IPAddress.Any, 0);
                        string message = null;

                        var ar = client.BeginReceive(null, null);
                        int rc = WaitHandle.WaitAny(new[] { ar.AsyncWaitHandle, m_StopEvent });
                        if (rc == 1)
                        {
                            break;
                        }
                        else if (rc != 0)
                        {
                            throw new Exception(string.Format("Unexpected wait status: {0}", rc));
                        }

                        var datagram = client.EndReceive(ar, ref remoteEndPoint);

                        try
                        {
                            message = Encoding.ASCII.GetString(datagram);

                            var beacon = JsonConvert.DeserializeObject<AntelopeBeacon>(message);

                            OnBeaconRecieved(remoteEndPoint, beacon);
                        }
                        catch (Exception ex)
                        {
                            Console.Error.WriteLine("Error while receiving packet");
                            Console.Error.WriteLine("Remote endpoint: '{0}'", remoteEndPoint);
                            Console.Error.WriteLine("Message: '{0}'", message ?? "(null)");
                            Console.Error.WriteLine("Error: '{0}'", ex);
                        }
                    }

                    client.Close();
                }
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine("Receive thread failed: {0}", ex);
            }
        }

        public void Dispose()
        {
            m_StopEvent.Set();
            m_Thread.Join();
        }

        ~AntelopeBeaconListener()
        {
            // todo
        }
    }
}
