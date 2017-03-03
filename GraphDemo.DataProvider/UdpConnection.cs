using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using GraphDemo.Data;

namespace GraphDemo.DataProvider
{
    public class UdpConnection
    {
        private UdpClient _udpcRecv;
        private Resolver _resolver;
        private MessageBuffer _buffer;

        public UdpConnection(ICallback callback)
        {
            _resolver = new Resolver(callback);
            IPEndPoint localIpep = new IPEndPoint(
                     IPAddress.Parse("192.168.1.100"), 4200); // 本机IP和监听端口号
            
            _udpcRecv = new UdpClient(localIpep);
            _buffer = new MessageBuffer(1024 * 1024, _resolver);
        }

        public void OPEN()
        {
            Thread thrRecv = new Thread(ReceiveMessage);
            thrRecv.IsBackground = true;
            thrRecv.Start();
            Console.WriteLine("UDP监听器已成功启动");
        }
        private void ReceiveMessage(object obj)
        {
            IPEndPoint remoteIpep = new IPEndPoint(IPAddress.Parse
                ("192.168.1.110"), 3500);
            while (true)
            {
                try
                {
                    byte[] bytRecv = _udpcRecv.Receive(ref remoteIpep);
                    var by = new byte[bytRecv.Length];
                    Array.Copy(bytRecv, by, bytRecv.Length);
                    _buffer.PutData(by);
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                    break;
                }
            }
        }
    }

}