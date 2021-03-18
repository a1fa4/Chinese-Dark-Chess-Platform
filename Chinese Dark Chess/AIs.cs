using System;
using System.Collections.Generic;
using System.Linq;
using System.Diagnostics;

namespace CDC
{
    class AIs
    {
        //Main parameters
        private static int max_sim = 30000; //最大模拟数量
        private static int search_depth = 0;//搜索深度
        private static int max_child = 0;//
        private const double z = 2;//UCB公式参数

        //Time control parameters
        private static long Lim_time = 9900;//Limited time for every turn
        private static long Lef_time = 0;
        Stopwatch timer = new Stopwatch();//Creating timer

        //random number creator
        Random rand = new Random();

        public Move Misirlou_v3(block[][] matrix, int currentside, int firstplayer, int turn)//Monte-Carlo
        {
            timer.Start();
            int now_sim = 0; //现在的模拟数
            nodes rootnode = new nodes();//创造根节点
            rootnode.board = matrix;//复制地图进入根节点
            rootnode.side = currentside;//根节点的玩家参数
            List<Move> moveList = AI_tools.getallmove(rootnode.board, currentside);//根节点时的可行动列表
            List<Move> flipList = AI_tools.getallfliplist(rootnode.board);//根节点时的翻棋列表

            for (int a = 1; a <= max_sim; a++)
            {
                UCT(rootnode, currentside, firstplayer);
                now_sim++;
            }

            Move bestmove = best_move(rootnode);
            timer.Stop();

            Lef_time -= timer.ElapsedMilliseconds;
            timer.Reset();
            return bestmove;
        }

        #region Monte-Carlo
        //得到最大UCB值节点，并通过常数控制保证没被模拟过的节点仍会被选择到
        private Move best_move(nodes rootnode)
        {
            double maxwin_ratio = -1;
            nodes bestnode = new nodes();
            rand = new Random();

            for (int i = 0; i < rootnode.child.Count; i++)
            {
                if (rootnode.child[i].win_ratio > maxwin_ratio)
                {
                    maxwin_ratio = rootnode.child[i].win_ratio;
                    max_child = i;
                    bestnode = rootnode.child[i];
                }
            }
            return bestnode.movtion;
        }

        //一次完整的UCT过程
        public void UCT(nodes rootnode, int currentside, int firstplayer)
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
            block[][] matrix = node.board;//末端节点的地图信息

            //叶子结点为最终节点
            if (checkboard(node.board, currentside, firstplayer) != -1)//即游戏已结束
            {
                if (checkboard(node.board, node.side, firstplayer) == 1)
                {
                    node.sim_Num++;
                    node.win_Num++;
                }
                else
                {
                    node.sim_Num++;//模拟数+1
                }
            }
            else//游戏没结束则扩展节点
            {
                expand(node);//扩展末端节点,被扩展的子节点内只有Move

                int tempNodeNum = rand.Next(node.child.Count);//随机选择一个子节点
                int nowside;
                block[][] board2 = node.child[tempNodeNum].board;//创造末端子节点的地图

                //循环进行模拟直到终局
                for (int k = board2.getTurnCount(); k < board2.getTurnLimit() && checkboard(board2, 0) == -1; k++)
                {
                    doOneTurnMove(board2, nowside++ % 2);
                }

                //更新末端节点胜利数与模拟数
                if (checkboard(board2, node.side, firstplayer) == 1)//终局模拟返回为胜
                {
                    node.win_Num++;//末端子节点胜利数+1
                    node.sim_Num++;//末端子节点模拟数+1

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
            block[][] board2 = node.board;
            List<Move> atkandmoveList;//移动指令序列
            List<Move> flippingList;//攻击指令序列
            int nexTeamCol;

            //列举可能行动之前检查行动队伍是否发生改变
                board2.incTurnCount();//回合数+1
                nexTeamCol = (node.side + 1) % 2;//蓝队变红队，红队变蓝队，可行动方交换
                flippingList = AI_tools.get((node.side + 1) % 2, board2);//移动指令变为对方可移动指令
                atkandmoveList = AI_tools.getAllAttackMoves((node.side + 1) % 2, board2);//攻击指令变为对方可攻击指令

            for (int i = 0; i < atkandmoveList.Count; i++)//对攻击指令进行遍历
            {
                node.child.Add(new nodes());//添加子节点
                node.child[i].movtion = atkandmoveList[i];//添加攻击指令进指令集
                node.child[i].board = board2;
                node.child[i].board.atkandmove(atkandmoveList[i]);
                node.child[i].side = nexTeamCol;//进入下一手、
            }
            for (int i = 0; i < flippingList.Count; i++)//对翻棋指令进行遍历
            {
                node.child.Add(new nodes());//添加子节点
                node.child[i + atkandmoveList.Count].movtion = flippingList[i];//添加移动指令进指令集
                node.child[i + atkandmoveList.Count].board = board2;//复制地图信息
                node.child[i + atkandmoveList.Count].board.atkandmove(flippingList[i]);
                node.child[i + atkandmoveList.Count].side = nexTeamCol;//进入下一手
            }
        }

        private double checkboard(block[][] board, int currentside, int firstplayer)
        {
            int redCount = 0;
            int blueCount = 0;
            for (int i = 0; i < 4; i++)
            {
                for (int j = 0; j < 8; j++)
                {
                    if (board[i][j].item.side == 0)//为红方
                    {
                        redCount++;//红加1
                    }
                    if (board[i][j].item.side == 1)//为蓝方
                    {
                        blueCount++;//蓝加1
                    }
                }
            }
            if (redCount == 0)//Blue win!
            {
                if ((currentside == 0 && firstplayer == 0) && (currentside == 1 && firstplayer == 1))//当前方为先手，且先手为红色
                {
                    return 0;// lose
                }
                else
                {
                    return 1;// win
                }
            }
            if (blueCount == 0)//Red win!
            {
                if ((currentside == 0 && firstplayer == 0) && (currentside == 1 && firstplayer == 1))//当前方为先手，且先手为红色
                {
                    return 1;// lose
                }
                else
                {
                    return 0;// win
                }
            }
            return -1;
        }
        #endregion
    }
}

