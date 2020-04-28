using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Net.Sockets;

namespace CprBroker.Engine
{
    public class TcpServer : IDisposable
    {
        public int Port { get; set; }
        public string Address { get; set; }

        public readonly object LockObject = new object();

        System.Net.Sockets.TcpListener _Listener;

        public int InputMessageSize { get; set; } = 12;
        public int MaxThreads { get; set; } = 5;
        public TimeSpan MaxWait { get; set; } = TimeSpan.FromSeconds(1);
        public Encoding TextEncoding { get; set; } = Encoding.ASCII;

        public bool Start()
        {
            IPAddress ipAddress;
            if (string.IsNullOrEmpty(Address))
            {
                System.Net.IPHostEntry localhost = System.Net.Dns.GetHostEntry("localhost");
                ipAddress = localhost.AddressList.Last();
            }
            else
            {
                ipAddress = IPAddress.Parse(Address);
            }
            CprBroker.Engine.Local.Admin.LogFormattedSuccess("Starting TCP server at address <{0}>, port <{1}>", ipAddress, Port);

            _Listener = new TcpListener(ipAddress, Port);
            _Listener.Start();

            for (int i = 0; i < MaxThreads; i++)
            {
                var dummyAsyncResult = _Listener.BeginAcceptTcpClient(new AsyncCallback(AcceptTcpClient), null);
            }
            CprBroker.Engine.Local.Admin.LogFormattedSuccess("TCP server started address <{0}>, port <{1}>", ipAddress, Port);
            return true;
        }

        void AcceptTcpClient(IAsyncResult result)
        {
            try
            {
                BrokerContext.Initialize(Utilities.Constants.EventBrokerApplicationToken.ToString(), Utilities.Constants.UserToken);

                try
                {
                    using (TcpClient tcpClient = _Listener.EndAcceptTcpClient(result))
                    {
                        Local.Admin.LogFormattedSuccess("TcpServer <{0}>: client connected", this.ToString());

                        int totalReadBytes = 0;
                        DateTime startTime = DateTime.Now;

                        byte[] messageBytes = new byte[InputMessageSize];
                        using (var stream = tcpClient.GetStream())
                        {
                            while (totalReadBytes < messageBytes.Length && DateTime.Now - startTime < MaxWait)
                            {
                                if (stream.DataAvailable)
                                {
                                    int readBytes = stream.Read(messageBytes, totalReadBytes, 1);// InputMessageSize - totalReadBytes
                                    totalReadBytes += readBytes;
                                }
                                else
                                {
                                    // Let some time pass
                                    System.Threading.Thread.Sleep(10);
                                }
                            }

                            Local.Admin.LogFormattedSuccess("TcpServer <{0}>: processing operation <{1}>",
                                this.ToString(), this.TextEncoding.GetString(messageBytes).Substring(0, 2)
                                );
                            var responseBytes = ProcessMessage(messageBytes.Take(totalReadBytes).ToArray());
                            stream.Write(responseBytes, 0, responseBytes.Length);
                        }
                    }
                }
                catch (Exception ex)
                {
                    Local.Admin.LogException(ex);
                }
            }
            finally
            {
                lock (LockObject)
                {
                    if (_Listener != null)
                        _Listener.BeginAcceptTcpClient(AcceptTcpClient, null);
                }
            }
        }

        public void Stop()
        {
            lock (LockObject)
            {
                var tmpListener = _Listener;
                _Listener = null;

                if (tmpListener != null)
                    tmpListener.Stop();
            }
        }

        public virtual byte[] ProcessMessage(byte[] message)
        {
            return new byte[] { };
        }

        public void Dispose()
        {
            Stop();
        }
    }
}
