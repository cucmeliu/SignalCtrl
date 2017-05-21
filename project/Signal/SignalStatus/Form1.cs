using FormMain;
using FormMain.utils;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SignalStatus
{
    public partial class Form1 : Form
    {
        private static List<SignalProp> sigList = new List<SignalProp>();
        SignalDao signalDao = SignalDao.getInstance();
        //Thread pingThread;
        PingBiz pingBiz; 

        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            sigList = signalDao.getSignalList();
            pingBiz = new PingBiz(sigList);
           // pingThread = new Thread(pingBiz.ping);
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            listView1.Items.Clear();
            foreach (SignalProp prop in sigList)
            {               
                ListViewItem it = listView1.Items.Add(prop.SignalID);
                it.SubItems.Add(prop.SignalName);
                it.SubItems.Add(prop.Ip);
                //it.SubItems.Add(prop.State);
                if (prop.State.Equals("True"))
                    it.SubItems.Add("连接正常", Color.Green, Color.Yellow, new Font("Times New Roman", 12));
                else
                    it.SubItems.Add("网络异常", Color.Red, Color.Yellow, new Font("Times New Roman", 12));
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            button1.Enabled = false;
            pingBiz.stopIt = false;
            new Thread(pingBiz.ping).Start();

            timer1.Enabled = true;

        }

        private void button2_Click(object sender, EventArgs e)
        {
            button1.Enabled = true;
            pingBiz.stopIt = true;

            timer1.Enabled = false;
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            pingBiz.stopIt = true;
            timer1.Enabled = false;
            sigList = null;
            pingBiz = null;
            this.Dispose();

        }

        private void listView1_SelectedIndexChanged(object sender, EventArgs e)
        {

        }
    }
}
