using System;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Threading;
using System.Xml;

namespace Sakuya_Aki
{
    class Tips
    {
        MainWindow mainwindow;
        Thread tips;
        private string tipcon;
        private int dt;
        private DispatcherTimer timetip = new DispatcherTimer();
        private DispatcherTimer timecheck = new DispatcherTimer();
        private DispatcherTimer tipscheck = new DispatcherTimer();
        private int nowhour = DateTime.Now.Hour;
        private int backhour = DateTime.Now.Hour;
        Random rd = new Random();

        public Tips(MainWindow mainwindow)
        {
            this.mainwindow = mainwindow;
        }
        public void Start()
        {
            Thread runtimer = new Thread(starttimer);
            runtimer.Start();
        }
        public void starttimer()
        {
            timetip.Tick += new EventHandler(rdtimetip);
            timetip.Interval = new TimeSpan(0, 16, 0);
            timetip.Start();
            timecheck.Tick += new EventHandler(checktime);
            timecheck.Interval = new TimeSpan(0, 1, 0);
            timecheck.Start();
            tipscheck.Tick += new EventHandler(sometips);
            tipscheck.Interval = new TimeSpan(0, 0, 31);
            tipscheck.Start();
        }
        public void firsttime()
        {
            DateTime now = DateTime.Now;
            displaytips("现在是" + Convert.ToString(now), 2000);
        }//报时
        public void displaytips(string tipcon, int tipint)
        {
            tips = new Thread(tipthread);
            this.tipcon = tipcon;
            dt = tipint;
            tips.Start();
        }//用新线程管理tip
        private void tipthread()
        {
            Label tip = mainwindow.tip;
            tip.Dispatcher.Invoke(new Action(()=>
                {
                    tip.Foreground = new SolidColorBrush(Color.FromRgb(mainwindow.colorR, mainwindow.colorG, mainwindow.colorB));
                    tip.FontSize = mainwindow.scale * mainwindow.swidth * mainwindow.sheight * 0.00001;
                    tip.Content = tipcon;
                    MainWindow.doevents();
                    tipxy(tipcon);
                    tip.SetValue(Canvas.LeftProperty, mainwindow.tipx);
                    tip.SetValue(Canvas.TopProperty, mainwindow.tipy);
                    tip.Visibility = Visibility.Visible;
                    MainWindow.doevents();
                }));
            //这里分开是因为.Dispatcher.Invoke里是主线程
            Thread.Sleep(dt);
            tip.Dispatcher.Invoke(new Action(() =>
                {
                    tip.Visibility = Visibility.Hidden;
                }));
        }//tip显示的管理
        public void rdtimetip(object sender, EventArgs e)
        {
            if (!mainwindow.isClick)
            {
                int i = rd.Next(0, 2);
                if (i == 0)
                {
                    reporttime();
                }
            }
        }//随机显示时间tip
        private void reporttime()
        {
            string hours;
            string mins;
            string secs;
            DateTime now = DateTime.Now;
            int hour = Convert.ToInt32(now.Hour);
            if (hour - 10 < 0)
            {
                hours = "0" + hour;
            }
            else
            {
                hours = Convert.ToString(hour);
            }
            int min = Convert.ToInt32(now.Minute);
            if (min - 10 < 0)
            {
                mins = "0" + min;
            }
            else
            {
                mins = Convert.ToString(min);
            }
            int sec = Convert.ToInt32(now.Second);
            if (sec - 10 < 0)
            {
                secs = "0" + sec;
            }
            else
            {
                secs = Convert.ToString(sec);
            }
            string str = "现在时间是" + hours + ":" + mins + ":" + secs;
            mainwindow.displaytips(str, 2000);
        }//计算时间
        private void checktime(object sender, EventArgs e)
        {
            int nowhour = DateTime.Now.Hour;
            //int nowmin = DateTime.Now.Minute;
            Console.WriteLine(mainwindow.lunchtime.Hour);
            if (nowhour - backhour == 1)
            {
                if (nowhour == mainwindow.lunchtime.Hour)
                {
                    displaytips("到点了，master我们去吃午饭吧", 5000);
                }
                else if (nowhour == mainwindow.sleeptime.Hour)
                {
                    displaytips("到点了,master我们去睡觉吧", 5000);
                }
                backhour++;
            }
        }//检查是否到了特定时间，否则进行普通报时 
        private void sometips(object sender, EventArgs e)
        {
            if (!mainwindow.isClick)
            {
                //概率出提示
                int way = rd.Next(0, 3);
                if (way == 1)
                {
                    //读取xml
                    XmlDocument xml = new XmlDocument();
                    try
                    {
                        //xml.Load("tips.xml");
                        xml.LoadXml(Properties.Resources.tips);
                        int levelnum = 1;
                        int wayx = rd.Next(0, 101);
                        //提高基础卖萌的概率
                        if (wayx < 51)
                        {
                            levelnum = 1;
                        }
                        else
                        {
                            levelnum = rd.Next(1, xml.SelectNodes("/tips/level").Count + 1);
                        }
                        string level = "/tips/level" + "[" + levelnum + "]";
                        int tipnum = rd.Next(1, xml.SelectNodes(level + "/tip").Count + 1);
                        string tip = level + "/tip" + "[" + tipnum + "]";
                        string con = xml.SelectSingleNode(tip + "/str").InnerText;
                        //xml里没写显示时间的默认操作
                        int dt;
                        try
                        {
                            dt = Convert.ToInt32(Convert.ToDouble(xml.SelectSingleNode(tip + "/t").InnerText));
                        }
                        catch
                        {
                            dt = 2000;
                        }
                        displaytips(con, dt);
                    }
                    //没有文件就会报错
                    catch
                    {
                        MessageBox.Show("检测到文件缺失，请重新下载");
                        Environment.Exit(0);
                    }
                }
            }
        }//日常卖萌
        public void tipxy(string tipcon)
        {
            mainwindow.picxy();
            double size = mainwindow.size;
            double picx = mainwindow.picx;
            double picy = mainwindow.picy;
            int swidth = mainwindow.swidth;
            int sheight = mainwindow.sheight;
            double length = tipcon.Length * mainwindow.tip.FontSize * mainwindow.scale * 1.05;
            //如果在右上角
            if (picy - mainwindow.tip.FontSize * mainwindow.scale < 0 && picx + length > swidth)
            {
                mainwindow.tipx = mainwindow.picx - length / 2.5;
                mainwindow.tipy = 16;
            }
            //如果在右边
            else if (picx + length > swidth)
            {
                mainwindow.tipx = mainwindow.picx - length / 2.5;
                mainwindow.tipy = mainwindow.picy - mainwindow.size * mainwindow.scale * 0.04;
            }
            //如果在上边
            else if (picy - mainwindow.tip.FontSize * mainwindow.scale < 0)
            {
                mainwindow.tipx = mainwindow.picx + mainwindow.size * mainwindow.scale * 0.80;
                mainwindow.tipy = 16;
            }
            else
            {
                mainwindow.tipx = mainwindow.picx + mainwindow.size * mainwindow.scale * 0.80;
                mainwindow.tipy = mainwindow.picy - mainwindow.size * mainwindow.scale * 0.04;
            }
        }//tip的坐标
    }
}
