using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;

/// <summary>
/// 信号机工厂类
/// 通过信号机属性，创建出对应的信号机对象
/// 同时提供一些信号机相关的通用方法
/// Designed by: leo.liu, email cucme@163.com
/// </summary>
namespace FormMain.C
{
    class SignalFactory
    {
        /// <summary>
        /// 工厂方法：根据信号机参数信息，生产出对应的信号机
        /// </summary>
        /// <param name="prop"></param>
        /// <returns></returns>
        public static SignalController buildController(SignalProp prop)
        {
            if (null == prop)
                return null;

            SignalController ctrl;
            switch (prop.Type)
            {
                // 旗骏
                case 1:
                    ctrl = new QJController(prop);
                    break;
                // 易华录
                case 4:
                    ctrl = new EHLController(prop);
                    break;
                // 鑫通
                case 5:
                    ctrl = new XTController(prop);
                    break;

                default:
                    ctrl = null;
                    break;
            }

            return ctrl;
        }

        /// <summary>
        /// 解析信号机在界面上的角度
        /// </summary>
        /// <param name="angList"></param>
        /// <param name="sigList"></param>
        public static void parseSigAngList(List<SignalAng> angList, List<SignalProp> sigList)
        {
            foreach (SignalAng a in angList)
            {
                SignalProp prop = getSigProp(a.LampId, sigList);
                if (null == prop)
                {
                    prop = new SignalProp();
                    prop.SignalID = a.LampId;
                    sigList.Add(prop);
                }
                prop.Angle[0] = a.EAng;
                prop.Angle[1] = a.SAng;
                prop.Angle[2] = a.WAng;
                prop.Angle[3] = a.NAng;
            }
        }

        /// <summary>
        /// 解析信号机在界面上的位置
        /// </summary>
        /// <param name="posList"></param>
        /// <param name="sigList"></param>
        public static void parseSigPosList(List<SignalPos> posList, List<SignalProp> sigList)
        {
            foreach (SignalPos p in posList)
            {
                SignalProp prop = getSigProp(p.LampId, sigList);
                if (null == prop)
                {
                    prop = new SignalProp();
                    prop.SignalID = p.LampId;
                    sigList.Add(prop);
                }
                int idx = (p.Compass - 1) * 4 + p.Dir - 1;
                //Trace.WriteLine("parse prop: " + idx + ";  " + prop.ToString());
                prop.Pos[(p.Compass - 1) * 4 + p.Dir - 1] = new Point(p.X, p.Y);
            }
        }

        public static SignalProp getSigPropByName(string signalName, List<SignalProp> sigList)
        {
            SignalProp prop = null;// new SignalProp();

            foreach (SignalProp p in sigList)
            {
                if (signalName.Equals(p.SignalName))
                {
                    prop = p;
                    break;
                }
            }
            return prop;
        }

        public static SignalProp getSigProp(string signalId, List<SignalProp> sigList)
        {
            SignalProp prop = null;// new SignalProp();

            foreach (SignalProp p in sigList)
            {
                if (signalId.Equals(p.SignalID.ToUpper()))
                {
                    prop = p;
                    break;
                }
            }
            return prop;
        }

    }
}
