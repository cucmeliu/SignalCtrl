using FormMain.utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

/// <summary>
/// 各种信号机的父类，抽象类
/// 需接入本平台的所有信号机均继承自本类，实现其中的抽象方法
/// Designed by: leo.liu, email cucme@163.com
/// </summary>

namespace FormMain.C
{
    abstract class SignalController
    {
        protected SignalProp prop;

        internal SignalProp Prop
        {
            get
            {
                return prop;
            }
            set
            {
                prop = value;
            }
        }

        public SignalController(SignalProp prop) {
            this.prop = prop;
        }

        //protected bool Ping(string ip)
        //{
        //    System.Net.NetworkInformation.Ping p = new System.Net.NetworkInformation.Ping();
        //    System.Net.NetworkInformation.PingOptions options = new System.Net.NetworkInformation.PingOptions();
        //    options.DontFragment = true;
        //    string data = "Test Data!";
        //    byte[] buffer = Encoding.ASCII.GetBytes(data);
        //    int timeout = 1000; // Timeout 时间，单位：毫秒
        //    System.Net.NetworkInformation.PingReply reply = p.Send(ip, timeout, buffer, options);
        //    if (reply.Status == System.Net.NetworkInformation.IPStatus.Success)
        //        return true;
        //    else
        //        return false;
        //}

        /// <summary>
        /// 根据方向执行特勤
        /// 输入方向统一： 1东、2南、3西、4北
        /// 各自信号机根据自己的协议解析其方向
        /// </summary>
        /// <param name="dirFrom">车辆驶入路口的方向</param>
        /// <param name="dirTo">车辆驶出路口的方向</param>
        /// <returns>特勤执行是否成功：成功true，未成功false</returns>
        public abstract Boolean openSpecDuty(byte dirFrom, byte dirTo);

        /// <summary>
        /// 关闭特勤
        /// </summary>
        /// <returns>关闭特勤是否成功：成功true，未成功false</returns>
        public abstract Boolean closeSpecDuty();

        /// <summary>
        /// 取得当前信号机相位状态，
        /// </summary>
        /// <returns>以16位字符表示当前信号机状态，格式如下：
        /// 东           南           西           北
        /// 0  0  0  0  0  0  0  0  0  0  0  0  0  0  0  0
        /// 直 左 右 人 直 左 右 人 直 左 右 人 直 左 右 人
        /// /returns>
        public abstract string getState();

        /// <summary>
        /// 重新获取信号机状态，更新prop中的state属性
        /// </summary>
        public bool updateState() {
            if (null == (prop.State = getState()))
                return false;
            return true;
        }

        public Boolean getPingStatus()
        {
            return CommonUtils.Ping(prop.Ip);
        }

        public abstract string getStatus();

        public abstract bool connect();

    }
}
