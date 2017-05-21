using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FormMain
{
    class DBConf
    {
        string serverip;

        public string Serverip
        {
            get { return serverip; }
            set { serverip = value; }
        }
        string dbName;

        public string DbName
        {
            get { return dbName; }
            set { dbName = value; }
        }
        string userid;

        public string Userid
        {
            get { return userid; }
            set { userid = value; }
        }
        string passwd;

        public string Passwd
        {
            get { return passwd; }
            set { passwd = value; }
        }
        string charset;

        public string Charset
        {
            get { return charset; }
            set { charset = value; }
        }
        string timeout;

        public string Timeout
        {
            get { return timeout; }
            set { timeout = value; }
        }

        public string getConnStr()
        {
            return   "Max Pool Size=512; database=" + dbName + ";server=" + serverip + ";user id=" + userid + "; pwd=" + passwd + ";charset=" + charset + ";Connect Timeout=" + timeout + "";

        }

        
    }
}
