using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Microsoft.Research.DynamicDataDisplay.DataSources;

namespace GraphDemo.DataProvider
{
    class Program
    {
        static void Main(string[] args)
        {
            ObservableDataSource<Point> DataSource1 = new ObservableDataSource<Point>();
            GraphDataProvider dataProvider = new GraphDataProvider(DataSource1);

        }
    }
}
