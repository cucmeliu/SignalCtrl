using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.IO;
using System.Diagnostics;
using FormMain.utils;


namespace FormMain.C
{
    class XTController : SignalController
    {
        private String docommand_url = null;  
        private String gettrafficlights_url = null;  
        private String excutecontorl_url = null;

        public XTController(SignalProp prop): base(prop)
        {
            this.Prop = prop;
            Option option = CommonUtils.LoadOption();
            gettrafficlights_url = "http://" + option.XtAddress + "/jtd/cxf/rest/sigservice/gettrafficlights/signumber.json";//信号机编号:216 获取实时状态单次
            docommand_url = "http://" + prop.Ip + ":"+option.XtAddress + "/jtd/cxf/rest/sigservice/docommand/signumber/33.json";//信号机编号:216 执行黄闪命令 
            gettrafficlights_url = "http://" + prop.Ip + ":" + option.XtAddress + "/jtd/cxf/rest/sigservice/gettrafficlights/signumber.json";//信号机编号:216 获取实时状态单次
            excutecontorl_url = "http://" + prop.Ip + ":" + option.XtAddress + "/jtd/cxf/rest/sigservice/excutecontrol/signumber/controlmsg.json";//信号机编号:216 执行特勤控制
        }

        public override bool closeSpecDuty()
        {
            try
            {
                docommand_url = docommand_url.Replace("signumber", prop.SignalID);
                Trace.WriteLine(docommand_url);
                String backMsg = HttpGet(docommand_url, "");
                Trace.WriteLine(" backMsg:" + backMsg);
                if (backMsg.Equals("1"))
                {
                    Trace.WriteLine("close XtSpecDuty success..." + backMsg);
                    return true;
                }
                else
                {
                    Trace.WriteLine("close XtSpecDuty failed..." + backMsg);
                    return false;
                }
            }
            catch (Exception e)
            {
                Trace.WriteLine("close XtSpecDuty error..." + e.StackTrace);
                return false;
            }
            //throw new NotImplementedException();
        }

        static string SwapChar(ref string s, int p1, int p2)
        {
            string ret;

            if (p1 == p2) return s;

            if (p1 > p2) { int p = p1; p1 = p2; p2 = p; } //Swap p1,p2

            ret = s.Substring(0, p1) + s[p2];
            ret += s.Substring(p1 + 1, p2 - p1 - 1) + s[p1];
            ret += s.Substring(p2 + 1, s.Length - p2 - 1);
            return ret;
        }

        public override string getState()
        {
            string result1 = "";
            string result2 = "";
            string result3 = "";
            string result4 = "";
            string result5 = "";
            string result6 = "";
            string result7 = "";
            string result = "";
            try
            {
                if (!prop.SignalID.Equals(""))
                {
                    gettrafficlights_url = gettrafficlights_url.Replace("signumber", prop.SignalID);
                    Trace.WriteLine("url:"+gettrafficlights_url);

                    String backMsg = HttpGet(gettrafficlights_url, "");
                    Trace.WriteLine("getState ok");
                    Trace.WriteLine(" backMsg xt:" + backMsg);
                    ///去除,[,]
                    result1 = backMsg.Replace(",","").Replace("[","").Replace("]","");
                    Trace.WriteLine(" backMsg xt:" + result1);
                    //将返回值里的1与3数字对调
                    result2 = result1.Replace("1", "4");
                    result3 = result2.Replace("3", "1");
                    result = result3.Replace("4", "3");
                    Trace.WriteLine(" backMsg first:" + result);
                    //将鑫通返回值16个数位中1,2位；5，6位；9，10位；13，14位相互对调成盾华的直左右行
                    result4 = swap.SwapChar(ref result,0,1);
                    result5 = swap.SwapChar(ref result4, 4, 5);
                    result6 = swap.SwapChar(ref result5, 8, 9);
                    result7 = swap.SwapChar(ref result6, 12, 13);
                    Trace.WriteLine(" backMsg later:" + result7);
                }
                return result7;
            }
            catch (Exception e)
            {
                Trace.WriteLine("get XtState failed..." + e.StackTrace);
                return null;
            }
            //throw new NotImplementedException();
        }

        public override bool openSpecDuty(byte dirFrom, byte dirTo)
        {
            String GreenTime = "6";
            String YellowTime = "1";
            String RedTime = "1";
            String SpecTime = GreenTime + "," + RedTime + "," + YellowTime;
            int resType = -1;
            try
            {
                int type = (int)(dirTo - dirFrom);
                if (type == 1 || type == -3)
                    resType = 0;
                else if (type == 2 || type == -2)
                    resType = 1;
                else if (type == -1 || type == 3)
                    resType = 2;
                else
                    resType = -1;

                if (resType == -1)
                    return false;

                int[,] array = new int[,] { { 3, 3, 3, 3 }, { 3, 3, 3, 3 }, { 3, 3, 3, 3 }, { 3, 3, 3, 3 } };
                array[dirFrom-1,resType] = 1;

                int i = 0, j = 0;
                String res = "";
                for (i = 0; i < 4; i++)
                    for (j = 0; j < 4; j++)
                        res = res + array[i, j].ToString() + ",";
                //////////////////////////////////////使用局部变量，否则特勤只认第一次方向
                String cmd = String.Copy(excutecontorl_url);
                cmd = cmd.Replace("signumber", prop.SignalID).Replace("controlmsg", res + SpecTime);
                //excutecontorl_url = excutecontorl_url.Replace("signumber", prop.SignalID).Replace("controlmsg", res + SpecTime);
                //////////////////////////////////////

                Trace.WriteLine(cmd);
                String backMsg = HttpGet(cmd, "");
                Trace.WriteLine(backMsg);//将连接成功的字符串显示在日志中
                if (backMsg.Equals("1"))
                {
                    Trace.WriteLine("open XtSpecDuty success..." + backMsg);
                    return true;
                }
                else
                {
                    Trace.WriteLine("open XtSpecDuty failed..." + backMsg);
                    return false;
                }
            }
            catch (Exception e)
            {
                Trace.WriteLine("open XtSpecDuty error..." + e.StackTrace);
                return false;
            }
            //throw new NotImplementedException();
        }

        //public override void updateState()
        //{
        //    //throw new NotImplementedException();
        //}

        //控制鑫通信号机httpget功能函数
        private string HttpGet(string Url, string postDataStr)
        {
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(Url + (postDataStr == "" ? "" : "?") + postDataStr);
            request.Method = "GET";
            request.ContentType = "text/html;charset=UTF-8";

            HttpWebResponse response = (HttpWebResponse)request.GetResponse();
            Stream myResponseStream = response.GetResponseStream();
            StreamReader myStreamReader = new StreamReader(myResponseStream, Encoding.GetEncoding("utf-8"));
            string retString = myStreamReader.ReadToEnd();
            myStreamReader.Close();
            myResponseStream.Close();

            return retString;
        }

        public override bool connect()
        {
            return true;
        }

        public override string getStatus()
        {
            return "unknown";
        }
    }
}
