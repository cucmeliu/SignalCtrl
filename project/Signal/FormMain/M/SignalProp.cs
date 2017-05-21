using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FormMain
{
    class SignalProp
    {
        private string signalID;
        private string signalName;
        private string ip;
        private string state;
        private byte type;
        private byte specState;
        private byte dirFrom;
        private byte dirTo;
        double[] angle = new double[4];
        Point[] pos = new Point[16];

        public SignalProp()
        {
            signalID = "";
            signalName = "";
            ip = "172.0.0.1";
            state = "0000000000000000";
            type = 0;
        }

        /// <summary>
        /// 信号机ID
        /// </summary>
        public string SignalID
        {
            get { return signalID; }
            set { signalID = value; }
        }
        
        /// <summary>
        /// 信号机名称
        /// </summary>
        public string SignalName
        {
            get { return signalName; }
            set { signalName = value; }
        }

        /// <summary>
        /// 信号机在显示图上的东南西北位置       
        /// (相对)const int BASE_WIDTH = 1936; 
        /// const int BASE_HEIGHT = 1056;
        /// </summary>
        public Point[] Pos
        {
            get { return pos; }
            set { pos = value; }
        }

        /// <summary>
        /// 信号机在显示图上的东南西北角度
        /// 南向北为0度
        /// </summary>
        public double[] Angle
        {
            get { return angle; }
            set { angle = value; }
        }

        /// <summary>
        /// 信号机当前状态
        /// </summary>
        public string State
        {
            get { return state; }
            set { state = value; }
        }

        /// <summary>
        /// 信号机IP地址
        /// </summary>
        public string Ip
        {
            get
            {
                return ip;
            }

            set
            {
                ip = value;
            }
        }

        /// <summary>
        /// 信号机类型: 1旗骏,2卡口,3虚拟信号机,  4易华录,5鑫通
        /// </summary>
        public byte Type
        {
            get
            {
                return type;
            }

            set
            {
                type = value;
            }
        }

        /// <summary>
        /// 特勤状态: 0正常,1特勤,2关闭特勤
        /// </summary>
        public byte SpecState
        {
            get
            {
                return specState;
            }

            set
            {
                specState = value;
            }
        }

        /// <summary>
        /// 特勤方向,从
        /// </summary>
        public byte DirFrom
        {
            get
            {
                return dirFrom;
            }

            set
            {
                dirFrom = value;
            }
        }

        /// <summary>
        /// 特勤方向,到
        /// </summary>
        public byte DirTo
        {
            get
            {
                return dirTo;
            }

            set
            {
                dirTo = value;
            }
        }

        public override string ToString()
        {
            String rst = "";
            rst += " ID: " + signalID
                + ", Name: " + signalName
                + ", IP: " + ip
                + ", Type: " + type;


            rst += ", pos:\n ";
            for (int i = 0; i < pos.Length; i++)
            {
                rst += pos[i].ToString();
            }

            rst += ", ang: \n";
            for (int i = 0; i < angle.Length; i++)
            {
                rst += angle[i].ToString() + ", ";
            }

            return rst;

        }

    }
}
