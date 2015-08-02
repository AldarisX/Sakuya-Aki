using System;
using System.Threading;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using System.Windows.Threading;

namespace Sakuya_Aki
{
    class Picctrl
    {
        MainWindow mainwindow;
        //pic大小为窗体的56.25x
        //设定线程暂停时间
        private static int sleeptime = 500;
        //屏幕分辨率相关
        static System.Drawing.Rectangle rect = System.Windows.Forms.SystemInformation.VirtualScreen;
        //获取机子的分辨率
        private int swidth = rect.Width;
        private int sheight = rect.Height;
        byte slowdown = 0;
        byte movenon = 0;
        //一些定时器
        private DispatcherTimer Timer = new DispatcherTimer();
        private DispatcherTimer outer = new DispatcherTimer();
        private DispatcherTimer mouser = new DispatcherTimer();
        private DispatcherTimer mousePoint = new DispatcherTimer();
        Thread imgctrl;
        int mousePointX0;
        int mousePointX1;
        byte skldown = 0;
        bool isAction = true;

        public Picctrl(MainWindow mainwindow)
        {
            this.mainwindow = mainwindow;
        }
        public void start()
        {
            //用新线程启动timer
            Thread runtimer = new Thread(starttimer);
            runtimer.Start();
        }
        public void starttimer()
        {
            Timer.Tick += new EventHandler(Timer_Tick);
            Timer.Interval = new TimeSpan(0, 0, 0, 0, sleeptime);
            Timer.Start();
            outer.Tick += new EventHandler(outArea);
            outer.Interval = new TimeSpan(0, 0, 0, 0, 200);
            outer.Start();
            mouser.Tick += new EventHandler(mouseMove);
            mouser.Interval = new TimeSpan(0, 0, 0, 0, 10);
            mouser.Start();
            mousePoint.Tick += new EventHandler(checkMousePoint);
            mousePoint.Interval = new TimeSpan(0, 0, 0, 0, 5);
            mousePoint.Start();
        }   //绘制pic
        private void Timer_Tick(object sender, EventArgs e)
        {
            //用新线程开始换图片
            imgctrl = new Thread(displayctrl);
            imgctrl.Start();
        }
        private void outArea(object sender, EventArgs e)
        {
            //当鼠标放开时判定是否出界
            if (!mainwindow.isClick)
            {
                //出界后再回到一个随机坐标
                Image pic = mainwindow.pic;
                double size = mainwindow.size * mainwindow.scale;
                pic.Dispatcher.Invoke(new Action(delegate
                {
                    mainwindow.randomxy();
                    double Left = (double)pic.GetValue(Canvas.LeftProperty);
                    double Top = (double)pic.GetValue(Canvas.TopProperty);
                    //if里的是边界的判定
                    isAction = false;
                    sleeptime = 10;
                    pic.IsHitTestVisible = false;
                    //旧的出界
                    //pic.SetValue(Canvas.LeftProperty, mainwindow.rx);
                    //pic.SetValue(Canvas.TopProperty, mainwindow.ry);
                    //如果在右上角
                    if (Left > swidth - size / 2 && Top < 0 - size / 4)
                    {
                        for (int i = 0; i < 16; i++)
                        {
                            sleeptime = 10;
                            displayimg("48", -2, 2);
                        }
                    }
                    //如果在右下
                    else if (Left > swidth - size / 2 && Top > sheight - size / 1.1)
                    {
                        for (int i = 0; i < 16; i++)
                        {
                            sleeptime = 10;
                            displayimg("48", -2, -2);
                        }
                    }
                    //如果在左下
                    else if (Left < 0 - size / 2 && Top > sheight - size / 1.1)
                    {
                        for (int i = 0; i < 16; i++)
                        {
                            sleeptime = 10;
                            displayimg("48", 2, -2);
                        }
                    }
                    //如果在左上
                    else if (Left < 0 - size / 2 && Top < 0 - size / 4)
                    {
                        for (int i = 0; i < 16; i++)
                        {
                            sleeptime = 10;
                            displayimg("48", 2, 2);
                        }
                    }
                    //如果在右边
                    else if (Left > swidth - size / 2)
                    {

                        for (int i = 0; i < 16; i++)
                        {
                            sleeptime = 10;
                            displayimg("48", -2, 0);
                        }
                    }
                    //如果在下面
                    else if (Top > sheight - size / 1.1)
                    {
                        for (int i = 0; i < 24; i++)
                        {
                            sleeptime = 10;
                            displayimg("48", 0, -2);
                        }
                    }
                    //如果在左边
                    else if (Left < 0 - size / 2)
                    {
                        for (int i = 0; i < 24; i++)
                        {
                            sleeptime = 10;
                            displayimg("48", 2, 0);
                        }
                    }
                    //如果在上边
                    else if (Top < 0 - size / 4)
                    {
                        for (int i = 0; i < 16; i++)
                        {
                            sleeptime = 10;
                            displayimg("48", 0, 2);
                        }
                    }
                    isAction = true;
                    sleeptime = 500;
                    pic.IsHitTestVisible = true;
                }));
            }
        }//关于出界的方法
        private void mouseMove(object sender, EventArgs e)//关于鼠标移动
        {
            if (mainwindow.isClick)
            {
                Image pic = mainwindow.pic;
                //鼠标停止的
                if (mousePointX0 - mousePointX1 == 0)
                {
                    movenon++;
                    if (movenon > 63)
                    {
                        pic.Source = new BitmapImage(new Uri("/img/frame/shime1.png", UriKind.Relative));
                        movenon = 0;
                    }
                }
                //判定鼠标移动速度是否很慢
                else if (Math.Abs(mousePointX0 - mousePointX1) < 4)
                {
                    slowdown++;
                    if (slowdown > 31)
                    {
                        pic.Source = new BitmapImage(new Uri("/img/frame/shime1.png", UriKind.Relative));
                    }
                }
                //拖动时判定鼠标左右方向
                else
                {
                    slowdown = 0;
                    //鼠标向左
                    if (mousePointX1 - mousePointX0 < 0)
                    {
                        pic.Source = new BitmapImage(new Uri("/img/frame/shime10.png", UriKind.Relative));
                    }
                    //鼠标向右
                    if (mousePointX1 - mousePointX0 > 0)
                    {
                        pic.Source = new BitmapImage(new Uri("/img/frame/shime9.png", UriKind.Relative));
                    }
                }
            }
        }
        private void checkMousePoint(object sender, EventArgs e)
        {
            if (mainwindow.isClick)
            {
                //获取两次鼠标的横坐标，X0，XY
                mousePointX0 = System.Windows.Forms.Cursor.Position.X;
                //int mousePointY0 = System.Windows.Forms.Cursor.Position.Y;
                Thread.Sleep(1);
                mousePointX1 = System.Windows.Forms.Cursor.Position.X;
                //int mousePointY1 = System.Windows.Forms.Cursor.Position.Y;
            }
        }
        private void displayctrl()
        {
            if (mainwindow.isClick==false)
            {
                if (isAction)
                {
                    Timer.Stop();
                    int way = new Random().Next(0, 50);
                    //way = 2;
                    switch (way)
                    {
                        case 0:
                            sleeptime = 300;
                            for (int i = 0; i < 2; i++)
                            {
                                displayimg("1", 2, 0);
                                displayimg("2", 2, 0);
                                displayimg("1", 2, 0);
                                displayimg("3", 2, 0);
                            }
                            break;
                        case 1:
                            sleeptime = 100;
                            for (int i = 0; i < 2; i++)
                            {
                                displayimg("1", 4, 0);
                                displayimg("2", 4, 0);
                                displayimg("1", 4, 0);
                                displayimg("3", 4, 0);
                            }
                            break;
                        case 2:
                            sleeptime = 100;
                            for (int i = 0; i < 2; i++)
                            {
                                displayimg("1", 8, 0);
                                displayimg("2", 8, 0);
                                displayimg("1", 8, 0);
                                displayimg("3", 8, 0);
                            }
                            break;
                        case 3:
                            sleeptime = 12500;
                            displayimg("11", 0, 0);
                            break;
                        case 4:
                            sleeptime = 12500;
                            displayimg("26", 0, 0);
                            break;
                        case 5:
                            sleeptime = 12500;
                            displayimg("11", 0, 0);
                            sleeptime = 6250;
                            displayimg("26", 0, 0);
                            break;
                        case 6:
                            sleeptime = 250;
                            displayimg("26", 0, 0);
                            displayimg("15", 0, 0);
                            displayimg("27", 0, 0);
                            displayimg("16", 0, 0);
                            displayimg("28", 0, 0);
                            displayimg("17", 0, 0);
                            displayimg("29", 0, 0);
                            displayimg("11", 0, 0);
                            break;
                        case 7:
                            sleeptime = 12500;
                            displayimg("30", 0, 0);
                            break;
                        case 8:
                            sleeptime = 12500;
                            displayimg("31", 0, 0);
                            break;
                        case 9:
                            sleeptime = 1250;
                            displayimg("30", 0, 0);
                            displayimg("31", 0, 0);
                            break;
                        case 10:
                            sleeptime = 250;
                            displayimg("31", 0, 0);
                            sleeptime = 750;
                            displayimg("32", 0, 0);
                            sleeptime = 250;
                            displayimg("31", 0, 0);
                            sleeptime = 750;
                            displayimg("33", 0, 0);
                            break;
                        case 11:
                            sleeptime = 12500;
                            displayimg("21", 0, 0);
                            break;
                        case 12:
                            sleeptime = 1400;
                            displayimg("20", 0, 0);
                            sleeptime = 200;
                            displayimg("20", 2, 0);
                            displayimg("21", 2, 0);
                            displayimg("21", 1, 0);
                            sleeptime = 1200;
                            displayimg("21", 0, 0);
                            break;
                        case 13:
                            sleeptime = 12500;
                            displayimg("23", 0, 0);
                            break;
                        case 14:
                            sleeptime = 800;
                            displayimg("25", 0, 0);
                            sleeptime = 200;
                            displayimg("25", 1, 0);
                            displayimg("23", 1, 0);
                            displayimg("24", 1, 0);
                            sleeptime = 800;
                            displayimg("24", 0, 0);
                            sleeptime = 200;
                            displayimg("24", 2, 0);
                            displayimg("23", 2, 0);
                            displayimg("25", 2, 0);
                            break;
                        case 15:
                            sleeptime = 12500;
                            displayimg("13", 0, 0);
                            break;
                        case 16:
                            sleeptime = 800;
                            displayimg("14", 0, 0);
                            sleeptime = 200;
                            displayimg("14", 0, 1);
                            displayimg("12", 0, 1);
                            displayimg("13", 0, 1);
                            sleeptime = 800;
                            displayimg("13", 0, 0);
                            sleeptime = 200;
                            displayimg("13", 0, 2);
                            displayimg("12", 0, 2);
                            displayimg("14", 0, 2);
                            break;
                        case 17:
                            sleeptime = 800;
                            displayimg("14", 0, 0);
                            sleeptime = 200;
                            displayimg("14", 0, -2);
                            displayimg("12", 0, -2);
                            displayimg("13", 0, -2);
                            sleeptime = 800;
                            displayimg("13", 0, 0);
                            sleeptime = 200;
                            displayimg("13", 0, -1);
                            displayimg("12", 0, -1);
                            displayimg("14", 0, -1);
                            break;
                        //case 18:
                        //sleeptime = 12500;
                        //displayimg("36", 0, 0);
                        //displayimg("34", 0, 0);
                        //displayimg("35", 0, 0);
                        //displayimg("34", 0, 0);
                        //displayimg("36", 0, 0);
                        //break;
                        //case 19:
                        //sleeptime = 300;
                        //displayimg("36", 0, 0);
                        //break;
                        case 20:
                            sleeptime = 2000;
                            displayimg("37", 0, 0);
                            break;
                        case 21:
                            sleeptime = 12500;
                            displayimg("22", 0, 0);
                            break;
                        //case 22:
                        //        sleeptime = 50;
                        //        for (int i = 0; i < 20; i++) 
                        //        {
                        //            displayimg("4", 0, 2);
                        //        } 
                        //        break;
                        case 23:
                            sleeptime = 200;
                            displayimg("18", 0, 0);
                            displayimg("19", 0, 0);
                            break;
                        case 24:
                            sleeptime = 400;
                            displayimg("19", 8, 0);
                            sleeptime = 200;
                            displayimg("18", 4, 0);
                            displayimg("20", 2, 0);
                            sleeptime = 500;
                            displayimg("20", 0, 0);
                            sleeptime = 200;
                            displayimg("19", 4, 0);
                            break;
                        case 25:
                            sleeptime = 250;
                            displayimg("9", 0, 0);
                            break;
                        case 26:
                            sleeptime = 250;
                            displayimg("7", 0, 0);
                            break;
                        case 27:
                            sleeptime = 250;
                            displayimg("1", 0, 0);
                            break;
                        case 28:
                            sleeptime = 250;
                            displayimg("8", 0, 0);
                            break;
                        case 29:
                            sleeptime = 250;
                            displayimg("10", 0, 0);
                            break;
                        case 30:
                            sleeptime = 250;
                            displayimg("5", 0, 0);
                            displayimg("6", 0, 0);
                            displayimg("5", 0, 0);
                            displayimg("6", 0, 0);
                            sleeptime = 2500;
                            displayimg("1", 0, 0);
                            sleeptime = 250;
                            for (int i = 0; i < 4; i++)
                            {
                                displayimg("5", 0, 0);
                                displayimg("6", 0, 0);
                            }
                            sleeptime = 5000;
                            displayimg("1", 0, 0);
                            sleeptime = 250;
                            for (int i = 0; i < 4; i++)
                            {
                                displayimg("5", 0, 0);
                                displayimg("6", 0, 0);
                            }
                            sleeptime = 100;
                            for (int i = 0; i < 4; i++)
                            {
                                displayimg("5", 0, 0);
                                displayimg("6", 0, 0);
                            }
                            break;
                        case 31:
                            sleeptime = 800;
                            displayimg("1", 0, 0);
                            sleeptime = 200;
                            displayimg("38", 0, 0);
                            sleeptime = 2000;
                            displayimg("39", 0, 0);
                            displayimg("40", 0, 0);
                            displayimg("41", 0, 0);
                            break;
                        case 32:
                            sleeptime = 50;
                            displayimg("9", -20, -20);
                            displayimg("9", -20, -10);
                            displayimg("9", -20, -5);
                            break;
                        case 33:
                            sleeptime = 250;
                            displayimg("42", 0, 0);
                            sleeptime = 100;
                            displayimg("43", 0, 0);
                            displayimg("44", 0, 0);
                            sleeptime = 250;
                            displayimg("45", 0, 0);
                            sleeptime = 1000;
                            displayimg("46", 0, 0);
                            break;
                        default:
                            displayimg("1", 0, 0);
                            break;
                    }
                    Timer.Start();
                    sleeptime = 500;
                }
            }
        }//图片控制，里面的内容非常无聊
        private void displayimg(string imgnum, int x, int y)
        {
            Image pic = mainwindow.pic;
            double size = mainwindow.size;
            //没有点击时
            if (!mainwindow.isClick)
            {
                //对文件名的操作
                string img = "/img/frame/shime" + imgnum + ".png";
                //pic.Dispatcher.Invoke(new Action(delegate 
                //    {

                //    }));
                //上面注释的是为了解决其他线程占用问题
                pic.Dispatcher.Invoke(new Action(delegate { pic.Source = new BitmapImage(new Uri(img, UriKind.Relative)); }));
                DoEvents();
                x = x * 3;
                y = y * 3;
                pic.Dispatcher.Invoke(new Action(delegate
                {
                    double Left = (double)pic.GetValue(Canvas.LeftProperty);
                    double Top = (double)pic.GetValue(Canvas.TopProperty);
                    //这里出界的判定还是留着吧
                    //if (Left > swidth - size | Top > sheight - size | Left < 0 | Top < 0)
                    //{
                    //    pic.SetValue(Canvas.LeftProperty, (double)mainwindow.rx);
                    //    pic.SetValue(Canvas.TopProperty, (double)mainwindow.ry);
                    //}
                    //else
                    //{
                        pic.SetValue(Canvas.LeftProperty, (double)Left + x);
                        pic.SetValue(Canvas.TopProperty, (double)Top + y);
                    //}
                }));
                DoEvents();
            } 
        }//更换图片的方法
        public static void DoEvents()
        {
            DispatcherFrame frame = new DispatcherFrame(true);
            Dispatcher.CurrentDispatcher.BeginInvoke(DispatcherPriority.Background, (SendOrPostCallback)delegate(object arg)
            {
                DispatcherFrame fr = arg as DispatcherFrame;
                fr.Continue = false;
            }, frame);
            Dispatcher.PushFrame(frame);
            Thread.Sleep(sleeptime);
        }//这里是写一个WF的DoEvents()
        public static void doevents()
        {
            DispatcherFrame frame = new DispatcherFrame(true);
            Dispatcher.CurrentDispatcher.BeginInvoke(DispatcherPriority.Background, (SendOrPostCallback)delegate(object arg)
            {
                DispatcherFrame fr = arg as DispatcherFrame;
                fr.Continue = false;
            }, frame);
            Dispatcher.PushFrame(frame);
        }//这里的doevents是没有线程暂停的，留着备用
        public void mouseleftdown()
        {
            mainwindow.isClick = true;
            skldown++;
            Image pic = mainwindow.pic;
            pic.Source = new BitmapImage(new Uri("/img/frame/shime18.png", UriKind.Relative));
            if (skldown > 3)
            {
                mainwindow.updatestate("skl", -1);
                skldown = 0;
            }
        }
        public void mouseleftup()
        {
            mainwindow.isClick = false;
            //鼠标放开时换成下面的图片
            Image pic = mainwindow.pic;
            pic.Source = new BitmapImage(new Uri("/img/frame/shime1.png", UriKind.Relative));
        }
    }
}