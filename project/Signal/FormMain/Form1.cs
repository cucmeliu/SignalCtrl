using FormMain.C;
using FormMain.utils;
using FormMain.V;
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows.Forms;

namespace FormMain
{
    public partial class MainForm : Form
    {
        public MainForm()
        {
            InitializeComponent();
        }

        [DllImport("kernel32.dll")]
        public static extern int WinExec(string exeName, int operType);

        const int SIGNAL_COUNT = 6; //num of signal
        const int STATE_COUNT = 16; //num of state type for each signal
        const int COLOR_COUNT = 4; // num of color for each lamp. 0: none, 1: red, 2: yel, 3: green
        const int DIR_COUNT = 4; // num of dir count of each lane.  1: strait, 2: left, 3: right, 4: walk

        const int BASE_WIDTH = 1936;
        const int BASE_HEIGHT = 1056;
        //
        SignalDao signalDao = SignalDao.getInstance();
        String CUR_DIR = System.AppDomain.CurrentDomain.SetupInformation.ApplicationBase; //Directory.GetCurrentDirectory();

        //SignalProp curManSigProp = new SignalProp(); // 当前手动选中的信号机ID
        SignalController curSignal;  // 当前选择的信号机

        List<SignalProp> sigList = new List<SignalProp>();  // 所有信号机属性的集合
        byte specPoly = 0; // 特勤策略：0手动；1自动

        Boolean PicPointMode = false;  // 选点模式

        Image img0_1;// = Properties.Resources.arr0_1;  // none, strait
        Image img0_2;// = Properties.Resources.arr0_2;  // none, left
        Image img0_3;// = Properties.Resources.arr0_3;  // none, right
        //Image img0_3 = Properties.Resources.arr0_3;  // none, walk

        Image img1_1;// = Properties.Resources.arr1_1;  // red, strait
        Image img1_2;// = Properties.Resources.arr1_2;  // red, left
        Image img1_3;// = Properties.Resources.arr1_3; // red, right

        Image img2_1;// = Properties.Resources.arr2_1; // yel
        Image img2_2;// = Properties.Resources.arr2_2; //
        Image img2_3;// = Properties.Resources.arr2_3; //

        Image img3_1;// = Properties.Resources.arr3_1; // green
        Image img3_2;// = Properties.Resources.arr3_2; //
        Image img3_3;// = Properties.Resources.arr3_3; //

        private Boolean initFromDB()
        {
            try
            {
                //读数据库方式 
                initSigListByDB();
                return true;
            }
            catch (Exception e)
            {
                Trace.TraceError(" init from db failed...\n" + e.StackTrace);
                lblSignalName.Text = "连接数据库失败！";
                MessageBox.Show("连接数据库失败,请重试");
                return false;
            }
        }


        private void loadArrow()
        {
            string arr_Path = System.AppDomain.CurrentDomain.SetupInformation.ApplicationBase + "arrow\\";
            try
            {
                img0_1 = System.Drawing.Image.FromFile(arr_Path + "arr0_1.png");
                img0_2 = System.Drawing.Image.FromFile(arr_Path + "arr0_2.png");
                img0_3 = System.Drawing.Image.FromFile(arr_Path + "arr0_3.png");

                img1_1 = System.Drawing.Image.FromFile(arr_Path + "arr1_1.png");
                img1_2 = System.Drawing.Image.FromFile(arr_Path + "arr1_2.png");
                img1_3 = System.Drawing.Image.FromFile(arr_Path + "arr1_3.png");

                img2_1 = System.Drawing.Image.FromFile(arr_Path + "arr2_1.png");
                img2_2 = System.Drawing.Image.FromFile(arr_Path + "arr2_2.png");
                img2_3 = System.Drawing.Image.FromFile(arr_Path + "arr2_3.png");

                img3_1 = System.Drawing.Image.FromFile(arr_Path + "arr3_1.png");
                img3_2 = System.Drawing.Image.FromFile(arr_Path + "arr3_2.png");
                img3_3 = System.Drawing.Image.FromFile(arr_Path + "arr3_3.png");


            }
            catch (Exception e)
            {
                Trace.TraceError("加载箭头失败: " + e.StackTrace);
            }
        }

        private void initSigListByDB()
        {
            Trace.WriteLine("init sig list by db");

            sigList = signalDao.getSignalList();
            // position
            SignalFactory.parseSigPosList(signalDao.getPosList(), sigList);
            // angle
            SignalFactory.parseSigAngList(signalDao.getAngleList(), sigList);

            Trace.WriteLine("all signals are: ");

            foreach (SignalProp p in sigList)
            {
                Trace.WriteLine(p.ToString());
            }


            Trace.WriteLine("...init sig list");
        }


        //  根据控件名称找到一个控件
        private Control getCtrlByName(string controlName)
        {
            FieldInfo field = this.GetType().GetField(controlName, BindingFlags.Instance | BindingFlags.NonPublic);
            if (field == null) return null;
            return field.GetValue(this) as Control;
        }

        private void showPBs(bool isShow)
        {
            // 东/南/西/北
            for (int i = 0; i < 4; i++)
            {
                //直/左/右/人
                for (int j = 0; j < 4; j++)
                {
                    string str = "pb" + (i * 4 + j + 1);
                    PictureBox pb = (PictureBox)getCtrlByName(str);

                    if (pb != null)
                    {
                        pb.Visible = isShow;
                    }
                }
            }
        }

        private void showMap(string signal)
        {
            string Map_Path = System.AppDomain.CurrentDomain.SetupInformation.ApplicationBase + "map\\";
            try
            {
                pbMap.Image = System.Drawing.Image.FromFile(Map_Path + signal + ".png");
            }
            catch (Exception e)
            {
                pbMap.Image = System.Drawing.Image.FromFile(Map_Path + "default.png");
                Trace.TraceError("加载路口图 [" + signal + "] 失败: " + e.StackTrace);
            }

        }

        private void showState(SignalProp prop)
        {
            if (null == prop || prop.SignalID.Equals(""))
                return;

            String signal = prop.SignalID;

            if (null == curSignal || !signal.Equals(curSignal.Prop.SignalID.ToUpper()))
                showMap(signal);

            prop = SignalFactory.getSigProp(signal, sigList);

            int idx = 0;

            Trace.WriteLine(" showState: " + prop.ToString());

            try
            {
                // 东/南/西/北
                for (int i = 0; i < 4; i++)
                {
                    //直/左/右/人
                    for (int j = 0; j < 4; j++)
                    {
                        idx = i * 4 + j + 1;
                        string str = "pb" + idx;
                        PictureBox pb = (PictureBox)getCtrlByName(str);
                        //if (pb == null)
                        //    continue;

                        if (pb != null)
                        {
                            if (idx > prop.State.Length)
                                continue;

                            string c = prop.State.Substring(idx - 1, 1);
                            byte s = byte.Parse(c);
                            Point p = prop.Pos[idx - 1];
                            // Trace.WriteLine("  point: " + p.ToString() + " ang: " + prop.Angle[i]);
                            showAState(pb, p, prop.Angle[i], idx, s);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Trace.TraceError("showstate error: " + ex.StackTrace);
            }
        }

        private void showAState(PictureBox pb, Point p, double ang, int dir, byte state)
        {
            Point pp = new Point();
            pp.X = pbMap.Left + p.X * pbMap.Width / BASE_WIDTH;
            pp.Y = pbMap.Top + p.Y * pbMap.Height / BASE_HEIGHT;
            pb.Location = pp;

            pb.Image = ImageEx.GetRotateImage(getImageByDirAndState(state, dir), (float)ang);
            //if (pb.Image != null)
            //    System.Console.WriteLine("image:" + pb.Image.ToString());
        }


        /// <summary>
        /// 确定当前要显示的信号方向箭头
        /// </summary>
        /// <param name="state"></param>
        /// <param name="dir">方向：1东2南3西4北</param>
        /// <returns></returns>
        private Image getImageByDirAndState(byte state, int dir)
        {
            Image img = null;
            int d = (dir - 1) % 4 + 1;

            System.Console.WriteLine("state:" + state + ", dir:" + d);

            switch (state)
            {

                case 1:
                case 6:
                    // red
                    switch (d)
                    {
                        case 1:
                            // strait
                            img = img1_1;
                            break;
                        case 2:
                            // left
                            img = img1_2;
                            break;
                        case 3:
                            //right
                            img = img1_3;
                            break;
                        default:
                            //walk
                            //img = img1_0;
                            break;
                    }
                    break;
                case 2:
                case 7:
                    // yel
                    switch (d)
                    {
                        case 1:
                            // strait
                            img = img2_1;
                            break;
                        case 2:
                            // left
                            img = img2_2;
                            break;
                        case 3:
                            //right
                            img = img2_3;
                            break;
                        default:
                            //walk
                            break;
                    }
                    break;
                case 3:
                case 8:
                    // green
                    switch (d)
                    {
                        case 1:
                            // strait
                            img = img3_1;
                            break;
                        case 2:
                            // left
                            img = img3_2;
                            break;
                        case 3:
                            //right
                            img = img3_3;
                            break;
                        default:
                            //walk
                            break;
                    }
                    break;
                default:
                    // 0 light close
                    switch (d)
                    {
                        case 1:
                            // strait
                            img = img0_1;
                            break;
                        case 2:
                            // left
                            img = img0_2;
                            break;
                        case 3:
                            //right
                            img = img0_3;
                            break;
                        default:
                            //walk
                            break;
                    }
                    break;
            }

            return img;
        }


        private void Form1_Load(object sender, EventArgs e)
        {
            Trace.WriteLine(" =======================================================");
            Trace.WriteLine(" signal program start ...");
            Trace.WriteLine(" form loading...");

            lblSignalName.Text = "加载初始路口图...";
            Trace.WriteLine(" loading default map...");
            pbMap.Image = System.Drawing.Image.FromFile(System.AppDomain.CurrentDomain.SetupInformation.ApplicationBase + "map\\default.png");

            lblSignalName.Text = "加载交通指示灯...";
            Trace.WriteLine(" loading arrows...");
            loadArrow();

            // 先不可见所有
            showPBs(false);

            Trace.WriteLine(" loading options...");
            Option option = LoadOption();
            Trace.WriteLine(option.ToString());

            lblSignalName.Text = "连接数据库...";
            Trace.WriteLine(" init from db...");

            if (initFromDB())
            {
                lblSignalName.Text = "初始化正常，请启动监控！";

                showLeftList(sigList);

                //if (option.AutoConn)
                //    timerGetCurSig.Enabled = true;
            }
            else
            {
                lblSignalName.Text = "连接数据库失败，请检查后重试！";
            }

            Trace.WriteLine("...form loaded");
        }

        private void showLeftList(List<SignalProp> list)
        {
            foreach (SignalProp sp in list)
                tvSignalList.Nodes.Add(sp.SignalName);
        }

        private void showSigInfo(SignalProp prop)
        {
            lblBaseSigName.Text = prop.SignalName;
            lblBaseIP.Text = prop.Ip;
            lblBaseType.Text = CommonUtils.SigType[prop.Type];
            lblBaseState.Text = prop.State;
        }

        private Option LoadOption()
        {
            string confFile = CUR_DIR + "/CONFIG.ini";
            Option option = new Option();
            option.AutoConn = Boolean.Parse(IniFileOperator.ReadIniData("OPTION", "AUTOCONN", "true", confFile));
            option.SigSwitchPath = IniFileOperator.ReadIniData("OPTION", "SIGPATH", "", confFile);
            option.XtAddress = IniFileOperator.ReadIniData("OPTION", "ADDRESS", "", confFile);///

            return option;
        }

        Boolean FResize = true;
        SignalProp FProp = new SignalProp();
        /// <summary>
        /// 定时获取当前信号机状态，并展示在界面上
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void timer1_Tick(object sender, EventArgs e)
        {
            if (specPoly == 1)
            {// 自动特勤，从数据库中取当前信号机
                if (signalDao.getState() == ConnectionState.Open)
                {
                    FProp = signalDao.getCurSigProp();
                    String state = FProp.State;
                    FProp = SignalFactory.getSigProp(FProp.SignalID, sigList);
                    FProp.State = state;

                    if (null == FProp )
                        return;

                    if ( null==curSignal || !FProp.SignalID.Equals(curSignal.Prop.SignalID))
                        curSignal = SignalFactory.buildController(FProp);
                    
                        curSignal.Prop = FProp;
                }
                else
                {
                    timerGetCurSig.Enabled = false;
                    showPBs(false);

                    lblSignalName.Text = "数据库未连接！";
                    return;
                }
            }
            else
            {//手动特勤，从左侧列表中取当前双击的信号机
                if (null == curSignal)
                {
                    timerGetCurSig.Enabled = false;
                    showPBs(false);
                    return;
                }

                if (curSignal.updateState())
                {
                    FProp = curSignal.Prop;
                    showSigInfo(FProp);
                    // lblBaseState.Text = FProp.State;
                }
                else
                {
                    // 如果更新状态失败，则说明信号机状态异常，则停掉定时器，避免程序挂死
                    timerGetCurSig.Enabled = false;
                    lblSignalName.Text = "获取信号机状态失败";
                    showPBs(false);
                    return;
                }

                //listBox1.Items.Insert(0, null==curSignal.Prop.State?"get state error": curSignal.Prop.State);
            }

            // 处理显示
            showPBs(true);
            showSigInfo(FProp);
            dealWithSigProp(FProp);
        }

        SignalProp curSigProp = new SignalProp();
        private void dealWithSigProp(SignalProp prop)
        {
            if (!(null == prop  // 空值
                    && curSigProp.SignalID.Equals(prop.SignalID)  // 信号机没变
                    && curSigProp.State.Equals(prop.State)  // 且信号机状态没变
                    && !FResize))    // 且窗口没变化
            {
                FResize = false;
                Trace.WriteLine(" deal with signal: " + prop.SignalID);
                if (null != prop.State && !"".Equals(prop.State))
                {
                    showState(prop);
                }

                curSigProp = prop;
                //curSigProp.SignalID = prop.SignalID;
                //curSigProp.State = prop.State;

                lblSignalName.Text = "信号机: " + prop.SignalName;
                //Trace.WriteLine(prop.ToString());
            }
        }


        private void pbMap_MouseClick(object sender, MouseEventArgs e)
        {
            if (PicPointMode)
            {
                MessageBox.Show("(w, h): (" + this.Width + ", " + this.Height + "); \n"
                    + "point: (" + e.X + ", " + e.Y + "); \n"
                    + "percent:(" + e.X * 1.0 / this.Width + ", " + e.Y * 1.0 / this.Height + "); \n");
            }
        }

        private void MainForm_Resize(object sender, EventArgs e)
        {
            FResize = true;
            pb1.Width = pb2.Width = pb3.Width
                = pb9.Width = pb10.Width = pb11.Width
                = 96 * this.Width / BASE_WIDTH;

            pb1.Height = pb2.Height = pb3.Height
                = pb9.Height = pb10.Height = pb11.Height
                = 26 * this.Height / BASE_HEIGHT;

            pb5.Width = pb6.Width = pb7.Width
                = pb13.Width = pb14.Width = pb15.Width
                = 26 * this.Width / BASE_WIDTH;

            pb5.Height = pb6.Height = pb7.Height
                = pb13.Height = pb14.Height = pb15.Height
                = 96 * this.Height / BASE_HEIGHT;

        }

        private void newToolStripMenuItem_Click(object sender, EventArgs e)
        {
            timerGetCurSig.Enabled = true;
            lblSignalName.Text = "正在连接...";
        }

        private void ToolStripMenuItemDesconnect_Click(object sender, EventArgs e)
        {
            timerGetCurSig.Enabled = false;
            lblSignalName.Text = "连接已断开";
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void toolStripMenuItem2_Click(object sender, EventArgs e)
        {
            PicPointMode = !PicPointMode;

            if (PicPointMode)
                MessageBox.Show("进入取点模式");
            else MessageBox.Show("关闭取点模式");
        }

        private void 信号机控制SToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Option option = LoadOption();
            WinExec(option.SigSwitchPath, 3);
            //0: 隐藏, 并且任务栏也没有最小化图标  
            //1: 用最近的大小和位置显示, 激活  
            //2: 最小化, 激活  
            //3: 最大化, 激活  
            //4: 用最近的大小和位置显示, 不激活  
            //5: 同 1  
            //6: 最小化, 不激活  
            //7: 同 3  
            //8: 同 3  
            //9: 同 1  
            //10: 同 1 
        }

        private void helpToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }

        private void toolsToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }

        private void btnManSpecStart_Click(object sender, EventArgs e)
        {
            if (cbbDirFrom.SelectedIndex == -1
                || cbbDirTo.SelectedIndex == -1
                || cbbDirFrom.SelectedIndex == cbbDirTo.SelectedIndex)
            {
                MessageBox.Show("特勤方向不正确,请重选!");
                return;
            }

            if (null != curSignal)
            {
                String str = "手动开启特勤：" + curSignal.Prop.SignalName + "From: " + CommonUtils.Dirs[cbbDirFrom.SelectedIndex + 1]
                    + " To: " + CommonUtils.Dirs[cbbDirTo.SelectedIndex + 1];
                //for (int i = 0; i < 5; i++)
                //{
                    if (curSignal.openSpecDuty((byte)(cbbDirFrom.SelectedIndex + 1), (byte)(cbbDirTo.SelectedIndex + 1)))
                    {
                        Trace.WriteLine(str + " ->>成功");
                        lblSpecRst.Text = "特勤成功";
                    }
                    else
                    {
                        Trace.WriteLine(str + " ->>失败");
                        lblSpecRst.Text = "特勤失败";
                    }
                //    Thread.Sleep(1000);
                //}
            }
            else
            {
                MessageBox.Show("没有指定信号机!");
            }
        }

        private void btnManSpecStop_Click(object sender, EventArgs e)
        {
            if (curSignal.closeSpecDuty())
            {
                Trace.WriteLine("手动关闭特勤: " + curSignal.Prop.SignalName + " ->>成功");
                lblSpecRst.Text = "关闭特勤成功";
            }
            else
            {
                Trace.WriteLine("手动关闭特勤: " + curSignal.Prop.SignalName + " ->>失败");
                lblSpecRst.Text = "关闭特勤失败";
            }
        }

        private void tvSignalList_DoubleClick(object sender, EventArgs e)
        {
            string selStr = tvSignalList.SelectedNode.Text;
            SignalProp prop = SignalFactory.getSigPropByName(selStr, sigList);

            timerGetCurSig.Enabled = false;

            if (null != prop)
            {
                tlog(prop.ToString());
                curSignal = SignalFactory.buildController(prop);
                tlog(prop.ToString());
                showMap(prop.SignalID);
                showSigInfo(prop);
                rbManSpec.Checked = true;

                if (curSignal.connect())
                {
                    timerGetCurSig.Enabled = true;
                }
                else
                {
                    lblSignalName.Text = "信号机：" + prop.SignalName + " 连接失败";
                }
            }
            else
            {
                lblSignalName.Text = "未选中信号机";
            }
        }

        private void tlog(string msg)
        {
            Trace.WriteLine(msg);
            //listBox1.Items.Insert(0, msg);
        }

        private void rbManSpec_CheckedChanged(object sender, EventArgs e)
        {
            if (rbManSpec.Checked)
            {
                specPoly = 0;
                //timerGetCurSig.Enabled = false;
                timerAutoSpec.Enabled = false;
                gbManPlate.Visible = true;
                tlog(" Special Duty Policy Change, New policy is: [手动]");
            }
        }

        private void rbAutoSpec_CheckedChanged(object sender, EventArgs e)
        {
            if (rbAutoSpec.Checked)
            {
                specPoly = 1;
                timerGetCurSig.Enabled = true;
                timerAutoSpec.Enabled = true;

                gbManPlate.Visible = false;

                tlog(" Special Duty Policy Change,  New policy is: [自动]");


            }
        }

        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            Trace.WriteLine(" Program ending....");
            Trace.WriteLine(" =======================================================");
        }

        /// <summary>
        /// 自动特勤定时器
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void timerAutoSpec_Tick(object sender, EventArgs e)
        {
            if (null == curSigProp)
                return;

            if (curSigProp.SpecState == 1)
            { // 开启特勤
                curSignal.openSpecDuty(curSigProp.DirFrom, curSigProp.DirTo);
                lblSpecRst.Text = "开启自动特勤";
            }
            else if (curSigProp.SpecState == 2)
            { //关闭特勤 
                curSignal.closeSpecDuty();
                lblSpecRst.Text = "关闭自动特勤";
            }
            else
            {
                lblSpecRst.Text = "正常";
                // 不处理
            }

        }

        private void button9_Click(object sender, EventArgs e)
        {
            FormSigEdit frm = new FormSigEdit();
            frm.Text = "添加信号机";

            frm.ShowDialog();
        }

        private void button11_Click(object sender, EventArgs e)
        {
            FormSigEdit frm = new FormSigEdit();
            frm.Text = "修改信号机信息";

            frm.ShowDialog();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            FormSpecEdit frm = new FormSpecEdit();

            frm.ShowDialog();
        }

        private void button5_Click(object sender, EventArgs e)
        {
            FormSpecEdit frm = new FormSpecEdit();

            frm.ShowDialog();
        }

        private void toolStripButton16_Click(object sender, EventArgs e)
        {
            AboutBox1 frm = new AboutBox1();
            frm.ShowDialog();
        }

        private void toolStripButton15_Click(object sender, EventArgs e)
        {
            System.Diagnostics.Process.Start("SignalConfig.exe");
        }

        private void toolStripButton2_Click(object sender, EventArgs e)
        {
            System.Diagnostics.Process.Start("SignalStatus.exe");
        }
    }
}
