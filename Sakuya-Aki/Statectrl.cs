using Microsoft.Win32;
using System;
using System.Threading;
using System.Windows.Threading;

namespace Sakuya_Aki
{
    class Statectrl
    {
        MainWindow mainwindow;
        RegistryKey rsg = null;
        //好感度
        private int skl;
        //饥饿度
        private int hung;
        private int hungskl = 0;
        //清洁度
        private int cleaner;
        private int cleanerskl = 0;
        //一些定时器
        private DispatcherTimer sklt = new DispatcherTimer();
        private DispatcherTimer hungt = new DispatcherTimer();
        private DispatcherTimer cleanert = new DispatcherTimer();
        private DispatcherTimer checkstate = new DispatcherTimer();

        public Statectrl(MainWindow mainwindow)
        {
            this.mainwindow = mainwindow;
        }
        public void start()
        {
            refresh();
            Thread runtimer = new Thread(starttimer);
            runtimer.Start();
        }
        public void starttimer()
        {
            hungt.Tick += new EventHandler(hungchecker);
            hungt.Interval = new TimeSpan(0, 0, 0, 900, 0);
            //hungt.Interval = new TimeSpan(0, 0, 0, 1, 0);
            hungt.Start();
            cleanert.Tick += new EventHandler(cleanerchecker);
            cleanert.Interval = new TimeSpan(0, 0, 0, 1800, 0);
            //cleanert.Interval = new TimeSpan(0, 0, 0, 1, 500);
            cleanert.Start();
            checkstate.Tick += new EventHandler(Checkstate);
            checkstate.Interval = new TimeSpan(0, 0, 1);
            checkstate.Start();
        }
        public void checkreg()//关于注册表的读取与失效时建立
        {
            rsg = Registry.LocalMachine.CreateSubKey("SOFTWARE\\Sakuya\\" + mainwindow.name);
            rsg = Registry.LocalMachine.OpenSubKey("SOFTWARE\\Sakuya\\" + mainwindow.name, true);
            try
            {
                if (rsg.GetValue("skl") != null && rsg.GetValue("hung") != null && rsg.GetValue("cleaner") != null &&
                    rsg.GetValue("lunchtiptime") != null && rsg.GetValue("sleeptiptime") != null && rsg.GetValue("colorR") != null &&
                    rsg.GetValue("colorG") != null && rsg.GetValue("colorB") != null && rsg.GetValue("scale") != null &&
                    rsg.GetValue("inStartTime") != null && rsg.GetValue("LastStartTime") != null)
                {
                    mainwindow.skl = Convert.ToInt32(rsg.GetValue("skl"));
                    mainwindow.hung = Convert.ToInt32(rsg.GetValue("hung"));
                    mainwindow.cleaner = Convert.ToInt32(rsg.GetValue("cleaner"));
                    mainwindow.lunchtime = Convert.ToDateTime(rsg.GetValue("lunchtiptime"));
                    mainwindow.sleeptime = Convert.ToDateTime(rsg.GetValue("sleeptiptime"));
                    mainwindow.colorR = Convert.ToByte(rsg.GetValue("colorR"));
                    mainwindow.colorG = Convert.ToByte(rsg.GetValue("colorG"));
                    mainwindow.colorB = Convert.ToByte(rsg.GetValue("colorB"));
                    mainwindow.scale = Convert.ToDouble(rsg.GetValue("scale"));
                    skl = Convert.ToInt32(rsg.GetValue("skl"));
                    hung = Convert.ToInt32(rsg.GetValue("hung"));
                    cleaner = Convert.ToInt32(rsg.GetValue("cleaner"));
                    mainwindow.LastStartTime = Convert.ToDateTime(rsg.GetValue("LastStartTime"));
                    mainwindow.inStartTime = Convert.ToDateTime(rsg.GetValue("inStartTime"));
                    checklogintime();
                }
                else
                {
                    Registry.LocalMachine.CreateSubKey("SOFTWARE\\Sakuya\\" + mainwindow.name);
                    rsg.SetValue("skl", "5");
                    rsg.SetValue("hung", "100");
                    rsg.SetValue("cleaner", "100");
                    rsg.SetValue("lunchtiptime", "12:0:0");
                    rsg.SetValue("sleeptiptime", "21:0:0");
                    rsg.SetValue("colorR", "255");
                    rsg.SetValue("colorG", "147");
                    rsg.SetValue("colorB", "147");
                    rsg.SetValue("scale", "1");
                    rsg.SetValue("LastStartTime", Convert.ToString(mainwindow.CurrentStartTime));
                    rsg.SetValue("inStartTime", Convert.ToString(mainwindow.CurrentStartTime));
                }
            }
            catch
            {
                Registry.LocalMachine.CreateSubKey("SOFTWARE\\Sakuya\\" + mainwindow.name);
                rsg.SetValue("skl", "5");
                rsg.SetValue("hung", "100");
                rsg.SetValue("cleaner", "100");
                rsg.SetValue("lunchtiptime", "12:0:0");
                rsg.SetValue("sleeptiptime", "21:0:0");
                rsg.SetValue("colorR", "255");
                rsg.SetValue("colorG", "147");
                rsg.SetValue("colorB", "147");
                rsg.SetValue("scale", "1");
                rsg.SetValue("LastStartTime", Convert.ToString(mainwindow.CurrentStartTime));
                rsg.SetValue("inStartTime", Convert.ToString(mainwindow.CurrentStartTime));
            }
        }
        private void hungchecker(object sender, EventArgs e)
        {
            //一个饥饿度的最小值
            if (hung > -200)
            {
                regupdate("hung", -1);
            }
            if (hung < 0)
            {
                checkkl();
            }
            else if (hung < 20)
            {
                checkkl();
            }
            else if (hung < 40)
            {
                checkkl();
            }
            else if (hung < 60)
            {
                checkkl();
            }
            else if (hung < 80)
            {
                checkkl();
            }
            hungskl++;
        }//超时后减小饥饿度
        private void cleanerchecker(object sender, EventArgs e)
        {
            //一个清洁度的最小值
            if (cleaner > -200)
            {
                regupdate("cleaner", -1);
            }
            if (cleaner < 0)
            {
                checkkl();
            }
            else if (cleaner < 20)
            {
                checkkl();
            }
            else if (cleaner < 40)
            {
                checkkl();
            }
            else if (cleaner < 60)
            {
                checkkl();
            }
            else if (cleaner < 80)
            {
                checkkl();
            }
            cleanerskl++;
        }//超时后减小清洁度
        private void checkkl()
        {
            if (hungskl > 19)
            {
                hungskl = 0;
                regupdate("skl", -1);
            }
            if (cleanerskl > 19)
            {
                cleanerskl = 0;
                regupdate("skl", -1);
            }
        }
        private void Checkstate(object sender, EventArgs e)
        {
            if (hung > 60)
            {
                mainwindow.pictooltip("萌萌的桜");
                mainwindow.picway = -1;
            }
            else if (hung > 40 && hung < 60)
            {
                mainwindow.pictooltip("桜饿了，快给我投食吧");
                mainwindow.picmove("enable");
            }
            else if(hung > 20 && hung < 40)
            {
                mainwindow.picway = -1;
            }
            else if (hung < 20)
            {
                mainwindow.pictooltip("桜要饿死了");
                mainwindow.picmove("disable");
                mainwindow.picway = 34;
            }
        }//Aki的一些反映
        public void regupdate(string name,double num)
        {
            rsg = Registry.LocalMachine.OpenSubKey("SOFTWARE\\Sakuya\\" + mainwindow.name, true);
            string rsgvalue = Convert.ToString(Convert.ToInt32((rsg.GetValue(name))) + num);
            rsg.SetValue(name, rsgvalue);
            refresh();
        }//更新注册表(int)
        public void regset(string name, double num)
        {
            rsg = Registry.LocalMachine.OpenSubKey("SOFTWARE\\Sakuya\\" + mainwindow.name, true);
            rsg.SetValue(name, num);
            refresh();
        }
        public void tipregupdate(string name, string num)
        {
            rsg = Registry.LocalMachine.OpenSubKey("SOFTWARE\\Sakuya\\" + mainwindow.name, true);
            if (name == "lunchtiptime")
            {
                rsg.SetValue(name, num);
            }
            if (name == "sleeptiptime")
            {
                rsg.SetValue(name, num);
            }
            refresh();
        }//更新注册表(string)
        private void refresh()
        {
            rsg = Registry.LocalMachine.OpenSubKey("SOFTWARE\\Sakuya\\" + mainwindow.name, true);
            skl = Convert.ToInt32(rsg.GetValue("skl"));
            hung = Convert.ToInt32(rsg.GetValue("hung"));
            cleaner = Convert.ToInt32(rsg.GetValue("cleaner"));
            mainwindow.lunchtime = Convert.ToDateTime(rsg.GetValue("lunchtiptime"));
            mainwindow.sleeptime = Convert.ToDateTime(rsg.GetValue("sleeptiptime"));
            mainwindow.skl = skl;
            mainwindow.hung = hung;
            mainwindow.cleaner = cleaner;
            statehungupdate();
            statecleanerupdate();
        }//刷新桌宠状态
        public void savestate()
        {
            rsg.SetValue("skl", Convert.ToString(skl));
            rsg.SetValue("hung", Convert.ToString(hung));
            rsg.SetValue("cleaner", Convert.ToString(cleaner));
        }//保存状态
        public void resetstate()
        {
            rsg.SetValue("skl", "5");
            rsg.SetValue("hung", "105");
            rsg.SetValue("cleaner", "105");
            skl = Convert.ToInt32(rsg.GetValue("skl"));
            hung = Convert.ToInt32(rsg.GetValue("hung"));
            cleaner = Convert.ToInt32(rsg.GetValue("cleaner"));
            refresh();
        }//admin
        //更新桌宠好感度语句  
        public void statehungupdate()
        {
            if (hung > 80)
            {
                mainwindow.statehung = "萌萌的桜酱";
            }
            else if (hung > 60 && hung < 80)
            {
                mainwindow.statehung = "祈想要糖糖";
            }
            else if (hung > 40 && hung < 60)
            {
                mainwindow.statehung = "祈想吃饭饭";
            }
            else if (hung > 20 && hung < 40)
            {
                mainwindow.statehung = "祈想吃吃吃";
            }
            else if (hung > 0 && hung < 20)
            {
                mainwindow.statehung = "祈好饿啊 >_<";
            }
            else if (hung < 0)
            {
                mainwindow.statehung = "祈再饿就没欧派了 >_<";
            }
        } //更新桌宠饥饿度语句
        public void statecleanerupdate()
        {
            if (cleaner > 80)
            {
                mainwindow.statecleaner = "香香的";
            }
            else if (cleaner > 60 && cleaner < 80)
            {
                mainwindow.statecleaner = "黏黏的";
            }
            else if (cleaner > 40 && cleaner < 60)
            {
                mainwindow.statecleaner = "脏脏的";
            }
            else if (cleaner > 20 && cleaner < 40)
            {
                mainwindow.statecleaner = "臭臭的";
            }
            else if (hung > 0 && hung < 20)
            {
                mainwindow.statecleaner = "黑黑的";
            }
            else if (cleaner < 0)
            {
                mainwindow.statecleaner = "发霉的";
            }
        }//更新桌宠清洁度语句
        public void checklogintime()
        {
            mainwindow.CurrentStartTime = DateTime.Now.Date;

            rsg = Registry.LocalMachine.CreateSubKey("SOFTWARE\\Sakuya\\" + mainwindow.name);
            rsg = Registry.LocalMachine.OpenSubKey("SOFTWARE\\Sakuya\\" + mainwindow.name, true);

            Thread.Sleep(50);

            if (mainwindow.CurrentStartTime == mainwindow.LastStartTime)//如果是今天第N次打开
            {
                mainwindow.LoginTime = (mainwindow.CurrentStartTime - mainwindow.inStartTime).TotalDays;
            }
            else if ((mainwindow.CurrentStartTime - mainwindow.inStartTime).TotalDays == 1)//如果就在第二天
            {
                mainwindow.LoginTime = (mainwindow.CurrentStartTime - mainwindow.inStartTime).TotalDays;
                rsg.SetValue("LastStartTime", Convert.ToString(mainwindow.CurrentStartTime));
            }
            else//不是在第二天
            {
                if ((mainwindow.CurrentStartTime - mainwindow.LastStartTime).TotalDays == 1)//如果是在上一次记录的后面一天
                {
                    mainwindow.LoginTime = (mainwindow.CurrentStartTime - mainwindow.inStartTime).TotalDays;
                    rsg.SetValue("LastStartTime", Convert.ToString(mainwindow.CurrentStartTime));
                }
                else//如果不是连续打开的
                {
                    rsg.SetValue("inStartTime", Convert.ToString(mainwindow.CurrentStartTime));
                    rsg.SetValue("LastStartTime", Convert.ToString(mainwindow.CurrentStartTime));
                    mainwindow.inStartTime = mainwindow.CurrentStartTime;
                }
            }
        }//检查签到
    }
}
