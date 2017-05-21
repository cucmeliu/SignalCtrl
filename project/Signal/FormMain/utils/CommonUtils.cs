using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FormMain.utils
{
    class CommonUtils
    {
        //                                       1               4         5
        public static String[] SigType = { "", "旗骏", "", "", "易华录", "鑫通" };
        public static String[] Dirs = { "", "东", "南", "西", "北" };

        public static Option LoadOption()
        {
            String CUR_DIR = System.AppDomain.CurrentDomain.SetupInformation.ApplicationBase;
            string confFile = CUR_DIR + "/CONFIG.ini";
            Option option = new Option();
            option.AutoConn = Boolean.Parse(IniFileOperator.ReadIniData("OPTION", "AUTOCONN", "true", confFile));
            option.SigSwitchPath = IniFileOperator.ReadIniData("OPTION", "SIGPATH", "", confFile);
            option.XtAddress = IniFileOperator.ReadIniData("OPTION", "ADDRESS", "", confFile);///

            return option;
        }

        public static bool Ping(string ip)
        {
            System.Net.NetworkInformation.Ping p = new System.Net.NetworkInformation.Ping();
            System.Net.NetworkInformation.PingOptions options = new System.Net.NetworkInformation.PingOptions();
            options.DontFragment = true;
            string data = "Test Data!";
            byte[] buffer = Encoding.ASCII.GetBytes(data);
            int timeout = 1000; // Timeout 时间，单位：毫秒
            System.Net.NetworkInformation.PingReply reply = p.Send(ip, timeout, buffer, options);
            if (reply.Status == System.Net.NetworkInformation.IPStatus.Success)
                return true;
            else
                return false;
        }
    }
}
