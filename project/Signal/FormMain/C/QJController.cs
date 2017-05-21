using MyTCPSocket;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Runtime.InteropServices;
using System.Net.Sockets;
using System.Threading;
using System.Diagnostics;

namespace FormMain.C
{
    class QJController : SignalController
    {

        private TCPClientSocket tcpClientSocket1;

        public QJController(SignalProp prop) : base(prop)
        {
            tcpClientSocket1 = new TCPClientSocket();
            tcpClientSocket1.Breaken = true;
            tcpClientSocket1.BreakSec = 5;
            tcpClientSocket1.DataArrival += new TCPClientSocket.DataArrivalEventHandler(tcpClientSocket1_DataArrival);
            
        }

        public override bool closeSpecDuty()
        {
            try
            {
                string CmdStr = "MCM_CLOSE";
                return tcpClientSocket1.SendCmdByStr(prop.Ip, CmdStr);
            }
            catch (Exception e)
            {
                Trace.WriteLine(e);
                return false;
            }
        }

        public override string getState()
        {
            return prop.State;
            //throw new NotImplementedException();
        }

        public override bool openSpecDuty(byte dirFrom, byte dirTo)
        {
            // 5/6/7/8阶段分别设置为东/南/西/北绿灯状态
            //return MySendData("MCM_STAGE" + (4 + dirFrom), -1);
            string CmdStr = "MCM_STAGE" + (4 + dirFrom);
            try
            {
                for (int i = 0; i < 5; i++)
                {
                    tcpClientSocket1.SendCmdByStr(prop.Ip, CmdStr);
                    Thread.Sleep(1000);
                }

                return tcpClientSocket1.SendCmdByStr(prop.Ip, CmdStr);
            }
            catch (Exception e)
            {
                Trace.WriteLine(e);
                return false;
            }
        }

        //  从EndPoint提取IP地址字符串 
        private string EndPointToStr(System.Net.EndPoint ePoint)
        {
            string IPPoint = ePoint.ToString();
            string IPStr = "";
            if (IPPoint.IndexOf(":") >= 0)
            {
                IPStr = IPPoint.Substring(0, IPPoint.IndexOf(":"));
            }
            return IPStr;
        }

        private void tcpClientSocket1_DataArrival(byte[] eData, EndPoint ePoint, string eType)
        {
            if (eData.Length == 98)
            {
                string msg = eType + "=";

                byte[] uStr = new byte[32];
                for (int i=0; i<32; i++)
                {
                    uStr[i] = eData[i + 7];
                }

                prop.State = buildState(uStr);
            } //  在有足够98字节内

        }

        private string buildState(byte[] uStr)
        {
            String ret = "";
            int n = 0;
            for (n = 0; n < 32; n++)
            {
                ret += uStr[n] + "";
            }
            return ret;
        }

        private bool isConnected()
        {
            return !tcpClientSocket1.GetStatus().Equals("");
        }

        public override bool connect()
        {
            if (!isConnected())
            {
                if (tcpClientSocket1.Connect(prop.Ip) >= 0)
                    return true;
                else
                    return false;
            }
            return true;
        }

        public override string getStatus()
        {
            return tcpClientSocket1.GetStatus();
        }
    }
}
