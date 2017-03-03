using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GraphDemo.Data
{
    public class Message
    {
        public short[] Data1;
        public short[] Data2;
        public short[] Data3;
        public short[] Data4;

        public Message(short[] data1, short[] data2,short[] data3,short[] data4)
        {
            Data1 = data1;
            Data2 = data2;
            Data3 = data3;
            Data4 = data4;

        }
    }


}
