using FormMain;
using FormMain.utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SignalStatus
{

    class PingBiz
    {
        private List<SignalProp> sigList = new List<SignalProp>();
        public bool stopIt = false;

        public PingBiz(List<SignalProp> list)
        {
            sigList = list;
        }
        
        public void ping()
        {
            while (!stopIt)
            {
                foreach (SignalProp prop in sigList)
                {
                    prop.State = CommonUtils.Ping(prop.Ip) + "";
                    //if (CommonUtils.Ping(prop.Ip))
                    //    prop.State = "连接正常";
                    //else
                    //    prop.State = "网络异常";
                }
                Thread.Sleep(3000);
            }
        }
    }
}
