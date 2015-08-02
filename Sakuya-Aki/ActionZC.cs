using System;

namespace Sakuya_Aki
{
    class ActionZC
    {
        MainWindow mainwindow;

        public ActionZC(MainWindow mainwindow)
        {
            this.mainwindow = mainwindow;
        }
        public void Game0(int num)
        {
            //Console.WriteLine("Game" + Akix);
            Random rd = new Random();
            int result = rd.Next(0, 3);
            //改判
            int Sakuya = rd.Next(0, 8);
            //Sakuya = 5;
            if (Sakuya <= 3)
            {

            }
            if (Sakuya > 3)
            {
                switch (num)
                {
                    case 0:
                        result = 2;
                        break;
                    case 1:
                        result = 0;
                        break;
                    case 2:
                        result = 1;
                        break;
                }
            }
            string set = " ";
            string words = " ";
            if (mainwindow.skl >= 0)
            {
                switch (result)
                {
                    case 0:
                        set = "石头\n";
                        switch (num)
                        {
                            case 0:
                                words = "平手了呢";
                                break;
                            case 1:
                                words = "Master 输了耶~~~~~";
                                break;
                            case 2:
                                words = "桜酱输了的说-，-";
                                break;
                        }
                        break;
                    case 1:
                        set = "剪刀\n";
                        switch (num)
                        {
                            case 0:
                                words = "桜酱输了的说-，-";
                                break;
                            case 1:
                                words = "平手了呢";
                                break;
                            case 2:
                                words = "Master 输了耶~~~~~";
                                break;
                        }
                        break;
                    case 2:
                        set = "布\n";
                        switch (num)
                        {
                            case 0:
                                words = "Master 输了耶~~~~~";
                                break;
                            case 1:
                                words = "桜酱输了的说-，-";
                                break;
                            case 2:
                                words = "平手了呢";
                                break;
                        }
                        break;
                }
                mainwindow.displaytips("桜酱出了" + set + words, 3000);
                mainwindow.updatestate("skl", 1);
            }
            else
            {
                mainwindow.displaytips("桜才不跟你玩呢", 5000);
            }
        }//石头剪刀布
    }
}
