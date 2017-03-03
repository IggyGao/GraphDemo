using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GraphDemo.Data;

namespace GraphDemo.DataProvider
{
    public interface ICallback
    {
        void AddPoints(List<Message> messages);
        void test();
    }
}
