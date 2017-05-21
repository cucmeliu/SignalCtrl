using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace FormMain
{
    class SignalDao
    {
        //String DB_CONN = "";
        String CUR_DIR = System.AppDomain.CurrentDomain.SetupInformation.ApplicationBase;
        MySql.Data.MySqlClient.MySqlConnection myConn = null;

        private static SignalDao dao = null;

        private SignalDao()
        {
            string DB_CONN = loadDBConfig();
            myConn = new MySql.Data.MySqlClient.MySqlConnection(DB_CONN);
            myConn.Open();
        }

        public static SignalDao getInstance()
        {
            if (dao == null)
            {
                dao = new SignalDao();
            }
            return dao;
        }

        public SignalProp getCurSigProp()
        {
            SignalProp prop = null;
            String sql = " SELECT s.lampId, s.signalName, s.type, s.state, c.state as specState, direction, dir_to, s.ip FROM cursignal c LEFT JOIN signal s ON c.lampId=s.lampId AND c.type=1 ";
            MySqlCommand cmd = new MySql.Data.MySqlClient.MySqlCommand(sql, myConn);

            System.Data.Common.DbDataReader reader = cmd.ExecuteReader();
            try
            {
                if (reader.Read())
                {
                    prop = new SignalProp();
                    prop.SignalID = reader.GetString(0).ToUpper();
                    prop.SignalName = DBStringToNormal(reader.GetString(1));
                    prop.Type = reader.IsDBNull(2)? (byte) 0 : (byte)reader.GetInt16(2);
                    prop.State = reader.GetString(3);
                    prop.SpecState = reader.IsDBNull(4) ? (byte)0 : (byte)reader.GetInt16(4);
                    prop.DirFrom = reader.IsDBNull(5) ? (byte)0 : (byte)reader.GetInt16(5);
                    prop.DirTo = reader.IsDBNull(6) ? (byte)0 : (byte)reader.GetInt16(6);
                    prop.Ip = reader.GetString(7);
                }
                reader.Close();
            }
            catch (Exception e)
            {
                reader.Close();
                Trace.TraceError("get cur signal prop " + e.StackTrace);
            }

            return prop;

        }

        /// <summary>
        ///  更新cursignal的特勤状态
        /// </summary>
        /// <param name="signalId">信号机编号</param>
        /// <param name="state">状态:0正常,1特勤,2关闭特勤,</param>
        public int updateCurSigState(string signalId, byte state)
        {
            String sql = " Update cursignal SET state=? WHERE signalId=?";
            MySqlCommand cmd = new MySql.Data.MySqlClient.MySqlCommand(sql, myConn);
            cmd.Parameters[0].Value = signalId;
            cmd.Parameters[1].Value = state;
            return cmd.ExecuteNonQuery();
        }

        public string getSigState()
        {
            return null;
        }

        public List<SignalProp> getSignalList()
        {
            List<SignalProp> list = new List<SignalProp>();

            String sql = " SELECT lampId, signalName, type, ip  FROM signal WHERE type IN (1, 4, 5) ";
            MySqlCommand cmd = new MySql.Data.MySqlClient.MySqlCommand(sql, myConn);
            System.Data.Common.DbDataReader reader = cmd.ExecuteReader();

            while (reader.Read())
            {
                SignalProp prop = new SignalProp();

                prop.SignalID = reader.GetString(0).Trim().ToUpper(); //.Replace('-', '_')
                prop.SignalName = DBStringToNormal(reader.GetString(1).Trim());
                prop.Type = reader.IsDBNull(2) ? (byte)1 : (byte)reader.GetInt32(2);
                prop.Ip = reader.IsDBNull(3) ? "" : reader.GetString(3).Trim();

                list.Add(prop);

            }
            reader.Close();

            return list;
        }

        public List<SignalPos> getPosList()
        {
            // position// position
            List<SignalPos> posList = new List<SignalPos>();
                        
            String sql = "SELECT lampId, compass, dir, x, y  FROM signalPos";
            MySqlCommand cmd = new MySql.Data.MySqlClient.MySqlCommand(sql, myConn);
            System.Data.Common.DbDataReader reader = cmd.ExecuteReader();
                        
            while (reader.Read())
            {
                SignalPos pos = new SignalPos();
                pos.LampId = reader.GetString(0).Trim().ToUpper(); //.Replace('-', '_')
                pos.Compass = reader.IsDBNull(1) ? 1 : reader.GetInt32(1);
                pos.Dir = reader.IsDBNull(2) ? 1 : reader.GetInt32(2);
                pos.X = reader.IsDBNull(3) ? 1 : reader.GetInt32(3);
                pos.Y = reader.IsDBNull(4) ? 1 : reader.GetInt32(4);
                posList.Add(pos);

            }
            reader.Close();

            return posList;

        }

        public List<SignalAng> getAngleList()
        {
            // angle
            List<SignalAng> angList = new List<SignalAng>();
            String sql = "SELECT lampId, eAng, sAng, wAng, nAng FROM signalAng ";
            MySqlCommand cmd = new MySql.Data.MySqlClient.MySqlCommand(sql, myConn);

            System.Data.Common.DbDataReader reader = cmd.ExecuteReader();
                        
            while (reader.Read())
            {
                SignalAng ang = new SignalAng();
                ang.LampId = reader.GetString(0).ToUpper().Trim();//.Replace('-', '_');
                ang.EAng = reader.GetInt32(1);
                ang.SAng = reader.GetInt32(2);
                ang.WAng = reader.GetInt32(3);
                ang.NAng = reader.GetInt32(4);

                angList.Add(ang);

            }
            reader.Close();
            return angList;
        }

        public ConnectionState getState()
        {
            return myConn.State;
        }

        private string loadDBConfig()
        {
            DBConf dbConf = new DBConf();
            string confFile = CUR_DIR + "/CONFIG.ini";

            dbConf.Serverip = IniFileOperator.ReadIniData("DBCONFIG", "SERVERIP", "172.0.0.1", confFile);
            dbConf.DbName = IniFileOperator.ReadIniData("DBCONFIG", "DBNAME", "signal", confFile);
            dbConf.Userid = IniFileOperator.ReadIniData("DBCONFIG", "USERID", "video", confFile);
            dbConf.Passwd = IniFileOperator.ReadIniData("DBCONFIG", "PASSWD", "video", confFile);
            dbConf.Charset = IniFileOperator.ReadIniData("DBCONFIG", "CHARSET", "utf8", confFile);
            dbConf.Timeout = IniFileOperator.ReadIniData("DBCONFIG", "TIMEOUT", "60", confFile);

            //string connstr = "Max Pool Size=512; database=" + dbname + ";server=" + dbip + ";user id=" + userid + "; pwd=" + passwd + ";charset=" + charset + ";Connect Timeout=" + timeout + "";
            return dbConf.getConnStr();
        }

        /// <summary>
        /// 解决取数据库乱码问题
        /// </summary>
        /// <param name="dbStr"></param>
        /// <returns></returns>
        private string DBStringToNormal(string dbStr)
        {
            byte[] str = new byte[dbStr.Length];
            for (int i = 0; i < dbStr.Length; ++i)
                str[i] = (byte)(dbStr[i]);
            return System.Text.Encoding.Default.GetString(str, 0, dbStr.Length);
        }

        /****************************************************
         * 写入数据库时进行转换,sql为含中文（或不含）的sql语句
        ****************************************************/
        public string GB2312_ISO8859(string sql)
        {
            //声明字符集  
            System.Text.Encoding iso8859, gb2312;
            iso8859 = System.Text.Encoding.GetEncoding("iso8859-1");
            gb2312 = System.Text.Encoding.GetEncoding("gb2312");
            byte[] gb;
            gb = gb2312.GetBytes(sql);
            //返回转换后的字符  
            return iso8859.GetString(gb);
        }
        /****************************************************
         * 读出时进行转换
        ****************************************************/
        public string ISO8859_GB2312(string value)
        {
            //声明字符集  
            System.Text.Encoding iso8859, gb2312;
            iso8859 = System.Text.Encoding.GetEncoding("iso8859-1");
            gb2312 = System.Text.Encoding.GetEncoding("gb2312");
            byte[] iso;
            iso = iso8859.GetBytes(value);
            //返回转换后的字符  
            return gb2312.GetString(iso);
        }


    }
}
