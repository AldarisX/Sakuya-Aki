﻿using System;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Threading;

namespace Sakuya_Aki
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class iFrame : Window
    {
        //名字
        public string name = "Aki";
        public string programname = "Sakuya-Aki";
        //颜色 Aki红FF9393 255, 147, 147
        public byte colorR = 255;
        public byte colorG = 147;
        public byte colorB = 147;
        Picctrl picctrl;
        Statectrl statectrl;
        BackGroundShell bgshell;
        ActionZC actionzc;
        Tips tips;
        Random rd = new Random();
        static System.Drawing.Rectangle rect = System.Windows.Forms.SystemInformation.VirtualScreen;
        //获取机子的分辨率（宽高）
        public int swidth = rect.Width;
        public int sheight = rect.Height;
        //缩放级别
        public double scale = 1;
        //桌宠的大小（正方形）
        public double size;
        //随机桌宠横纵坐标
        public double rx;
        public double ry;
        //桌宠的横纵坐标
        public double picx;
        public double picy;
        //tip的坐标
        public double tipx;
        public double tipy;
        //好感度
        public int skl;
        public int sklup;
        public int skldown;
        //饥饿度
        public int hung;
        //清洁度
        public int cleaner;
        //桌宠状态
        public string stateskl;
        public string statehung;
        public string statecleaner;
        //提醒时间
        public DateTime lunchtime;
        public DateTime sleeptime;
        //判断是否点击了桌宠
        public bool isClick = false;
        //签到相关的
        public DateTime LastStartTime;
        public DateTime inStartTime = new DateTime();
        public DateTime CurrentStartTime = new DateTime();
        public double LoginTime;
        //强制控制图片
        public int picway = -1;

        public iFrame()
        {
            InitializeComponent();
           
            try
            {
                bgshell = new BackGroundShell(this);
                picctrl = new Picctrl(this);
                statectrl = new Statectrl(this);
                actionzc = new ActionZC(this);
                tips = new Tips(this);

                //设定窗体的宽高
                Width = swidth;
                Height = sheight;
                canvas.Width = swidth;
                canvas.Height = sheight;
                //处理好感度
                Thread.Sleep(5);
                statectrl.checkreg();
                //启动好感控制
                Thread.Sleep(5);
                statectrl.start();
                //开始执行托盘
                Thread.Sleep(5);
                statectrl.ChecksStartUp();
                bgshell.InitialTray();
                //绘制pic
                Thread.Sleep(5);
                picdraw();
                //加载提示
                Thread.Sleep(400);
                tips.Start();
                tips.firsttime();
            }
            catch
            {
                //MessageBox.Show("没有使用管理员权限运行\n是否自动申请管理员权限");
                Environment.Exit(0);
            }
        }
        public void picdraw()
        {
            //使桌宠的大小等比的适应屏幕
            size = Math.Floor(Math.Sqrt(swidth * sheight / 56.25));
            if (size > 256)
            {
                size = 256 * scale;
            }
            //随机rx，ry
            randomxy();
            //设定桌宠的大小
            pic.Width = size * scale;
            pic.Height = size * scale;
            //设定桌宠的坐标
            pic.SetValue(Canvas.LeftProperty, rx);
            pic.SetValue(Canvas.TopProperty, ry);
            Thread.Sleep(5);
            picctrl.starttimer();
        }
        private void pic_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            picctrl.mouseleftdown();
        }//左键按下时桌宠xxx
        private void pic_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            picctrl.mouseleftup();
        }//左键弹起时桌宠xxx
        public void randomxy()
        {
            rx = rd.Next((int)size, swidth - (int)size);
            ry = rd.Next((int)size, sheight - (int)size);
        }//随机坐标的生成方法
        public void displaytips(string tipcon, int dt)//关于tip的
        {
            tips.displaytips(tipcon, dt);
        }
        public void savestate()
        {
            statectrl.savestate();
        }//保存桌宠状态
        public void updatestate(string name, double num)
        {
            statectrl.regupdate(name, num);
        }//更新桌宠各种属性
        public void regset(string name, double num)
        {
            statectrl.regset(name, num);
        }//设置注册表项目
        private void SklAdd(object sender, RoutedEventArgs e)
        {
            statectrl.regupdate("skl", 10);
            SklUp();
        }
		//WPF右键菜单状态的触发
        private void HungAdd(object sender, RoutedEventArgs e)
        {
            statectrl.regupdate("hung", 10);
            SklUp();
            displaytips("吃饭饭", 2000);
        }//WPF右键菜单状态的触发
        private void CleanerAdd(object sender, RoutedEventArgs e)
        {
            statectrl.regupdate("cleaner", 10);
            SklUp();
            displaytips("洗澡澡", 2000);
        }//WPF右键菜单状态的触发
        private void AddAll(object sender, RoutedEventArgs e)
        {
            statectrl.regupdate("skl", 10);
            statectrl.regupdate("hung", 10);
            statectrl.regupdate("cleaner", 10);
        }//WPF右键菜单状态的触发
        private void Game0st(object sender, RoutedEventArgs e)
        {
            Game0(0);
        }//WPF右键菜单游戏的触发
        private void Game0jd(object sender, RoutedEventArgs e)
        {
            Game0(1);
        }//WPF右键菜单游戏的触发
        private void Game0bu(object sender, RoutedEventArgs e)
        {
            Game0(2);
        }//WPF右键菜单游戏的触发
        private void Game0(int num)
        {
            actionzc.Game0(num);
            SklUp();
        }
		//WPF右键菜单游戏的触发
        private void picmovechange(object sender, RoutedEventArgs e)
        {
            //如果没有开启禁止移动
            if (picmovex.IsChecked)
            {
                picmove("disable");
                bgshell.picmove.Checked = true;
                picmovex.IsChecked = true;
            }
            //如果开启了禁止移动
            else
            {
                picmove("enable");
                bgshell.picmove.Checked = false;
                picmovex.IsChecked = false;
            }
        }//WPF右键菜单移动的触发
        private void windowstopchange(object sender, RoutedEventArgs e)
        {
            //如果没有开启顶置
            if (windowtopx.IsChecked)
            {
                windowtopx.IsChecked = true;
                bgshell.windowtop.Checked = true;
                windowtopon();
            }
            //如果开启了顶置
            else
            {
                windowtopx.IsChecked = false;
                bgshell.windowtop.Checked = false;
                windowtopoff();
            }
        }
        public void SklUp()
        {
            sklup++;
            if (sklup > 3)
            {
                updatestate("skl", 1);
                sklup = 0;
            }
        }
        private void ResetState(object sender, RoutedEventArgs e)
        {
            resetstate();
        }
        public string tiptype;
        public string tipv;
        //加入一个TextBox
        private TextBox TB = new TextBox();
        //加入Button
        private Button Busure = new Button();
        private Button Bucancle = new Button();
        public void settiptime(string con, string tipcon, string type)
        {
            //TextBox
            TB.Width = swidth * sheight * 0.000135;
            TB.Height = swidth * sheight * 0.0000215;
            TB.Foreground = new SolidColorBrush(Color.FromRgb(colorR, colorG, colorB));
            TB.FontSize = (double)swidth * sheight * 0.000015;
            //TB.Text = "输入时间(一个整数)";
            TB.Text = con;
            double tbwidth = swidth / 2 - swidth * 0.14;
            double tbheight = sheight / 2 - sheight * 0.04;
            TB.SetValue(Canvas.LeftProperty, tbwidth);
            TB.SetValue(Canvas.TopProperty, tbheight);
            //设置桌宠大小
            pic.Width = size;
            pic.Height = size;
            //设定桌宠位置
            pic.SetValue(Canvas.LeftProperty, tbwidth - size);
            pic.SetValue(Canvas.TopProperty, tbheight - size / 2);
            //设置tip的大小
            tip.Foreground = new SolidColorBrush(Color.FromRgb(colorR, colorG, colorB));
            double tipfontsize = swidth * sheight * 0.00001;
            tip.FontSize = tipfontsize;
            //tip.Content = "在屏幕中间输入0-24整数的时间哦";
            tip.Content = tipcon;
            tip.SetValue(Canvas.LeftProperty, tbwidth - size + tipfontsize + tip.ActualWidth / 3.5);
            tip.SetValue(Canvas.TopProperty, tbheight - size / 2 - tipfontsize - size * 0.00001);
            tip.Visibility = Visibility;
            //Button sure
            Busure.Width = swidth * sheight * 0.000055;
            Busure.Height = swidth * sheight * 0.0000215;
            Busure.FontSize = (double)swidth * sheight * 0.000014;
            Busure.Content = "确定";
            Busure.SetValue(Canvas.LeftProperty, tbwidth + swidth * sheight * 0.000142);
            Busure.SetValue(Canvas.TopProperty, tbheight);
            buttontype = type;
            Busure.Click += new RoutedEventHandler(tiptimeupdate);
            //Button cancle
            Bucancle.Width = swidth * sheight * 0.000055;
            Bucancle.Height = swidth * sheight * 0.0000215;
            Bucancle.FontSize = (double)swidth * sheight * 0.000014;
            Bucancle.Content = "取消";
            Bucancle.SetValue(Canvas.LeftProperty, tbwidth + swidth * sheight * 0.000204);
            Bucancle.SetValue(Canvas.TopProperty, tbheight);
            Bucancle.Click += new RoutedEventHandler(tipupdatecancle);
            //添加至容器
            canvas.Children.Add(TB);
            canvas.Children.Add(Busure);
            canvas.Children.Add(Bucancle);
        }
        private void tipupdatecancle(object sender, RoutedEventArgs e)
        {
            canvas.Children.Remove(TB);
            canvas.Children.Remove(Busure);
            canvas.Children.Remove(Bucancle);
            displaytips(" ", 1);
            pic.Width = size * scale;
            pic.Height = size * scale;
            tip.FontSize = scale * swidth * sheight * 0.00001;
        }
        //设定一个按钮类型的变量
        string buttontype;
        private void tiptimeupdate(object sender, RoutedEventArgs e)
        {
            double test = 0;
            bool res = true;
            try
            {
                test = Convert.ToDouble(TB.Text);
            }
            catch (Exception)
            {
                displaytips("请输入数字哦", 2000);
                res = false;
                MessageBox.Show("输入的不是数字\n试试关掉输入法");
            }
            if (res == true)
            {
                if (test < 0)
                {
                    MessageBox.Show("输入的数不能小于0哦");
                }
                else
                {
                    //修改时间的
                    if (buttontype == "time")
                    {
                        double swap = Math.Floor(test);
                        if (test != swap)
                        {
                            displaytips("请输入整数哦", 2000);
                            MessageBox.Show("输入的数不是整数");
                        }
                        else
                        {
                            tipv = TB.Text + ":0:0";
                            displaytips(" ", 1);
                            statectrl.tipregupdate(tiptype, tipv);
                        }
                    }
                    //修改缩放级别的
                    else if (buttontype == "scale")
                    {
                        if (tipv == "0")
                        {
                            MessageBox.Show("输入的数不能为0");
                        }
                        else
                        {
                            displaytips(" ", 1);
                            scale = test;
                            pic.Width = size * scale;
                            pic.Height = size * scale;
                            statectrl.regset(tiptype, test);
                        }
                    }
                    canvas.Children.Remove(TB);
                    canvas.Children.Remove(Busure);
                    canvas.Children.Remove(Bucancle);
                    displaytips("Aki记住了哦", 2000);
                    pic.Width = size * scale;
                    pic.Height = size * scale;
                    tip.FontSize = scale * swidth * sheight * 0.00001;
                }
            }
        }
        public void picxy()
        {
            picx = Convert.ToInt32(pic.GetValue(Canvas.LeftProperty));
            picy = Convert.ToInt32(pic.GetValue(Canvas.TopProperty));
        }
        public void resetstate()
        {
            statectrl.resetstate();
        }//admin
        public void picmove(string tf)
        {
            if (tf == "disable")
            {
                picctrl.picisMove = false;
            }
            else if (tf == "enable")
            {
                picctrl.picisMove = true;
            }
        }//允许移动的开关
        public void checklogintime()
        {
            statectrl.checklogintime();
        }//定时检查签到
        public void windowtopon()
        {
            Topmost = true;
        }//开启顶置
        public void windowtopoff()
        {
            Topmost = false;
        }//关闭顶置
        public void pictooltip(string con)
        {
            pic.ToolTip = con;
            doevents();
        }
        public static void doevents()
        {
            DispatcherFrame frame = new DispatcherFrame(true);
            Dispatcher.CurrentDispatcher.BeginInvoke(DispatcherPriority.Background, (SendOrPostCallback)delegate (object arg)
            {
                DispatcherFrame fr = arg as DispatcherFrame;
                fr.Continue = false;
            }, frame);
            Dispatcher.PushFrame(frame);
        }//没有线程暂停的doevents
    }
}
