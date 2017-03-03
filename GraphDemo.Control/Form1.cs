// -----------------------------------------------------------------------
// <copyright file="Form1.cs" company="None.">
//  By Philip R. Braica (HoshiKata@aol.com, VeryMadSci@gmail.com)
//
//  Distributed under the The Code Project Open License (CPOL)
//  http://www.codeproject.com/info/cpol10.aspx
// </copyright>
// -----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using GraphDemo.DataProvider;

namespace GraphDemo.Control
{
    /// <summary>
    /// Form class.
    /// </summary>
    public partial class Form1 : Form
    {
        //protected SoundSource source = null;
        //protected SoundPlayback playback = null;
        private DataProvider.DataProvider _dataProvider;

        /// <summary>
        /// Constructor.
        /// </summary>
        public Form1()
        {
            InitializeComponent();
            System.Windows.Forms.Control.CheckForIllegalCrossThreadCalls = false;
            _dataProvider = new DataProvider.DataProvider();
            _dataProvider.ProcessData += OnAppendpoints1;
            _dataProvider.ReceiveData += OnReceiveData;
        }

        private void OnAppendpoints1(short[] data1,short[]data2,short[] data3,short[] data4)
        {
            if (graph1.Lines.Count < 1)
            {
                graph1.Lines.Add(new Graph.Line(Color.Blue));
                graph1.Lines.Add(new Graph.Line(Color.Red));
                graph1.Lines.Add(new Graph.Line(Color.Yellow));
                graph1.Lines.Add(new Graph.Line(Color.Green));
            }
            graph1.Lines[0].Update(data1);
            graph1.Lines[1].Update(data2);
            graph1.Lines[2].Update(data3);
            graph1.Lines[3].Update(data4);
            graph1.TriggerRedraw();
        }

        private void OnReceiveData()
        {
            label1.Text = "Data Received";
        }

    }
}
