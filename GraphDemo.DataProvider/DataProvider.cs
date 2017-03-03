using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows;
using System.Windows.Threading;
using GraphDemo.Data;
using Microsoft.Research.DynamicDataDisplay.DataSources;

namespace GraphDemo.DataProvider
{
    public class DataProvider : ICallback
    {
        private UdpConnection _connection;
        private int i;
        public event ProcessDataDelegate ProcessData;
        public event ReceiveDataDelegate ReceiveData;

        public ObservableDataSource<Point> DataSource1;
        public ObservableDataSource<Point> DataSource2;
        public ObservableDataSource<Point> DataSource3;
        public ObservableDataSource<Point> DataSource4;
        public delegate void ProcessDataDelegate(short[] data1,short[] data2,short[] data3,short[] data4);
        public delegate void ReceiveDataDelegate();

        public DataProvider()
        {
            _connection = new UdpConnection(this);
            _connection.OPEN();
        }

        


        void ICallback.AddPoints(List<Message> messages)
        {
            ProcessDataDelegate pdt = ProcessData;
            if (pdt != null)
            {
                foreach (Message message in messages)
                {
                    pdt(message.Data1, message.Data2, message.Data3, message.Data4);
                }
            }
        }


        void ICallback.test()
        {
            ReceiveDataDelegate rdd = ReceiveData;
            if (rdd != null)
            {
                rdd();
            }
        }
    }
}
