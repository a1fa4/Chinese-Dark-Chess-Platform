using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CDC
{
    class nodes
    {
        public double ucb;//双精度浮点型数据 表达节点的UCB值
        public int sim_Num;//整形数据 表达节点进行的模拟次数
        public double win_Num;//结点胜利数
        public double alpha;
        public double beta;
        public double score;
        public int depth;
        public simap board;//结点的地图
        public Move movtion;//结点的指令
        public List<nodes> child;//子节点的数据类型
        public double win_ratio;//结点的胜率
        public int turn;//记录节点的回合数
        public int visit;//0为未被访问，1为访问过一次，-1为所有子节点访问完
        public nodes parent;
        public int remain_flip_number;
        public int resign;

        public nodes()//创造节点的方法
        {
            ucb = 10;//初始化UCB值为10
            sim_Num = 0;//初始化节点模拟数为零
            win_Num = 0;//初始化结点胜利数为零
            child = new List<nodes>();//创造出子节点序列
            movtion = new CDC.Move();
            win_ratio = 0;//胜率
            turn = 0;
            visit = 0;
            alpha = -1000;//下界值最大化
            beta = 1000;//上界值最大化
            remain_flip_number = 0;
            resign = -1;
        }

        public nodes(nodes parent)//创造节点的方法
        {
            this.parent = parent;
            ucb = 10;//初始化UCB值为10
            sim_Num = 0;//初始化节点模拟数为零
            win_Num = 0;//初始化结点胜利数为零
            child = new List<nodes>();//创造出子节点序列
            Move movtion = new CDC.Move();
            win_ratio = 0;//胜率
            turn = 0;
            visit = 0;
            alpha = -1000;
            beta = 1000;
            remain_flip_number = 0;
            resign = -1;
        }
    }
}
