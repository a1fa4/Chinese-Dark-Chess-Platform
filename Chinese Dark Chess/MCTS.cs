using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq;
using System.Diagnostics;

namespace CDC
{
    class MCTS
    {


        #region MCTS Parameters
        //Main parameters
        private static int max_sim = 3000; //Maximum of simulation number
        private static int search_depth = 0;//depth you can search
        private static int max_child = 0;
        private const double z = 2;//control UCB formula

        Stopwatch timer = new Stopwatch();//Creating timer
        private static long Lim_time = 100;//Limited time for every turn

        //random number creator
        Random rand = new Random();
        #endregion
        
        //Main logic module
        public Move Misirlou_v3(Map vmap)
        {
            timer.Start();
            simap map = new CDC.simap();
            map = map.createDeepClone(vmap);
            int now_sim = 0; //现在的模拟数
            nodes rootnode = new nodes();//创造根节点
            rootnode.board = map;//复制地图进入根节点
            List<Move> moveList = AI_tools.getallmove(rootnode.board);//根节点时的可行动列表
            List<Move> flipList = AI_tools.getallfliplist(rootnode.board);//根节点时的翻棋列表

            for (int a = 1; a <= max_sim; a++)
            {
                if (timer.ElapsedMilliseconds <= Lim_time)
                {
                    UCT(rootnode, map.currentside, map.firstplayer);
                    now_sim++;
                }
                else
                {
                    break;
                }
            }

            Move bestmove = best_move(rootnode);
            timer.Stop();
            timer.Reset();
            return bestmove;
        }

        //得到最大UCB值节点，并通过常数控制保证没被模拟过的节点仍会被选择到
        private Move best_move(nodes rootnode)
        {
            int temp;
            double maxucb = -1;
            nodes bestnode = new nodes();
            rand = new Random();

            for (int i = 0; i < rootnode.child.Count; i++)
            {
                if (rootnode.child[i].ucb > maxucb)
                {
                    maxucb = rootnode.child[i].ucb;
                    max_child = i;
                    bestnode = rootnode.child[max_child];
                }
            }
            if (maxucb == 10)
            {
                temp = rand.Next(rootnode.child.Count);
                bestnode = rootnode.child[temp];
            }
            return bestnode.movtion;
        }

        //一次完整的UCT过程
        private void UCT(nodes rootnode, int currentside, int firstplayer)
        {
            int d = 0;//depth
            nodes[] oneway = new nodes[max_sim];

            //拓展根节点，根节点的子节点中存入Moves
            oneway[d] = rootnode;

            //节点存在子节点的情况下则进行selection
            while (oneway[d].child.Count() > 0)
            {
                oneway[d + 1] = UCBselection(oneway[d]);
                d++;
            }
            //没有子节点存在则进行模拟（模拟过程包含扩展）
            simulate(oneway[d], currentside, firstplayer);

            if (search_depth < d)//如果此次为最大深度 更新搜索深度
            {
                search_depth = d;
            }

            //backpropagation，更新oneway所有节点的模拟数，胜利数，UCB值
            for (int depth = d; depth > 0; depth--)//遍历所有父节点
            {
                oneway[depth - 1].sim_Num += oneway[depth].sim_Num;//更新模拟数
                oneway[depth].win_ratio = oneway[depth].win_Num / oneway[depth].sim_Num;
                oneway[depth].ucb = (1 - oneway[depth].win_Num / oneway[depth].sim_Num) + Math.Sqrt(z * Math.Log(oneway[depth - 1].sim_Num) / oneway[depth].sim_Num);//计算末端ucb
            }
        }

        //到达末端结点则进行一次终盘模拟
        private void simulate(nodes node, int currentside, int firstplayer)
        {
            //叶子结点为最终节点
            if (checkboard(node.board) != -1)//即游戏已结束
            {
                if (checkboard(node.board) == 1)
                {
                    node.sim_Num++;
                    node.win_Num++;
                }
                else if (checkboard(node.board) == 0)
                {
                    node.sim_Num++;//模拟数+1
                }
                else
                {
                    node.sim_Num++;
                    node.win_Num += 0.5;
                }
            }
            else//游戏没结束则扩展节点
            {
                expand(node);//扩展末端节点,被扩展的子节点内只有Move
                int temp = rand.Next(node.child.Count);//随机选择一个末端子节点
                simap board2 = node.child[temp].board.createDeepClone();//复制末端子节点的地图

                while (checkboard(board2) == -1)//循环进行模拟直到终局
                {
                    doRandomMove(board2);
                }

                //更新末端节点胜利数与模拟数
                if (checkboard(board2) == 1)//终局模拟返回为胜
                {
                    node.win_Num++;//末端子节点胜利数+1
                    node.sim_Num++;//末端子节点模拟数+1
                }
                else if (checkboard(board2) == 0.5)
                {
                    node.win_Num += 0.5;
                    node.sim_Num++;
                }
                else
                {
                    node.sim_Num++;
                }
            }
        }

        //UCB选择模块
        private nodes UCBselection(nodes node)
        {
            rand = new Random();
            double maxUcb = 0;
            int maxUcbChild = 0;
            int a = 0;
            nodes Maxnode;

            //遍历子节点
            for (a = 0; a < node.child.Count; a++)
            {
                nodes child = node.child[a];

                if (child.sim_Num == 0)
                {
                    child.ucb = 10;
                }
                if (child.ucb > maxUcb)
                {
                    maxUcb = child.ucb;
                    maxUcbChild = a;
                }
            }
            Maxnode = node.child[maxUcbChild];
            return Maxnode;
        }

        //扩展结点
        private void expand(nodes node)
        {
            simap board2 = new CDC.simap();
            board2 = node.board.createDeepClone();
            List<Move> atkandmoveList;//移动指令序列
            List<Move> flipList;//攻击指令序列

            atkandmoveList = AI_tools.getallmove(node.board);//移动指令变为对方可移动指令
            flipList = AI_tools.getallfliplist(node.board);//攻击指令变为对方可攻击指令

            for (int i = 0; i < atkandmoveList.Count; i++)//对攻击指令进行遍历
            {
                node.child.Add(new nodes());//添加子节点
                node.child[i].movtion = atkandmoveList[i];//添加攻击移动指令进指令集
                node.child[i].board = board2.createDeepClone();//复制地图信息进入子节点
                node.child[i].board.executemove(node.child[i].board, atkandmoveList[i]);
            }

            for (int i = 0; i < flipList.Count; i++)//对翻棋指令进行遍历
            {
                node.child.Add(new nodes());//添加子节点
                node.child[i + atkandmoveList.Count].movtion = flipList[i];//添加翻棋指令进指令集
                node.child[i + atkandmoveList.Count].board = board2.createDeepClone();//复制地图信息进入子节点
                node.child[i + atkandmoveList.Count].board.executemove(node.child[i + atkandmoveList.Count].board, flipList[i]);//在子节点地图上执行指令
            }
        }

        private double checkboard(simap map)
        {
            int redCount = 0;
            int blueCount = 0;
            List<Move> alllist = AI_tools.getmovflip(map);
            if ((map.turncount >= 300) || (alllist.Count == 0))//Limit number of Draw
            {
                return 0.5;
            }

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

            if (redCount == 0)//Blue win!
            {
                if ((map.firstplayer == 0 && map.currentside == 0) || (map.firstplayer == 1 && map.currentside == 1))//map.currentside is red
                {
                    return 0;
                }
                if ((map.firstplayer == 1 && map.currentside == 0) || (map.firstplayer == 1 && map.currentside == 0))//map.currentside is blue
                {
                    return 1;
                }
            }
            if (blueCount == 0)//Red win !
            {
                if ((map.firstplayer == 1 && map.currentside == 0) || (map.firstplayer == 1 && map.currentside == 0))//map.currentside is blue
                {
                    return 0;
                }
                if ((map.firstplayer == 0 && map.currentside == 0) || (map.firstplayer == 1 && map.currentside == 1))//map.currentside is red
                {
                    return 1;
                }
            }
            return -1;//游戏未结束则返回-1
        }


        #region Methods for Simulation
        private void domove(simap board, int currentside)
        {
            int side = currentside;
            while (doRandomflip(board, side))
            {
                side = (side + 1) % 2;
            }
            while (domoveandatk(board, side))
            {
                side = (side + 1) % 2;
            }
        }

        private void doRandomMove(simap board)
        {
            List<Move> alllist = AI_tools.getmovflip(board);
            int tmp = rand.Next(alllist.Count);
            board.executemove(board, alllist[tmp]);
        }

        private bool doonlyMove(simap board, int currentside)
        {
            List<Move> movelist = AI_tools.getonlymove(board);
            if (movelist.Count == 0)
            {
                return false;
            }
            int tmp = rand.Next(movelist.Count);
            board.executemove(board, movelist[tmp]);
            return true;
        }

        private bool doRandomflip(simap board, int currentside)
        {
            List<Move> fliplist = AI_tools.getallfliplist(board);
            if (fliplist.Count == 0)
            {
                return false;
            }
            int tmp = rand.Next(fliplist.Count);
            board.executemove(board, fliplist[tmp]);
            return true;
        }

        private bool doRandomatk(simap board, int currentside)
        {
            List<Move> atklist = AI_tools.getallatk(board);
            if (atklist.Count == 0)
            {
                return false;
            }
            int tmp = rand.Next(atklist.Count);
            board.executemove(board, atklist[tmp]);
            return true;
        }

        private bool domoveandatk(simap board, int currentside)
        {
            List<Move> atkandmovelist = AI_tools.getallmove(board);
            if (atkandmovelist.Count == 0)
            {
                return false;
            }
            int tmp = rand.Next(atkandmovelist.Count);
            board.executemove(board, atkandmovelist[tmp]);
            return true;
        }
        #endregion
    }

}
