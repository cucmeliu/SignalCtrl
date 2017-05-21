using MyTCPSocket4EHL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;

namespace FormMain.C
{
    class EHLController : SignalController
    {

        private TCPClientSocket4EHL tcpClientSocket1;

        public EHLController(SignalProp prop) : base(prop)
        {
            tcpClientSocket1 = new TCPClientSocket4EHL();
            tcpClientSocket1.Breaken = true;
            tcpClientSocket1.BreakSec = 5;
            tcpClientSocket1.DataArrival += new TCPClientSocket4EHL.DataArrivalEventHandler(tcpClientSocket1_DataArrival);
        }

        public override bool closeSpecDuty()
        {
            bool rst = false;
            tcpClientSocket1.SendCmdByStr(prop.Ip, "EHL_Open", 0x00, 0x00);
            rst = tcpClientSocket1.SendCmdByStr(prop.Ip, "EHL_VIPOut", 0x00, 0x00);
            tcpClientSocket1.SendCmdByStr(prop.Ip, "EHL_Close", 0x00, 0x00);

            return rst;
        }

        public override string getState()
        {
            return prop.State;
        }

        public override bool openSpecDuty(byte dirFrom, byte dirTo)
        {
            bool rst = false;
            tcpClientSocket1.SendCmdByStr(prop.Ip, "EHL_Open", 0x00, 0x00);
            rst = tcpClientSocket1.SendCmdByStr(prop.Ip, "EHL_VIPIn",
                parseFromDir(dirFrom), parseToDir(dirTo));

            tcpClientSocket1.SendCmdByStr(prop.Ip, "EHL_Close", 0x00, 0x00);

            return rst;
        }

        private void tcpClientSocket1_DataArrival(byte[] eData, System.Net.EndPoint ePoint, string eType)      //    01=Red  11=Yellow  10=Green  else=Black  北东南西，行右直左
        {
            string msg = eType + " =  ";
            for (int i = 0; i < eData.Length; i++)
                msg = msg + Convert.ToString(eData[i], 16).ToUpper() + "H ";

            //  获取IP地址
            string RcvIpInfo = ePoint.ToString();
            int IpStrLen = RcvIpInfo.IndexOf(":");
            RcvIpInfo = RcvIpInfo.Substring(0, IpStrLen);

            if (eData.Length == 2)
            {
                if ((eData[0] == 0xFF) && (eData[1] == 0x01))     //    FF  01  灯态
                {
                    string[] uStr = new string[19];
                    for (int i = 3; i < 19; i++)
                        uStr[i] = eType.Substring((i - 3) * 2, 2);

                    prop.State = buidState(uStr);
                    //UpDataTable(RcvIpInfo, uStr);
                }
                else if ((eData[0] == 0xFF) && (eData[1] == 0x00))     //    FF  00  一般状态
                {
                }
                else if ((eData[0] == 0xFF) && (eData[1] == 0x02))     //    FF  02  一般错误
                {
                }
                else if ((eData[0] == 0xFF) && (eData[1] == 0x03))     //    FF  03  时间
                {
                }
                else if ((eData[0] == 0xFF) && (eData[1] == 0x04))     //    FF  04  控制策略 40H
                {
                }
                else if ((eData[0] == 0xFF) && (eData[1] == 0x05))     //    FF  05  控制策略 42H
                {
                }
                else if ((eData[0] == 0xFF) && (eData[1] == 0x06))     //    FF  06  VIP控制策略 45H
                {
                }
                else if ((eData[0] == 0xFF) && (eData[1] == 0x07))     //    FF  07  VIP控制通知 46H
                {
                }
                else if ((eData[0] == 0xFF) && (eData[1] == 0x08))     //    FF  08  Other ExeComData
                {
                }
                else if ((eData[0] == 0xFF) && (eData[1] == 0x09))     //    FF  09  设备编号
                {
                }
                else if ((eData[0] == 0xFF) && (eData[1] == 0x0A))     //    FF  0A  灯态回送周期
                {
                }
                else if ((eData[0] == 0xAA) && (eData[1] == 0xDD))     //    AA  DD  Data OK
                {
                }
                else if ((eData[0] == 0xAA) && (eData[1] == 0xEE))     //    AA  EE  Data Error
                {
                }
                else
                {
                }
            }
        }

        private string buidState(string[] uStr)
        {
            // 这里将接收到由”0””1”组成的长度32的字符串串，分为4段（4段分别表示北东南西，每段长度8），
            // 每段的8个字符再分4小段（4小段分别表示行、右、直、左，每小段长度2），
            // 每2个字符表示一个颜色：01=Red  11=Yellow  10=Green  else=Black
            //--> 需按成按盾华协议的: 1东/2南/3西/4北 // 及 0无/红1/黄2/绿3  // 及 直1/左2/右3/行4

            string ret = "";
            //将字符串数组转为字符串
            String ustr = string.Join("", uStr);
            Console.WriteLine("----ustr=" + ustr);
            //将易华录 北东南西 转为盾华 东南西北
            String dirstr1 = ustr.Substring(0, 8);
            String dirstr2 = ustr.Substring(ustr.Length - 24, 24);
            String dirstr3 = dirstr1 + dirstr2;
            Console.WriteLine("----dir convert ustr=" + dirstr3);
            // 11 10 01 11   10 11 11 01   01 11 10 10   01 10 11 11
            //将易华录的灯色字符转为盾华灯色字符  01=>1 11=>2 10=>3 else=>0
            int i = 0, j = 0, k = 0, m = 0;
            int[] array = new int[16] { -4, -4, -4, -4, -4, -4, -4, -4, -4, -4, -4, -4, -4, -4, -4, -4 };
            for (i = 0, j = 0; i < 31; i += 2, j++)
            {
                if (dirstr3[i].ToString().Equals("0") && dirstr3[i + 1].ToString().Equals("1"))
                    array[j] = 1;
                else if (dirstr3[i].ToString().Equals("1") && dirstr3[i + 1].ToString().Equals("1"))
                    array[j] = 2;
                else if (dirstr3[i].ToString().Equals("1") && dirstr3[i + 1].ToString().Equals("0"))
                    array[j] = 3;
                else
                    array[j] = 0;
            }
            for (k = 0; k < 16; k++)
            {
                ret += array[k].ToString();
            }
            Console.WriteLine("----EHL two bit => dunhua one bit :" + ret);
            //将易华录行、右、直、左转为盾华直、左、右、行
            for (m = 0; m < 15; m += 4)
            {
                swap.SwapChar(ref ret, m, m + 2);
                swap.SwapChar(ref ret, m + 1, m + 3);
                swap.SwapChar(ref ret, m + 2, m + 3);
            }
            return ret;
        }

        private byte parseFromDir(byte dir)
        {
            // direction, dir_to 
            //数据库中方向定义
            //1: 东 2：南， 3 西， 4 北
            // 易华录方向定义
            // 东 南 西 北
            // 2  4  6  0
            return (byte)((dir * 2) % 8);

        }

        //数据库方向转为易华录方向（去车的方向）
        private byte parseToDir(byte dir)
        {
            // direction, dir_to 
            //数据库中方向定义
            //1: 东 2：南， 3 西， 4 北
            // 易华录方向定义
            // 东 南 西 北
            // 6  0  2  4
            return (byte)((dir * 2 + 4) % 8);

        }

        private bool isConnected()
        {
            return !tcpClientSocket1.GetStatus().Equals("");
        }

        public override bool connect()
        {
            if (!isConnected())
            {
                if (tcpClientSocket1.Connect(prop.Ip) > 0)
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
