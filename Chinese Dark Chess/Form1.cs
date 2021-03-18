using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using System.Diagnostics;

namespace CDC
{
    //This Chinese Dark Chess Experiment Platform is designed by Aoshuang YE in April 2017
    //AI part hope for Monte-Carlo Tree Search But still used Evaluation Function in version 1.00
    //Version 2.00 uses Alpha-Beta for AI search algorithm.
    //version 1.00 updated 24/04/17
    //version 2.00 updated 30/08/17

    public partial class Form1 : Form
    {
        Map vmap = new Map();
        int chozenX;
        int chozenY;
        static int onedice = 0;//用于决定AI的行动顺序
        Move movtion = new Move();
        Random rand = new Random();
        nodes clicknode = new nodes();
        List<double> difference = new List<double>();
        List<double> allB = new List<double>();
        List<double> calB = new List<double>();
        List<int> win_jiang = new List<int>();
        List<int> win_zu = new List<int>();
        List<int> win_pao = new List<int>();
        List<int> lose_jiang = new List<int>();
        List<int> lose_zu = new List<int>();
        List<int> lose_pao = new List<int>();
        Stopwatch timer = new Stopwatch();//Creating timer
        List<double> Allturn = new List<double>();
        List<List<double>> gameresult = new List<List<double>>();

        public Form1()
        {
            InitializeComponent();

            for (int i = 0; i < 4; i++)
            {
                for (int j = 0; j < 8; j++)
                {
                    Control[] pic = this.Controls.Find("pictureBox" + (i * 8 + j + 1), false);
                    vmap.Matrix[i, j].container = pic[0] as PictureBox;
                }
            }
            AI_tools.resetground(vmap);//重置棋盘
            Judge_system(vmap);
        }

        private void click(object sender, EventArgs e)//翻棋子事件
        {
            string name = (sender as PictureBox).Name;//名字
            string number = name.Substring(10);//字符串第十位开始截取，即截取编号
            int index = Convert.ToInt32(number);//编号转换字符串为整形
            int i, j;
            int currentcolor = 2;
            i = (index - 1) / 8;//被选择的行
            j = (index - 1) % 8;//被选择的列


            if (vmap.Matrix[i, j].flip == 0)//点击对象为未翻开的对象时翻棋
            {
                if (vmap.turncount == 1)//一局开始
                {
                    vmap.firstplayer = vmap.Matrix[i, j].item.side;//第一个棋子即为第一玩家的颜色
                    vmap.secondplayer = (vmap.Matrix[i, j].item.side + 1) % 2;
                    display(vmap.firstplayer);
                }
                clicknode.movtion.froX = i;
                clicknode.movtion.froY = j;
                atkandmove(vmap, clicknode);
            }
            else if (vmap.Matrix[i, j].flip == 1)//点击的对象为已翻开的棋子
            {
                if (vmap.currentside == 0)//为先手方的时候
                {
                    currentcolor = vmap.firstplayer;
                }
                else if (vmap.currentside == 1)//为后手方的时候
                {
                    currentcolor = vmap.secondplayer;
                }
                if (vmap.Matrix[i, j].item.side == currentcolor)//为本方棋子的时候
                {
                    clicknode.movtion.froX = i;
                    clicknode.movtion.froY = j;
                    return;
                }
                if (vmap.Matrix[i, j].item.side == (currentcolor + 1) % 2 || vmap.Matrix[i, j].item.side == 2)//为对方棋子的时候或者空的时候
                {
                    clicknode.movtion.desX = i;
                    clicknode.movtion.desY = j;
                }
                atkandmove(vmap, clicknode);
                score(vmap);
                if (checkboard(clicknode, vmap) != 0)
                {
                    AI_tools.resetground(vmap);
                    MessageBox.Show("Game End!");
                }

            }
        }

        private void setmove(Map map, Move movtion)//放子
        {
            map.Matrix[movtion.desX, movtion.desY].container.Image = map.Matrix[movtion.froX, movtion.froY].container.Image;//替换坐标图象
            map.Matrix[movtion.desX, movtion.desY].item = map.Matrix[movtion.froX, movtion.froY].item;
            map.Matrix[movtion.froX, movtion.froY].container.Image = null;//原来的位置变为空
            map.Matrix[movtion.froX, movtion.froY].item.side = 2;//原来位置side空置，2为空
            map.Matrix[movtion.froX, movtion.froY].item.type = chesstype.blank;//原来位置棋子类型控制
            map.Matrix[movtion.froX, movtion.froY].flip = 1;
            movtion.froX = -1;
            movtion.froY = -1;
            movtion.desX = -1;
            movtion.desY = -1;//清空movtion内数据
            map.turncount += 1;
            map.currentside = (map.currentside + 1) % 2;//已落子换方
        }

        private bool deloop(nodes node, Map map)
        {
            bool value = false;
            int n = 0;
            if (map.Plyr_move.Count >= 6)
            {
                for (int i = map.Plyr_move.Count - 6; i < map.Plyr_move.Count - 1; i++)//重复度检查
                {
                    if (map.Plyr_move[i].movtion.froX == node.movtion.froX && map.Plyr_move[i].movtion.froY == node.movtion.froY
                    && map.Plyr_move[i].movtion.desX == node.movtion.desX && map.Plyr_move[i].movtion.desY == node.movtion.desY)
                    {
                        n += 1;
                    }
                }
            }

            if (n >= 3)//多次重复
            {
                value = true;
                Console.WriteLine("Repeation Draw!!!!");
            }            
            return value;
        }

    private int checkboard(nodes node, Map map)
        {
            int flag = 0;//0为游戏未结束， 1为游戏结束
            int redCount = 0;
            int blueCount = 0;
            for (int i = 0; i < 4; i++)
            {
                for (int j = 0; j < 8; j++)
                {
                    if (map.Matrix[i, j].item.side == 0)//为红方
                    {
                        redCount++;//红加1
                    }
                    if (map.Matrix[i, j].item.side == 1)//为蓝方
                    {
                        blueCount++;//蓝加1
                    }
                }
            }
            if(node.resign == 1)
            {
                if((map.currentside == 0 && map.firstplayer == 0) || (map.currentside == 1 && map.firstplayer == 1))
                {
                    flag = 1;
                    Console.WriteLine(map.turncount);
                    textBox1.AppendText("Blue Win !" + "\r\n");
                }
                else if((map.currentside == 1 && map.firstplayer == 0) || (map.currentside == 0 && map.firstplayer == 1))
                {
                    flag = 2;
                    Console.WriteLine(map.turncount);
                    textBox1.AppendText("Red Win !" + "\r\n");
                }
            }
            if (redCount == 0)//Blue win!
            {
                flag = 1;
                Console.WriteLine(map.turncount);
                textBox1.AppendText("Blue Win !" + "\r\n");
            }
            else if (blueCount == 0)//Red win!
            {
                flag = 2;
                Console.WriteLine(map.turncount);
                textBox1.AppendText("Red Win !" + "\r\n");
            }
            else if(deloop(node, map) == true)
            {
                flag = 3;
                textBox1.AppendText("Draw" + "\r\n");
            }
            else if (map.turncount >= 150)
            {
                flag = 4;
                textBox1.AppendText("Draw" + "\r\n");
            }
            else
            {
                flag = Judge_system(map);
            }//采用最终结束式


            textBox1.AppendText("Turn Count: " + (map.turncount - 1) + "\r\n");
            textBox1.AppendText("Red Jiang Remains: " + map.redjiang + "\r\n");
            textBox1.AppendText("Red Shi Remains: " + map.redshi + "\r\n");
            textBox1.AppendText("Red Che Remains: " + map.redche + "\r\n");
            textBox1.AppendText("Red Ma Remains: " + map.redma + "\r\n");
            textBox1.AppendText("Red Pao Remains: " + map.redpao + "\r\n");
            textBox1.AppendText("Red Zu Remains: " + map.redzu + "\r\n");
            textBox1.AppendText("Red Xiang Remians: " + map.redxiang + "\r\n");
            textBox1.AppendText("Blue Jiang Remains: " + map.bluejiang + "\r\n");
            textBox1.AppendText("Blue Shi Remains: " + map.blueshi + "\r\n");
            textBox1.AppendText("Blue Che Remains: " + map.blueche + "\r\n");
            textBox1.AppendText("Blue Ma Remains: " + map.bluema + "\r\n");
            textBox1.AppendText("Blue Pao Remains: " + map.bluepao + "\r\n");
            textBox1.AppendText("Blue Zu Remains: " + map.bluezu + "\r\n");
            textBox1.AppendText("Blue Xiang Remians: " + map.bluexiang + "\r\n");
            textBox1.AppendText("Red Pieces Remaining: " + map.redremain + "\r\n");
            textBox1.AppendText("Blue Pieces Remaining: " + map.blueremain + "\r\n");
            textBox1.AppendText("\r\n");
            textBox1.AppendText("\r\n");
            if(flag == 1)//蓝方胜
            {
                lose_jiang.Add(map.redjiang);
                lose_zu.Add(map.redzu);
                lose_pao.Add(map.redpao);
            }
            else if (flag == 2)//红方胜
            {
                win_jiang.Add(map.redjiang);
                win_zu.Add(map.redzu);
                win_pao.Add(map.redpao);
            }
            return flag;
        }

        private int Judge_system(Map map)
        {
            //初始化各项数值，皆初始化为零
            //以下为红方
            map.redjiang = 0;
            map.redshi = 0;
            map.redxiang = 0;
            map.redche = 0;
            map.redma = 0;
            map.redpao = 0;
            map.redzu = 0;
            map.redremain = 0;
            map.unflipp_red = 0;
            //以下为蓝方
            map.bluejiang = 0;
            map.blueshi = 0;
            map.bluexiang = 0;
            map.blueche = 0;
            map.bluema = 0;
            map.bluepao = 0;
            map.bluezu = 0;
            map.topred = 0;//用于代表红方位阶最高棋子
            map.topblue = 0;//用于代表蓝方位阶最高棋子
            map.blueremain = 0;
            map.unflipp_blue = 0;
            int value = 0;
            //将位阶5，士位阶4，象位阶3，车位阶2，马位阶1，卒位阶0
            for (int i = 0; i < 4; i++)
            {
                for (int j = 0; j < 8; j++)
                {
                    if (map.Matrix[i, j].item.side == 1)//蓝方
                    {
                        if(map.Matrix[i, j].flip == 0)
                        {
                            map.unflipp_blue += 1;
                        }
                        switch (map.Matrix[i, j].item.type)
                        {
                            case chesstype.che:
                                map.blueche += 1;
                                map.blueremain += 1;
                                if (map.topblue <= 2)
                                {
                                    map.topblue = 2;
                                }
                                break;
                            case chesstype.ma:
                                map.bluema += 1;
                                map.blueremain += 1;
                                if (map.topblue <= 1)
                                {
                                    map.topblue = 1;
                                }
                                break;
                            case chesstype.jiang:
                                map.bluejiang += 1;
                                map.blueremain += 1;
                                break;
                            case chesstype.pao:
                                map.bluepao += 1;
                                map.blueremain += 1;
                                if (map.topred <= 0)
                                {
                                    map.topred = 0;
                                }
                                break;
                            case chesstype.zu:
                                map.bluezu += 1;
                                map.blueremain += 1;
                                if (map.topblue <= 0)
                                {
                                    map.topblue = 0;
                                }
                                break;
                            case chesstype.shi:
                                map.blueshi += 1;
                                map.blueremain += 1;
                                if (map.topblue <= 4)
                                {
                                    map.topblue = 4;
                                }
                                break;
                            case chesstype.xiang:
                                map.bluexiang += 1;
                                map.blueremain += 1;
                                if (map.topblue <= 3)
                                {
                                    map.topblue = 3;
                                }
                                break;
                        }
                    }

                    if (map.Matrix[i, j].item.side == 0)//红方
                    {
                        if (map.Matrix[i, j].flip == 0)
                        {
                            map.unflipp_red += 1;
                        }
                        switch (map.Matrix[i, j].item.type)
                        {
                            case chesstype.che:
                                map.redche += 1;
                                map.redremain += 1;
                                if (map.topred <= 2)
                                {
                                    map.topred = 2;
                                }
                                break;
                            case chesstype.ma:
                                map.redma += 1;
                                map.redremain += 1;
                                if (map.topred <= 1)
                                {
                                    map.topred = 1;
                                }
                                break;
                            case chesstype.jiang:
                                map.redjiang += 1;
                                map.redremain += 1;
                                break;
                            case chesstype.pao:
                                map.redpao += 1;
                                map.redremain += 1;
                                if (map.topred <= 0)
                                {
                                    map.topred = 0;
                                }
                                break;
                            case chesstype.zu:
                                map.redzu += 1;
                                map.redremain += 1;
                                if (map.topred <= 0)
                                {
                                    map.topred = 0;
                                }
                                break;
                            case chesstype.shi:
                                map.redshi += 1;
                                map.redremain += 1;
                                if (map.topred <= 4)
                                {
                                    map.topred = 4;
                                }
                                break;
                            case chesstype.xiang:
                                map.redxiang += 1;
                                map.redremain += 1;
                                if (map.topred <= 3)
                                {
                                    map.topred = 3;
                                }
                                break;
                        }
                    }
                }
            }

           if (map.turncount != 1)//Classification Model
            { //决策树生成分类模型
              //对红方
              //最高位阶不为将的情况
                if (map.topblue > map.topred && map.redpao != 2 && map.bluejiang == 0 && map.redjiang == 0)
                {
                    value = 1;//蓝方胜
                    Console.WriteLine("提前结束： 红方最高位阶小于蓝方投降");
                }//sc2
                if (map.topred > map.topblue && map.bluepao != 2 && map.redjiang == 0 && map.bluejiang == 0)
                {
                    value = 2;//红方胜
                    Console.WriteLine("提前结束： 蓝方最高位阶小于红方投降");
                }//sc2
                if (map.topred == 4 && map.topblue == 4 && map.redshi == 1 && map.blueshi == 1 && map.redpao == 0 && map.bluepao == 0)
                {
                    value = 3;
                    Console.WriteLine("提前结束： 双方无法达成将死条件（最高为士）");
                }//4
                if (map.topred == 3 && map.topblue == 3 && map.redxiang == 1 && map.bluexiang == 1 && map.redpao == 0 && map.bluepao == 0)
                {
                    value = 3;
                    Console.WriteLine("提前结束： 双方无法达成将死条件（最高为象）");
                }//4
                if (map.topred == 2 && map.topblue == 2 && map.redche == 1 && map.blueche == 1 && map.redpao == 0 && map.bluepao == 0)
                {
                    value = 3;
                    Console.WriteLine("提前结束： 双方无法达成将死条件（最高为车）");
                }//4
                if (map.topred == 1 && map.topblue == 1 && map.redma == 1 && map.bluema == 1 && map.redpao == 0 && map.bluepao == 0)
                {
                    value = 3;
                    Console.WriteLine("提前结束： 双方无法达成将死条件（最高为马）");
                }//4
                if (map.topred == 0 && map.topblue == 0 && map.redzu == 1 && map.bluezu == 1 && map.redpao == 0 && map.bluepao == 0)
                {
                    value = 3;
                    Console.WriteLine("提前结束： 双方无法达成将死条件（最高为卒）");
                }//4
                if (map.redremain == 1 && map.blueremain == 1)
                {
                    value = 3;
                    Console.WriteLine("提前结束： 双方无法达成将死条件（两子和棋）");
                }
                if (map.topred == 4 && map.topblue == 4 && map.redpao == 0 && map.bluepao == 0 && map.redshi == 1 && map.blueshi == 2)
                {
                    value = 1;
                    Console.WriteLine("提前结束： 达成将死条件（蓝方胜）");
                }
                if (map.topred == 4 && map.topblue == 4 && map.redpao == 0 && map.bluepao == 0 && map.redshi == 2 && map.blueshi == 1)
                {
                    value = 2;
                    Console.WriteLine("提前结束： 达成将死条件（红方胜）");
                }
                if (map.topred == 3 && map.topblue == 3 && map.redpao == 0 && map.bluepao == 0 && map.redxiang == 1 && map.bluexiang == 2)
                {
                    value = 1;
                    Console.WriteLine("提前结束： 达成将死条件（蓝方胜）");
                }
                if (map.topred == 3 && map.topblue == 3 && map.redpao == 0 && map.bluepao == 0 && map.redxiang == 2 && map.bluexiang == 1)
                {
                    value = 2;
                    Console.WriteLine("提前结束： 达成将死条件（红方胜）");
                }
                if (map.topred == 2 && map.topblue == 2 && map.redpao == 0 && map.bluepao == 0 && map.redche == 1 && map.blueche == 2)
                {
                    value = 1;
                    Console.WriteLine("提前结束： 达成将死条件（蓝方胜）");
                }
                if (map.topred == 2 && map.topblue == 2 && map.redpao == 0 && map.bluepao == 0 && map.redche == 2 && map.blueche == 1)
                {
                    value = 2;
                    Console.WriteLine("提前结束： 达成将死条件（红方胜）");
                }
                if (map.topred == 1 && map.topblue == 1 && map.redpao == 0 && map.bluepao == 0 && map.redma == 1 && map.bluema == 2)
                {
                    value = 1;
                    Console.WriteLine("提前结束： 达成将死条件（蓝方胜）");
                }
                if (map.topred == 1 && map.topblue == 1 && map.redpao == 0 && map.bluepao == 0 && map.redma == 2 && map.bluema == 1)
                {
                    value = 2;
                    Console.WriteLine("提前结束： 达成将死条件（红方胜）");
                }
                if (map.topred == 0 && map.topblue == 0 && map.redpao == 0 && map.bluepao == 0 && map.redzu == 1 && map.bluezu == 2)
                {
                    value = 1;
                    Console.WriteLine("提前结束： 达成将死条件（蓝方胜）");
                }
                if (map.topred == 0 && map.topblue == 0 && map.redpao == 0 && map.bluepao == 0 && map.redzu == 2 && map.bluezu == 1)
                {
                    value = 2;
                    Console.WriteLine("提前结束： 达成将死条件（红方胜）");
                }
                if (map.topblue > map.topred && map.bluejiang == 0 && map.redpao == 0 && map.blueremain >= 2)
                {
                    value = 1;//蓝方胜
                    Console.WriteLine("提前结束： 蓝方胜");
                }
                if (map.topred > map.topblue && map.redjiang == 0 && map.bluepao == 0 && map.redremain >= 2)
                {
                    value = 2;//红方胜
                    Console.WriteLine("提前结束： 红方胜");
                }


                //最高位阶为将//
                if (map.bluejiang == 1 && ((map.redzu <= 1 && map.redpao == 0) || (map.redzu == 0 && map.redpao <= 1)) && map.redjiang == 0)
                {
                    value = 1;
                    Console.WriteLine("提前结束： 红方因无法吃掉蓝方将投降");
                }//sc1
                if (map.redjiang == 1 && ((map.bluezu <= 1 && map.bluepao == 0) || (map.bluezu == 0 && map.bluepao <= 1)) && map.bluejiang == 0)
                {
                    value = 2;
                    Console.WriteLine("提前结束： 蓝方因无法吃掉红方将投降");
                }//sc1
                if (map.redjiang == 1 && map.bluejiang == 1 && map.redpao == 0 && map.bluepao == 0 && map.redzu == 0 && map.bluezu == 0)
                {
                    value = 3;
                    Console.WriteLine("提前结束： 双方无法达成将死条件（最高为将）");
                }
                if (map.redjiang == 1 && map.bluejiang == 0 && map.bluezu == 1 && map.redremain == 1 && map.blueremain == 1)
                {
                    value = 3;
                    Console.WriteLine("提前结束： 双方无法达成将死条件（一将一卒）");
                }//
                if (map.redjiang == 0 && map.bluejiang == 1 && map.redzu == 1 && map.redremain == 1 && map.blueremain == 1)
                {
                    value = 3;
                    Console.WriteLine("提前结束： 双方无法达成将死条件（一将一卒）");
                }
                if (map.redjiang == 1 && map.bluejiang == 1 && map.redremain == 1 && map.blueremain > 1)
                {
                    value = 1;
                    Console.WriteLine("提前结束： 达成将死条件（蓝方胜）");
                }
                if (map.redjiang == 1 && map.bluejiang == 1 && map.redremain > 1 && map.blueremain == 1)
                {
                    value = 2;
                    Console.WriteLine("提前结束： 达成将死条件（红方胜）");
                }
                if (map.bluejiang == 1 && map.topred == 0 && map.redpao == 0 && map.blueremain >= 2)
                {
                    value = 1;//蓝方胜
                    Console.WriteLine("提前结束： 红方卒被将死");
                }
                if (map.redjiang == 1 && map.topblue == 0 && map.bluepao == 0 && map.redremain >= 2)
                {
                    value = 2;//红方胜
                    Console.WriteLine("提前结束： 蓝方卒被将死");
                }
                if (map.redjiang == 1 && map.bluejiang == 0 && (map.bluezu >= 1 || map.bluepao >= 1) && map.topred > map.topblue)
                {
                    value = 2;
                    Console.WriteLine("提前结束： 达成将死条件（红方胜）");
                }
                if (map.bluejiang == 1 && map.redjiang == 0 && (map.redzu >= 1 || map.redpao >= 1) && map.topblue > map.topred)
                {
                    value = 1;
                    Console.WriteLine("提前结束： 达成将死条件（蓝方胜）");
                }
                if (map.redremain == 1 && map.blueremain >= 1 && map.topred > map.topblue)
                {
                    value = 3;//You can't capture opponents' piece
                }
                if (map.blueremain == 1 && map.redremain >= 1 && map.topblue > map.topred)
                {
                    value = 3;//You can't capture opponents' piece
                }

                if (map.redjiang == 1)
                {
                    if (map.redpao < 1)
                    {
                        value = 1;//Blue win
                    }
                }
                else if (map.redjiang == 0)
                {
                    if (map.redzu < 2)
                    {
                        value = 1;
                    }
                }

                if(map.bluejiang == 1)
                {
                    if (map.bluepao < 1)
                    {
                        value = 2;//Red win
                    }
                }
                else if (map.bluejiang == 0)
                {
                    if (map.bluezu < 2)
                    {
                        value = 2;
                    }
                }
            }
            return value;
        }

        private void cal_Proba(Map map)
        {
            int unflip_blue_che = map.blueche;
            int unflip_blue_ma = map.bluema;
            int unflip_blue_jiang = map.bluejiang;
            int unflip_blue_shi = map.blueshi;
            int unflip_blue_xiang = map.bluexiang;
            int unflip_blue_zu = map.bluezu;
            int unflip_blue_pao = map.bluepao;
            int unflip_red_che = map.redche;
            int unflip_red_ma = map.redma;
            int unflip_red_jiang = map.redjiang;
            int unflip_red_shi = map.redshi;
            int unflip_red_xiang = map.redxiang;
            int unflip_red_zu = map.redzu;
            int unflip_red_pao = map.redpao;
            simap sim_map = new simap();
            sim_map = sim_map.createDeepClone(map);
            List<Move> flipping = new List<Move>();
            flipping = AI_tools.getallfliplist(sim_map);
            int all_unflip = flipping.Count;
            
            for (int i = 0; i < 4; i++)
            {
                for (int j = 0; j < 8; j++)
                {
                    if ((map.Matrix[i, j].item.side == 1)&& (map.Matrix[i, j].flip == 1))//蓝方
                    {
                        switch (map.Matrix[i, j].item.type)
                        {
                            case chesstype.che:
                                unflip_blue_che -= 1;
                                break;
                            case chesstype.ma:
                                unflip_blue_ma -= 1;
                                break;
                            case chesstype.jiang:
                                unflip_blue_jiang -= 1;
                                break;
                            case chesstype.pao:
                                unflip_blue_pao -= 1;
                                break;
                            case chesstype.zu:
                                unflip_blue_zu -= 1;
                                break;
                            case chesstype.shi:
                                unflip_blue_shi -= 1;
                                break;
                            case chesstype.xiang:
                                unflip_blue_xiang -= 1;
                                break;
                        }
                    }

                    if (map.Matrix[i, j].item.side == 0)//红方
                    {
                        switch (map.Matrix[i, j].item.type)
                        {
                            case chesstype.che:
                                unflip_red_che -= 1;
                                break;
                            case chesstype.ma:
                                unflip_red_ma -= 1;
                                break;
                            case chesstype.jiang:
                                unflip_red_jiang -= 1;
                                break;
                            case chesstype.pao:
                                unflip_red_pao -= 1;
                                break;
                            case chesstype.zu:
                                unflip_red_zu -= 1;
                                break;
                            case chesstype.shi:
                                unflip_red_shi -= 1;
                                break;
                            case chesstype.xiang:
                                unflip_red_xiang -= 1;
                                break;
                        }
                    }
                }
            }
            map.Pbche = unflip_blue_che / all_unflip;
            map.Pbjiang = unflip_blue_jiang / all_unflip;
            map.Pbma = unflip_blue_ma / all_unflip;
            map.Pbxiang = unflip_blue_xiang / all_unflip;
            map.Pbshi = unflip_blue_shi / all_unflip;
            map.Pbzu = unflip_blue_zu / all_unflip;
            map.Pbpao = unflip_blue_pao / all_unflip;
            map.Prche = unflip_red_che / all_unflip;
            map.Prjiang = unflip_red_jiang / all_unflip;
            map.Prma = unflip_red_ma / all_unflip;
            map.Prxiang = unflip_red_xiang / all_unflip;
            map.Prshi = unflip_red_shi / all_unflip;
            map.Przu = unflip_red_zu / all_unflip;
            map.Prpao = unflip_red_pao / all_unflip;
        }


        private void flipchess(Map map, int i, int j)
        {
            chozenX = i;
            chozenY = j;
            if (map.Matrix[chozenX, chozenY].item.side == 1)
            {
                switch (map.Matrix[chozenX, chozenY].item.type)
                {
                    case chesstype.che:
                        map.Matrix[chozenX, chozenY].container.Image = global::CDC.Properties.Resources.BlueChe;
                        map.Matrix[chozenX, chozenY].flip = 1;
                        break;
                    case chesstype.ma:
                        map.Matrix[chozenX, chozenY].container.Image = global::CDC.Properties.Resources.BlueMa;
                        map.Matrix[chozenX, chozenY].flip = 1;
                        break;
                    case chesstype.jiang:
                        map.Matrix[chozenX, chozenY].container.Image = global::CDC.Properties.Resources.BlueJiang;
                        map.Matrix[chozenX, chozenY].flip = 1;
                        break;
                    case chesstype.pao:
                        map.Matrix[chozenX, chozenY].container.Image = global::CDC.Properties.Resources.BluePao;
                        map.Matrix[chozenX, chozenY].flip = 1;
                        break;
                    case chesstype.zu:
                        map.Matrix[chozenX, chozenY].container.Image = global::CDC.Properties.Resources.BlueZu;
                        map.Matrix[chozenX, chozenY].flip = 1;
                        break;
                    case chesstype.shi:
                        map.Matrix[chozenX, chozenY].container.Image = global::CDC.Properties.Resources.BlueShi;
                        map.Matrix[chozenX, chozenY].flip = 1;
                        break;
                    case chesstype.xiang:
                        map.Matrix[chozenX, chozenY].container.Image = global::CDC.Properties.Resources.BlueXiang;
                        map.Matrix[chozenX, chozenY].flip = 1;
                        break;
                }
            }

            if (map.Matrix[chozenX, chozenY].item.side == 0)
            {
                switch (map.Matrix[chozenX, chozenY].item.type)
                {
                    case chesstype.che:
                        map.Matrix[chozenX, chozenY].container.Image = global::CDC.Properties.Resources.RedChe;
                        map.Matrix[chozenX, chozenY].flip = 1;
                        break;
                    case chesstype.ma:
                        map.Matrix[chozenX, chozenY].container.Image = global::CDC.Properties.Resources.RedMa;
                        map.Matrix[chozenX, chozenY].flip = 1;
                        break;
                    case chesstype.jiang:
                        map.Matrix[chozenX, chozenY].container.Image = global::CDC.Properties.Resources.RedShuai;
                        map.Matrix[chozenX, chozenY].flip = 1;
                        break;
                    case chesstype.pao:
                        map.Matrix[chozenX, chozenY].container.Image = global::CDC.Properties.Resources.RedPao;
                        map.Matrix[chozenX, chozenY].flip = 1;
                        break;
                    case chesstype.zu:
                        map.Matrix[chozenX, chozenY].container.Image = global::CDC.Properties.Resources.RedBin;
                        map.Matrix[chozenX, chozenY].flip = 1;
                        break;
                    case chesstype.xiang:
                        map.Matrix[chozenX, chozenY].container.Image = global::CDC.Properties.Resources.RedXiang;
                        map.Matrix[chozenX, chozenY].flip = 1;
                        break;
                    case chesstype.shi:
                        map.Matrix[chozenX, chozenY].container.Image = global::CDC.Properties.Resources.RedShi;
                        map.Matrix[chozenX, chozenY].flip = 1;
                        break;
                }
            }
            if (map.turncount == 1)
            {
                map.firstplayer = map.Matrix[chozenX, chozenY].item.side;//第一个棋子即为第一玩家的颜色
                map.secondplayer = (map.Matrix[chozenX, chozenY].item.side + 1) % 2;//第二玩家为另一颜色
                display(map.firstplayer);
            }
            map.unflipped -= 1;
            map.turncount += 1;
            map.currentside = (map.currentside + 1) % 2;//已落子换方
        }

        private void atkandmove(Map map, nodes node)
        {
            #region Branching Factor Part
            List<Move> flipList = new List<Move>();
            List<Move> movelist = new List<Move>();
            simap map2 = new CDC.simap();
            map2 = map2.createDeepClone(vmap);
            int branchingF = 0;
            int flipList_num = 0;
            int e_flip = AI_tools.getallfliplist(map2).Count;

            if ((map.currentside == 0 && map.firstplayer == 0)||(map.currentside == 1 && map.firstplayer == 1))//Red
            {
                e_flip = (e_flip - map.unflipp_blue);
            }
            else if ((map.currentside == 0 && map.firstplayer == 1) || (map.currentside == 1 && map.firstplayer == 0))//Blue
            {
                e_flip = (e_flip - map.unflipp_red);
            }
            flipList_num = e_flip;//从缩减后改为毫无缩减的状况

            movelist = AI_tools.getallmove(map2);
            flipList = AI_tools.getallfliplist(map2);
            branchingF = movelist.Count + flipList.Count;
            allB.Add(branchingF);
            #endregion

            Move use = new CDC.Move();
            use.froX = node.movtion.froX;
            use.froY = node.movtion.froY;
            use.desX = node.movtion.desX;
            use.desY = node.movtion.desY; //避免将原数据消除

            if (use.desX == -1 && use.desY == -1 && use.froX != -1 && use.froY != -1)//AI给出的行动为翻棋
            {
                flipchess(map, use.froX, use.froY);
            }
            if (use.froX != -1 && use.froY != -1 && use.desX != -1 && use.desY != -1)//都选定的情况
            {
                if (GameController.checkmove(map, use) == true)
                {
                    setmove(map, use);
                }
                else
                {
                    use.froX = -1;
                    use.froY = -1;
                    use.desX = -1;
                    use.desY = -1;
                }
            }
            if (map.currentside == 0)
            {
                label1.Text = "Current Player: First Player";
            }
            if (map.currentside == 1)
            {
                label1.Text = "Current Player: Second Player";
            }

            if ((vmap.currentside + 1) % 2 == 0)//已经实行的那一步的实施方（因为再flpchess和setmove中改变了currentside的值）属于先手或者后手
            {
                vmap.Plyr1move.Add(node);//movtion存入先手List
                vmap.Plyr_move.Add(node);
            }
            else if (((vmap.currentside + 1) % 2 == 1))
            {
                vmap.Plyr2move.Add(node);//movtion存入后手List
                vmap.Plyr_move.Add(node);
            }

            double displayturn = map.turncount - 1;
            use.froX = -1;
            use.froY = -1;
            use.desX = -1;
            use.desY = -1;
        }

        private void button1_Click(object sender, EventArgs e)//AI battle
        {
            int game_limit = 25;//进行试验的游戏局数
            int game_noli_limit = game_limit;
            int games = 1;//初始化为1
            double firstAIwin = 0;//先手AI胜利数
            double secondAIwin = 0;//后手AI胜利数
            double draw = 0;
            double draw_3 = 0;
            double draw_4 = 0;
            double totalturn = 0;//本次模拟总长度
            double totalB = 0;

            AI ai = new AI();
            ai2 ai2 = new ai2();

            while (games <= game_limit)//没有执行完规定次数时
            {
                List<double> scoreit = new List<double>();
                while (vmap.flag == 0)//循环直到游戏结束
                {
                    nodes movnod = new CDC.nodes();
                    if (vmap.currentside == 0)//先手
                    {                      
                        movnod = ai.alpha_Beta(vmap);
                        atkandmove(vmap, movnod);//调用先手AI
                    }
                    else if (vmap.currentside == 1)//后手
                    {                        
                        movnod = ai2.alpha_Beta(vmap);
                        atkandmove(vmap, movnod);//调用后手AI
                    }
                    textBox1.AppendText("Experiment Game: " + games + "\r\n");
                    Console.WriteLine("+++++++++++++++++++++++++++++++++++++" + score(vmap));
                    double temp = score(vmap);
                    scoreit.Add(temp);
                    vmap.flag = checkboard(movnod,vmap);
                }
                Console.WriteLine("One Game Finished");

                if ((vmap.firstplayer == 0 && vmap.flag == 2) || (vmap.firstplayer == 1 && vmap.flag == 1))//先手AI胜
                {
                    Console.WriteLine("First AI win!");
                    gameresult.Add(scoreit);
                    firstAIwin++;
                    Allturn.Add(vmap.turncount);//记录每局长度
                }
                else if ((vmap.firstplayer == 0 && vmap.flag == 1) || (vmap.firstplayer == 1 && vmap.flag == 2))//后手AI胜
                {
                    Console.WriteLine("Second AI win!");
                    gameresult.Add(scoreit);
                    secondAIwin++;
                    Allturn.Add(vmap.turncount);//记录每局长度
                }
                else if (vmap.flag == 3)
                {
                    draw += 1;
                    draw_3 += 1;
                    Allturn.Add(vmap.turncount);//记录每局长度
                    gameresult.Add(scoreit);
                }
                else if (vmap.flag == 4)
                {
                    draw_4 += 1;
                    AI_tools.resetground(vmap);
                    allB.Clear();
                    continue;
                }
                Console.WriteLine("Number of Games：" + games);//游戏结束后
                games++;
                cal_averageB(allB);
                allB.Clear();
                AI_tools.resetground(vmap);
            }

            for (int i = 0; i < game_noli_limit; i++)
            {
                totalturn += Allturn[i] - 1;
                totalB += calB[i];
            }
            textBox1.AppendText("Simulation Ended" + "\r\n");
            textBox1.AppendText("FirstAI Winning Ratio：" + (firstAIwin / game_limit) * 100 + "%" + "\r\n");
            textBox1.AppendText("SecondAI Winning Ratio：" + (secondAIwin / game_limit) * 100 + "%" + "\r\n");
            textBox1.AppendText("Draw Ratio：" + (draw / game_limit) * 100 + "%" + "\r\n");
            textBox1.AppendText("Number of Flag 3 draw：" + draw_3 + "\r\n");
            textBox1.AppendText("Number of Flag 4 draw：" + draw_4 + "\r\n");
            textBox1.AppendText("Avarage Game Length：" + totalturn / game_noli_limit + " turns" + "\r\n");
            textBox1.AppendText("Avarage Branching Factor：" + totalB / game_noli_limit + "\r\n");
            for(int i = 0; i < win_jiang.Count; i++)
            {
                Console.WriteLine(win_jiang[i]);
            }
            Console.WriteLine("=====================================================================================================" + "\r\n");
            for (int i = 0; i < win_zu.Count; i++)
            {
                Console.WriteLine(win_zu[i]);
            }
            Console.WriteLine("=====================================================================================================" + "\r\n");
            for (int i = 0; i < win_pao.Count; i++)
            {
                Console.WriteLine(win_pao[i]);
            }
            Console.WriteLine("=====================================================================================================" + "\r\n");
            for (int i = 0; i < lose_jiang.Count; i++)
            {
                Console.WriteLine(lose_jiang[i]);
            }
            Console.WriteLine("=====================================================================================================" + "\r\n");
            for (int i = 0; i < lose_zu.Count; i++)
            {
                Console.WriteLine(lose_zu[i]);
            }
            Console.WriteLine("=====================================================================================================" + "\r\n");
            for (int i = 0; i < lose_pao.Count; i++)
            {
                Console.WriteLine(lose_pao[i]);
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            AI ai = new AI();
            nodes movnod = new CDC.nodes();
            movnod = ai.alpha_Beta(vmap);
            atkandmove(vmap, movnod);//调用先手AI
            score(vmap);
            if (checkboard(movnod, vmap) != 0)
            {
                AI_tools.resetground(vmap);
                MessageBox.Show("Game End!");
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            nodes movnod = new CDC.nodes();
            AI ai = new AI();
            movnod = ai.alpha_Beta(vmap);
            if (onedice == 0)//先手为评估函数AI
            {
                click(sender, e);
                onedice = (onedice + 1) % 2;//交换骰子
            }
            else
            {
                atkandmove(vmap, movnod);
                onedice = (onedice + 1) % 2;//交换骰子
            }
        }

        private void display(int firstplayer)
        {
            if (firstplayer == 0)//显示出定好的颜色
            {
                label2.Text = "First Player: Red";
                label3.Text = "econd Player: Blue";
            }
            else if (firstplayer == 1)
            {
                label2.Text = "First Player: Blue";
                label3.Text = "Second Player: Red";
            }
        }

        private void cal_averageB(List<double> allB)
        {
            double allvalue = 0;
            for (int i = 0; i < allB.Count; i++)
            {
                allvalue += allB[i];
            }
            calB.Add(allvalue / (vmap.turncount - 1));
        }

        private double score(Map map)
        {
            double point = 0;
            double score = 0;

            for (int i = 0; i < 4; i++)
            {
                for (int j = 0; j < 8; j++)
                {
                    if (map.Matrix[i, j].item.side == 0)//红方估值
                    {
                        switch (map.Matrix[i, j].item.type)
                        {
                            case chesstype.jiang:
                                point = 20;
                                break;
                            case chesstype.shi:
                                point = 10;
                                break;
                            case chesstype.xiang:
                                point = 6;
                                break;
                            case chesstype.che:
                                point = 4;
                                break;
                            case chesstype.ma:
                                point = 3;
                                break;
                            case chesstype.zu:
                                point = 2;
                                break;
                            case chesstype.pao:
                                point = 6;
                                break;
                            case chesstype.blank://移动
                                point = 0;
                                break;
                        }
                    }
                    if (map.Matrix[i, j].item.side == 1)//蓝方估值
                    {
                        switch (map.Matrix[i, j].item.type)
                        {
                            case chesstype.jiang:
                                point = -20;
                                break;
                            case chesstype.shi:
                                point = -10;
                                break;
                            case chesstype.xiang:
                                point = -6;
                                break;
                            case chesstype.che:
                                point = -4;
                                break;
                            case chesstype.ma:
                                point = -3;
                                break;
                            case chesstype.zu:
                                point = -2;
                                break;
                            case chesstype.pao:
                                point = -6;
                                break;
                            case chesstype.blank://移动
                                point = 0;
                                break;
                        }
                    }
                    if (map.Matrix[i, j].item.side == 2)
                    {
                        point = 0;
                    }
                    for (int b = 0; b < 4; b++)
                    {
                        for (int a = 0; a < 8; a++)
                        {
                            if (((Math.Abs(b - i) <= 1) && (Math.Abs(a - j) <= 1)) && map.Matrix[b, a].flip == 1 && map.Matrix[b, a].item.side != 2)
                            {
                                Move movtion = new Move();
                                Move movtion2 = new Move();

                                movtion.froX = i;
                                movtion.froY = j;
                                movtion.desX = b;
                                movtion.desY = a;

                                if (GameController.checkmove(map, movtion) == true)
                                {
                                    point = point * 1.25;
                                }

                                movtion2.froX = b;
                                movtion2.froY = a;
                                movtion2.desX = i;
                                movtion2.desY = j;

                                if (GameController.checkmove(map, movtion2) == true)
                                {
                                    point = point * 0.75;
                                }
                            }
                        }
                    }
                    score += point;
                }
            }
            textBox1.AppendText("Board Scoring: " + score + "\r\n");
            return score;
        }

        private void button4_Click(object sender, EventArgs e)
        {
            AI_tools.resetground(vmap);
        }

        private void button5_Click(object sender, EventArgs e)
        {
            AI_tools.resetground(vmap);
        }
    }

    public static class ExtensionMethod
    {
        private static Random rng = new Random();

        public static void Shuffle<T>(this IList<T> list)
        {
            int n = list.Count;
            while (n > 1)
            {
                n--;
                int k = rng.Next(n + 1);
                T value = list[k];
                list[k] = list[n];
                list[n] = value;
            }
        }
    }
}

