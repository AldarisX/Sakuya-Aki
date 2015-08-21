using System;
using System.Diagnostics;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Forms;
using System.Windows.Threading;
using Microsoft.Win32;

namespace Sakuya_Aki
{
    class BackGroundShell
    {
        //用WF撸托盘
        MainWindow mainwindow;
        NotifyIcon notifyIcon = new NotifyIcon();
        private string state;
        private DateTime BeginTime;
        private DateTime EndTime;
        private DispatcherTimer notifystate = new DispatcherTimer();
        RegistryKey rsg = null;
        Process cmd = new Process();
        public System.Windows.Forms.MenuItem picmove = new System.Windows.Forms.MenuItem("禁止移动");
        public System.Windows.Forms.MenuItem windowtop = new System.Windows.Forms.MenuItem("顶置");

        public BackGroundShell(MainWindow mainwindow)
        {
            this.mainwindow = mainwindow;
        }
        public void InitialTray()//托盘初始化
        {
            BeginTime = DateTime.Now;
            //设置托盘的各个属性
            notifyIcon.BalloonTipText = "桜来了";
            notifyIcon.Text = Convert.ToString(mainwindow.statecleaner) + Convert.ToString(mainwindow.statehung);
            //notifyIcon.Icon = new Icon(System.Windows.Forms.Application.StartupPath + "/icon.ico");
            notifyIcon.Icon = Properties.Resources.icon;
            notifyIcon.Visible = true;
            notifyIcon.ShowBalloonTip(100);
            notifyIcon.MouseClick += new MouseEventHandler(notifyIcon_MouseClick);

            //设置投食菜单项
            System.Windows.Forms.MenuItem skladd = new System.Windows.Forms.MenuItem("好感度+10");
            skladd.Click += new EventHandler(Addskl);
            System.Windows.Forms.MenuItem hungadd = new System.Windows.Forms.MenuItem("饥饿度+10");
            hungadd.Click += new EventHandler(Addhung);
            System.Windows.Forms.MenuItem cleaneradd = new System.Windows.Forms.MenuItem("清洁度+10");
            cleaneradd.Click += new EventHandler(Addcleaner);
            System.Windows.Forms.MenuItem alladd = new System.Windows.Forms.MenuItem("全部+10");
            alladd.Click += new EventHandler(Addall);
            System.Windows.Forms.MenuItem admin = new System.Windows.Forms.MenuItem("恢复初始值");
            admin.Click += new EventHandler(resetstate);
            System.Windows.Forms.MenuItem menu = new System.Windows.Forms.MenuItem("给桜酱投食", new System.Windows.Forms.MenuItem[] { skladd, hungadd, cleaneradd, alladd, admin });

            //时间提醒的菜单项
            System.Windows.Forms.MenuItem warnlunchtime = new System.Windows.Forms.MenuItem("设定吃午饭时间");
            warnlunchtime.Click += new EventHandler(setlunchtiptime);
            System.Windows.Forms.MenuItem warnsleeptime = new System.Windows.Forms.MenuItem("设定睡觉时间");
            warnsleeptime.Click += new EventHandler(setsleeptiptime);
            System.Windows.Forms.MenuItem tipupdate = new System.Windows.Forms.MenuItem("设置提醒", new System.Windows.Forms.MenuItem[] { warnlunchtime, warnsleeptime });

            //设置颜色菜单项
            System.Windows.Forms.MenuItem colorAki = new System.Windows.Forms.MenuItem("Aki红");
            colorAki.Click += new EventHandler(setcolorAki);
            System.Windows.Forms.MenuItem colorTianyi = new System.Windows.Forms.MenuItem("天依蓝");
            colorTianyi.Click += new EventHandler(setcolorTianyi);
            System.Windows.Forms.MenuItem colorMiku = new System.Windows.Forms.MenuItem("Miku绿");
            colorMiku.Click += new EventHandler(setcolorMiku);
            System.Windows.Forms.MenuItem colorBlack = new System.Windows.Forms.MenuItem("粉切黑");
            colorBlack.Click += new EventHandler(setcolorBlack);
            System.Windows.Forms.MenuItem setcolor = new System.Windows.Forms.MenuItem("设置颜色", new System.Windows.Forms.MenuItem[] { colorAki, colorTianyi, colorMiku, colorBlack });

            //设置缩放级别菜单项
            System.Windows.Forms.MenuItem l1 = new System.Windows.Forms.MenuItem("0.25");
            l1.Click += new EventHandler(setscalel1);
            System.Windows.Forms.MenuItem l2 = new System.Windows.Forms.MenuItem("0.50");
            l2.Click += new EventHandler(setscalel2);
            System.Windows.Forms.MenuItem l3 = new System.Windows.Forms.MenuItem("1.00");
            l3.Click += new EventHandler(setscalel3);
            System.Windows.Forms.MenuItem l4 = new System.Windows.Forms.MenuItem("1.50");
            l4.Click += new EventHandler(setscalel4);
            System.Windows.Forms.MenuItem l5 = new System.Windows.Forms.MenuItem("2.00");
            l5.Click += new EventHandler(setscalel5);
            System.Windows.Forms.MenuItem lc = new System.Windows.Forms.MenuItem("自定义");
            lc.Click += new EventHandler(setscalelc);
            System.Windows.Forms.MenuItem setscale = new System.Windows.Forms.MenuItem("设置缩放", new System.Windows.Forms.MenuItem[] { l1, l2, l3, l4, l5, lc });

            //开机启动菜单项
            System.Windows.Forms.MenuItem startup = new System.Windows.Forms.MenuItem("开机启动");
            startup.Click += new EventHandler(AutoStartUp);
            System.Windows.Forms.MenuItem startupc = new System.Windows.Forms.MenuItem("关闭开机启动");
            startupc.Click += new EventHandler(AutoStartUpC);
            System.Windows.Forms.MenuItem StartUpMenu = new System.Windows.Forms.MenuItem("开机启动", new System.Windows.Forms.MenuItem[] { startup ,startupc});

            //设置设置菜单项
            System.Windows.Forms.MenuItem aboutme = new System.Windows.Forms.MenuItem("关于桜祈");
            aboutme.Click += new EventHandler(AboutMe);
            System.Windows.Forms.MenuItem reset = new System.Windows.Forms.MenuItem("重置桌桜酱的位置");
            reset.Click += new EventHandler(resetPic);
            picmove.Checked = false;
            picmove.Click += new EventHandler(picmovechange);
            windowtop.Checked = false;
            windowtop.Click += new EventHandler(windowstopchange);
            System.Windows.Forms.MenuItem setting = new System.Windows.Forms.MenuItem("设置", new System.Windows.Forms.MenuItem[] { tipupdate, setcolor, setscale, picmove, windowtop, StartUpMenu, aboutme, reset, });

            //设置单菜单项
            System.Windows.Forms.MenuItem exit = new System.Windows.Forms.MenuItem("退出");
            exit.Click += new EventHandler(exit_Click);
            System.Windows.Forms.MenuItem loadxxx = new System.Windows.Forms.MenuItem("查看桜酱的状态");
            loadxxx.Click += new EventHandler(loadstate);

            //关联托盘控件
            System.Windows.Forms.MenuItem[] childen = new System.Windows.Forms.MenuItem[] { loadxxx, menu, setting, exit };
            notifyIcon.ContextMenu = new System.Windows.Forms.ContextMenu(childen);

            //动态托盘状态
            notifystate.Tick += new EventHandler(notifystateupdate);
            notifystate.Interval = new TimeSpan(0, 0, 30);
            notifystate.Start();
        }
        private void AboutMe(object sender, EventArgs e)
        {
            System.Windows.MessageBox.Show("由Aldaris&吉祥物编写，鸭纸绘制");
        }
        private void exit_Click(object sender, EventArgs e)
        {
            if (System.Windows.MessageBox.Show("要抛弃桜了吗?",
                                               "退出",
                                                MessageBoxButton.YesNo,
                                                MessageBoxImage.Question,
                                                MessageBoxResult.No) == MessageBoxResult.Yes)
            {
                mainwindow.savestate();
                notifyIcon.Dispose();
                //System.Windows.Application.Current.Shutdown();
                Environment.Exit(0);
            }
        }//退出的方法
        private void notifyIcon_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                if (mainwindow.Visibility == Visibility.Visible)
                {
                    mainwindow.Visibility = Visibility.Hidden;
                }
                else
                {
                    mainwindow.Visibility = Visibility.Visible;
                    mainwindow.Activate();
                }
            }
        } //鼠标单击
        private void resetPic(object sender, EventArgs e)
        {
            mainwindow.randomxy();
            mainwindow.pic.SetValue(Canvas.LeftProperty, (double)mainwindow.rx);
            mainwindow.pic.SetValue(Canvas.TopProperty, (double)mainwindow.ry);
        }//重置桌宠坐标
        private void loadstate(object sender, EventArgs e)
        {
            EndTime = DateTime.Now;
            TimeSpan RunTime = new TimeSpan();
            RunTime = EndTime.Subtract(BeginTime);
            state = Convert.ToString(mainwindow.statecleaner) + Convert.ToString(mainwindow.statehung) + "\n";
            string runtime = "桜酱卖了" + Convert.ToString(RunTime);
            string skl = "\n好感度：" + Convert.ToString(mainwindow.skl);
            string hung = "\n饥饿度：" + Convert.ToString(mainwindow.hung);
            string cleaner = "\n清洁度：" + Convert.ToString(mainwindow.cleaner);
            string logintime = "\n连续开启了" + Convert.ToString(mainwindow.LoginTime) + "天";
            string show = runtime.Substring(0, runtime.Length - 8) + "的萌\n" + state + skl + hung + cleaner + logintime;
            System.Windows.MessageBox.Show(show);
        }//加载状态
        private void Addskl(object sender, EventArgs e)
        {
            mainwindow.updatestate("skl", 10);
            mainwindow.SklUp();
        }//+10好感度
        private void Addhung(object sender, EventArgs e)
        {
            mainwindow.updatestate("hung", 10);
            mainwindow.SklUp();
            mainwindow.displaytips("吃饭饭", 2000);
        }//+10饥饿度
        private void Addcleaner(object sender, EventArgs e)
        {
            mainwindow.updatestate("cleaner", 10);
            mainwindow.SklUp();
            mainwindow.displaytips("洗澡澡", 2000);
        }//+10清洁度
        private void Addall(object sender, EventArgs e)
        {
            mainwindow.updatestate("skl", 10);
            mainwindow.updatestate("hung", 10);
            mainwindow.updatestate("cleaner", 10);
        } //+10全属性
        private void resetstate(object sender, EventArgs e)
        {
            mainwindow.resetstate();
        }//admin
        private void picmovechange(object sender, EventArgs e)
        {
            //如果没有开启禁止移动
            if (!picmove.Checked)
            {
                picmove.Checked = true;
                mainwindow.picmovex.IsChecked = true;
                mainwindow.picmove("disable");
            }
            //如果开启了禁止移动
            else
            {
                picmove.Checked = false;
                mainwindow.picmovex.IsChecked = false;
                mainwindow.picmove("enable");
            }
        }//允许移动的操作
        private void windowstopchange(object sender, EventArgs e)
        {
            //如果没有开启顶置
            if (!picmove.Checked)
            {
                windowtop.Checked = true;
                mainwindow.windowtopx.IsChecked = true;
                mainwindow.windowtopon();
            }
            //如果开启了顶置
            else
            {
                windowtop.Checked = false;
                mainwindow.windowtopx.IsChecked = false;
                mainwindow.windowtopoff();
            }
        }//窗口顶置的操作
        private void setlunchtiptime(object sender, EventArgs e)
        {
            mainwindow.tiptype = "lunchtiptime";
            mainwindow.settiptime("输入时间(一个整数)", "在屏幕中间输入0-24整数的时间哦", "time");
        }//设置吃午饭时间提醒
        private void setsleeptiptime(object sender, EventArgs e)
        {
            mainwindow.tiptype = "sleeptiptime";
            mainwindow.settiptime("输入时间(一个整数)", "在屏幕中间输入0-24整数的时间哦", "time");
        }//设置睡觉时间提醒
        private void setcolorAki(object sender, EventArgs e)
        {
            mainwindow.colorR = 255;
            mainwindow.colorG = 147;
            mainwindow.colorB = 147;
            setcolor(mainwindow.colorR, mainwindow.colorG, mainwindow.colorB);
        }//Aki红
        private void setcolorTianyi(object sender, EventArgs e)
        {
            mainwindow.colorR = 51;
            mainwindow.colorG = 204;
            mainwindow.colorB = 255;
            setcolor(mainwindow.colorR, mainwindow.colorG, mainwindow.colorB);
        }//天依蓝
        private void setcolorMiku(object sender, EventArgs e)
        {
            mainwindow.colorR = 51;
            mainwindow.colorG = 204;
            mainwindow.colorB = 0;
            setcolor(mainwindow.colorR, mainwindow.colorG, mainwindow.colorB);
        }//Miku绿
        private void setcolorBlack(object sender, EventArgs e)
        {
            mainwindow.colorR = 0;
            mainwindow.colorG = 0;
            mainwindow.colorB = 0;
            setcolor(mainwindow.colorR, mainwindow.colorG, mainwindow.colorB);
        }//粉切黑
        private void setcolor(byte r,byte g,byte b)
        {
            mainwindow.regset("colorR", r);
            mainwindow.regset("colorG", g);
            mainwindow.regset("colorB", b);
            mainwindow.displaytips("已经换好了哦", 5000);
        }//更新rgb色的注册表项
        private void setscalel1(object sender, EventArgs e)
        {
            setscalelevel(0.25);
        }//缩放l1
        private void setscalel2(object sender, EventArgs e)
        {
            setscalelevel(0.5);
        }//缩放l2
        private void setscalel3(object sender, EventArgs e)
        {
            setscalelevel(1);
        }//缩放l3
        private void setscalel4(object sender, EventArgs e)
        {
            setscalelevel(1.5);
        }//缩放l4
        private void setscalel5(object sender, EventArgs e)
        {
            setscalelevel(2);
        }//缩放l5
        private void setscalelc(object sender, EventArgs e)
        {
            mainwindow.tiptype = "scale";
            mainwindow.settiptime("输入一个数", "在屏幕中间输入一个数哦", "scale");
        }//缩放lc
        private void AutoStartUp(object sender, EventArgs e)
        {
            Console.WriteLine();
            rsg = Registry.LocalMachine.CreateSubKey("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run\\");
            rsg = Registry.LocalMachine.OpenSubKey("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run\\", true);
            string command = @"""" + Process.GetCurrentProcess().MainModule.FileName + @"""";
            rsg.SetValue("Sakuya-Aki", command);
            mainwindow.displaytips("已经设定好了哦", 2000);
        }//开机启动
        private void AutoStartUpC(object sender, EventArgs e)
        {
            rsg = Registry.LocalMachine.CreateSubKey("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run\\");
            rsg = Registry.LocalMachine.OpenSubKey("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run\\", true);
            rsg.DeleteValue("Sakuya-Aki");
            mainwindow.displaytips("已经取消了哦", 2000);
        }//关闭开机启动
        private void setscalelevel(double level)
        {
            mainwindow.scale = level;
            mainwindow.pic.Width = mainwindow.size * mainwindow.scale;
            mainwindow.pic.Height = mainwindow.size * mainwindow.scale;
            mainwindow.regset("scale", level);
            MainWindow.doevents();
            mainwindow.displaytips("已经换好了哦", 5000);
        }
        public void notifystateupdate(object sender, EventArgs e)
        {
            notifyIcon.Text = Convert.ToString(mainwindow.statecleaner) + Convert.ToString(mainwindow.statehung);
        }//托盘的悬停提示
    }
}