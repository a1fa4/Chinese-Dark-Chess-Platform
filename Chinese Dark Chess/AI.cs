using System;
using System.Collections.Generic;
using System.Linq;
using System.Diagnostics;

namespace CDC
{
    class AI
    {
        #region Alpha-Beta Parameters
        private static int limitdepth = 2;//6 is a human thinking level
        private int currentside = 2;
        int loop_number = 0;
        int cut = 0;
        #endregion

        //random number creator
        Random rand = new Random();

        public nodes alpha_Beta(Map map)
        {

            //start rootnode;
            nodes rootnode = new nodes();
            simap board = new simap();
            nodes bestnode = new nodes();
            List<Move> atkmovtion = new List<Move>();
            List<Move> allmovtion = new List<Move>();
            double maxscore = -1000;
            board = board.createDeepClone(map);
            rootnode.board = board;
            rootnode.depth = 0;//根节点的深度为零
            currentside = rootnode.board.currentside;

            atkmovtion = AI_tools.getallatk(board);
            allmovtion = AI_tools.getallmove(board);

            alphabeta(rootnode);
            Console.WriteLine("CUT:" + cut + "\r\n\r\n\r\n");

            foreach (nodes child in rootnode.child)
            {
                if (child.visit == 0)
                {
                    continue;
                }
                else if (maxscore <= child.beta)
                {
                    maxscore = child.beta;
                    bestnode = child;
                }
            }

            if (deloop(bestnode, map) == true)//6步里2步以上重复　且为bestnode  则跳出
            {
                rootnode.child.Remove(bestnode);

                if (map.unflipped > 0)
                {
                    bestnode = smartflipping(rootnode, map);
                }
                else if (map.unflipped == 0 && rootnode.child.Count != 0)
                {
                    foreach (nodes achild in rootnode.child)
                    {
                        if (maxscore <= achild.beta)
                        {
                            maxscore = achild.beta;
                            bestnode = achild;
                        }
                    }
                }
                else if (map.unflipped == 0 && rootnode.child.Count == 0)
                {
                    bestnode = subAI_Evaluate(map);
                }
            }

            if(bestnode.movtion.desX == -1 && bestnode.movtion.desY == -1)
            {
                bestnode = smartflipping(rootnode, map);
            }

            return bestnode;
        }

        private bool deloop(nodes node, Map map)
        {
            bool value = false;
            if (map.currentside == 0)//为先手方时调用先手方历史纪录
            {
                int n = 0;
                if (map.Plyr1move.Count <= 6)
                {
                    for (int i = 0; i < map.Plyr1move.Count - 1; i++)//重复度检查
                    {
                        if (map.Plyr1move[i].movtion.froX == node.movtion.froX && map.Plyr1move[i].movtion.froY == node.movtion.froY
                        && map.Plyr1move[i].movtion.desX == node.movtion.desX && map.Plyr1move[i].movtion.desY == node.movtion.desY)
                        {
                            n += 1;
                        }
                    }
                }
                else
                {
                    for (int i = map.Plyr1move.Count - 6; i < map.Plyr1move.Count - 1; i++)//重复度检查
                    {
                        if (map.Plyr1move[i].movtion.froX == node.movtion.froX && map.Plyr1move[i].movtion.froY == node.movtion.froY
                        && map.Plyr1move[i].movtion.desX == node.movtion.desX && map.Plyr1move[i].movtion.desY == node.movtion.desY)
                        {
                            n += 1;
                        }
                    }
                }

                if (n >= 3)//多次重复
                {
                    value = true;
                    Console.WriteLine("Found Repeat!");
                }
            }
            else if (map.currentside == 1)//为后手方时调用后手方历史纪录
            {
                int m = 0;
                if (map.Plyr2move.Count <= 6)
                {
                    for (int i = 0; i < map.Plyr2move.Count - 1; i++)//重复度检查
                    {
                        if (map.Plyr2move[i].movtion.froX == node.movtion.froX && map.Plyr2move[i].movtion.froY == node.movtion.froY
                        && map.Plyr2move[i].movtion.desX == node.movtion.desX && map.Plyr2move[i].movtion.desY == node.movtion.desY)
                        {
                            m += 1;
                        }
                    }
                }
                else
                {
                    for (int i = map.Plyr2move.Count - 6; i < map.Plyr2move.Count - 1; i++)//重复度检查
                    {
                        if (map.Plyr2move[i].movtion.froX == node.movtion.froX && map.Plyr2move[i].movtion.froY == node.movtion.froY
                        && map.Plyr2move[i].movtion.desX == node.movtion.desX && map.Plyr2move[i].movtion.desY == node.movtion.desY)
                        {
                            m += 1;
                        }
                    }
                }

                if (m >= 3)//多次重复
                {
                    value = true;
                    Console.WriteLine("Found Repeat!");
                }
            }
            return value;
        }

        private bool check_suicide(Map vmap, Move bestmove)
        {
            bool re_bool = false;
            simap map = new CDC.simap();
            Move move = new Move();
            move = bestmove.deepclone();
            map = map.createDeepClone(vmap);
            map.executemove(map, move);
            if (AI_tools.getcheckone(map, bestmove) == true)
            {
                re_bool = true;
            }
            return re_bool;
        }

        private double alphabeta(nodes node)
        {
            loop_number++;
            double rvalue = 0;
            if (node.depth < limitdepth && node.visit == 0)
            {
                atk_expand(node);
                if (node.depth != 0)
                {
                    if (node.depth % 2 == 0)//极大结点继承上界
                    {
                        node.beta = node.parent.beta;
                    }
                    else//极小结点继承下界
                    {
                        node.alpha = node.parent.alpha;
                    }
                }
            }
            if (node.depth == limitdepth || (node.visit == 1 && node.child.Count == 0)||(node.visit == -3))//最终结点
            {
                rvalue = score(node);
                if (node.depth % 2 == 0)//MAX
                {
                    node.alpha = rvalue;
                }
                else//MIN
                {
                    node.beta = rvalue;
                }

                if (node.depth != 0)
                {
                    nodes temp = node;
                    temp.child = null;
                    temp = null;
                }
                return rvalue;
            }

            if (node.depth % 2 == 0)// 极大节点
            {
                foreach (nodes child in node.child)
                {
                    double temp = alphabeta(child);
                    if (temp >= node.alpha)
                    {
                        node.alpha = temp;
                    }

                    if (node.beta <= node.alpha) // 该极大节点的值>=α>=β，该极大节点后面的搜索到的值肯定会大于β，因此不会被其上层的极小节点所选用了。对于根节点，β为正无穷
                    {
                        cut++;
                        break;
                    }
                }
                //子节点循环完毕
                rvalue = node.alpha;

                if (node.depth != 0)
                {
                    nodes temp = node;
                    temp.child = null;
                    temp = null;
                }
            }
            else// 极小节点
            {
                foreach (nodes child in node.child) // 极大节点
                {
                    double temp = alphabeta(child);
                    if (temp <= node.beta)
                    {
                        node.beta = temp;
                    }

                    if (node.beta <= node.alpha) // 该极大节点的值<=β<=α，该极小节点后面的搜索到的值肯定会小于α，因此不会被其上层的极大节点所选用了。对于根节点，α为负无穷
                    {
                        cut++;
                        break;
                    }
                }
                rvalue = node.beta;

                if (node.depth != 0)
                {
                    nodes temp = node;
                    temp.child = null;
                    temp = null;
                }
            }
            return rvalue;
        }


        private nodes smartflipping(nodes rootnode, Map map)
        {
            int flippedpiece = 0;
            double temp_value = 0;
            double min_risk = 100;
            int mark = 0;
            nodes nodesmove = new nodes();
            List<Move> flippingList = new List<Move>();
            flippingList = AI_tools.getallfliplist(rootnode.board);
            Map mapasses = new Map();
            mapasses = map.createDeepClone();

            for (int i = 0; i < 4; i++)
            {
                for (int j = 0; j < 8; j++)
                {
                    if (rootnode.board.Matrix[i, j].flip == 1)
                    {
                        flippedpiece += 1;
                    }
                }
            }
            //第一阶段，只要相邻格有敌方棋子则不翻
            for (int i = 0; i < flippingList.Count; i++)
            {
                for (int a = 0; a < 4; a++)
                {
                    for (int b = 0; b < 8; b++)
                    {
                        if ((((Math.Abs(a - flippingList[i].froX) == 0) && (Math.Abs(b - flippingList[i].froY) == 1)) || ((Math.Abs(a - flippingList[i].froX) == 1) && (Math.Abs(b - flippingList[i].froY) == 0))) &&//与翻棋坐标相邻的
                            ((rootnode.board.Matrix[a, b].flip == 1 && rootnode.board.Matrix[a, b].item.side == rootnode.board.firstplayer && rootnode.board.currentside == 1) ||
                            (rootnode.board.Matrix[a, b].flip == 1 && rootnode.board.Matrix[a, b].item.side == rootnode.board.secondplayer && rootnode.board.currentside == 0)))//周围的非己方棋子
                        {
                            mark++;
                        }
                    }
                }
                if (mark != 0)
                {
                    flippingList.Remove(flippingList[i]);//则去掉这样一个元素
                    mark = 0;//清除
                    continue;
                }

            }
            if (flippingList.Count != 0)//存在保守翻棋策略
            {
                nodesmove.remain_flip_number = flippingList.Count;
                nodesmove.movtion.froX = flippingList[rand.Next(flippingList.Count)].froX;
                nodesmove.movtion.froY = flippingList[rand.Next(flippingList.Count)].froY;
            }
            //第二阶段，不存在完全保守的策略时
            else if (flippingList.Count == 0)//不存在保守翻棋策略
            {
                flippingList = AI_tools.getallfliplist(rootnode.board);//重新刷进flippinglist
                mark = 0;//清除mark
                for (int i = 0; i < flippingList.Count; i++)
                {
                    for (int a = 0; a < 4; a++)
                    {
                        for (int b = 0; b < 8; b++)
                        {
                            if ((((Math.Abs(a - flippingList[i].froX) == 0) && (Math.Abs(b - flippingList[i].froY) == 1)) || ((Math.Abs(a - flippingList[i].froX) == 1) && (Math.Abs(b - flippingList[i].froY) == 0))) &&//与翻棋坐标相邻的
                                ((rootnode.board.Matrix[a, b].flip == 1 && rootnode.board.Matrix[a, b].item.side == rootnode.board.firstplayer && rootnode.board.currentside == 1) ||
                                (rootnode.board.Matrix[a, b].flip == 1 && rootnode.board.Matrix[a, b].item.side == rootnode.board.secondplayer && rootnode.board.currentside == 0)))//周围的已经翻开的非己方棋子
                            {
                                temp_value = cal_risk(a, b, map);
                                if (temp_value == -1000)//绝对会被吃掉的情况
                                {
                                    mark++;
                                }
                                else
                                {
                                    flippingList[i].risk += cal_risk(a, b, map);
                                }
                            }
                        }
                    }
                    if (mark != 0)
                    {
                        flippingList.Remove(flippingList[i]);//则去掉这样一个元素
                        mark = 0;//清除
                        continue;
                    }
                }
                for (int i = 0; i < flippingList.Count; i++)//选择威胁值最小的翻棋步骤
                {
                    if (flippingList[i].risk <= min_risk)//越高的正值
                    {
                        nodesmove.remain_flip_number = flippingList.Count;
                        nodesmove.movtion.froX = flippingList[i].froX;
                        nodesmove.movtion.froY = flippingList[i].froY;
                    }
                }
            }
            return nodesmove;
        }

        private double cal_risk(int a, int b, Map map)
        {
            double risk = 0;
            if ((map.currentside == 0) && (map.firstplayer == 0) || (map.currentside == 1) && (map.firstplayer == 1))//现在先手且先手为红或者现在后手且先手为蓝
            {
                switch (map.Matrix[a, b].item.type)
                {
                    case chesstype.jiang://周围蓝将
                        risk = (-1) * (map.Przu) + (1 - (map.Przu));
                        break;
                    case chesstype.shi:
                        risk = (-1) * (map.Prjiang) + (1 - (map.Prjiang));
                        break;
                    case chesstype.xiang:
                        risk = (-1) * (map.Prjiang + map.Prshi) + (1 - (map.Prjiang + map.Prshi));
                        break;
                    case chesstype.che:
                        risk = (-1) * (map.Prjiang + map.Prshi + map.Prxiang) + (1 - (map.Prjiang + map.Prshi + map.Prxiang));
                        break;
                    case chesstype.ma:
                        risk = (-1) * (map.Prjiang + map.Prshi + map.Prxiang + map.Prche) + (1 - (map.Prjiang + map.Prshi + map.Prxiang + map.Prche));
                        break;
                    case chesstype.zu:
                        risk = (-1) * (map.Prjiang + map.Prshi + map.Prxiang + map.Prche + map.Prma) + (1 - (map.Prjiang + map.Prshi + map.Prxiang + map.Prche + map.Prma));
                        break;
                    case chesstype.pao:
                        risk = (-1) * (map.Prjiang + map.Prshi + map.Prxiang + map.Prche + map.Prma) + (1 - (map.Prjiang + map.Prshi + map.Prxiang + map.Prche + map.Prma));
                        break;
                }
            }
            else if ((map.currentside == 1) && (map.firstplayer == 0) || (map.currentside == 0) && (map.firstplayer == 1))//现在后手且先手为红或者现在先手且先手为蓝
            {
                switch (map.Matrix[a, b].item.type)
                {
                    case chesstype.jiang://周围红将
                        risk = (-1) * (map.Pbzu) + (1 - (map.Pbzu));
                        break;
                    case chesstype.shi:
                        risk = (-1) * (map.Pbjiang) + (1 - (map.Pbjiang));
                        break;
                    case chesstype.xiang:
                        risk = (-1) * (map.Pbjiang + map.Pbshi) + (1 - (map.Pbjiang + map.Pbshi));
                        break;
                    case chesstype.che:
                        risk = (-1) * (map.Pbjiang + map.Pbshi + map.Pbxiang) + (1 - (map.Pbjiang + map.Pbshi + map.Pbxiang));
                        break;
                    case chesstype.ma:
                        risk = (-1) * (map.Pbjiang + map.Pbshi + map.Pbxiang + map.Pbche) + (1 - (map.Pbjiang + map.Pbshi + map.Pbxiang + map.Pbche));
                        break;
                    case chesstype.zu:
                        risk = (-1) * (map.Pbjiang + map.Pbshi + map.Pbxiang + map.Pbche + map.Pbma) + (1 - (map.Pbjiang + map.Pbshi + map.Pbxiang + map.Pbche + map.Pbma));
                        break;
                    case chesstype.pao:
                        risk = (-1) * (map.Pbjiang + map.Pbshi + map.Pbxiang + map.Pbche + map.Pbma) + (1 - (map.Pbjiang + map.Pbshi + map.Pbxiang + map.Pbche + map.Pbma));
                        break;
                }
            }
            if (risk == -1)
            {
                risk = -1000;//default value
            }
            return risk;
        }

        private void atk_expand(nodes node)
        {
            simap board2 = new CDC.simap();
            board2 = node.board.createDeepClone();
            List<Move> atkandmoveList  = new List<Move>();//移动指令序列
            List<Move> flippingList = new List<Move>();
            node.visit = 1;//节点已经被访问过

            atkandmoveList = AI_tools.getallmove(node.board);//移动指令变为对方可移动指令
            flippingList = AI_tools.getallfliplist(node.board);

            for (int i = 0; i < atkandmoveList.Count; i++)//对攻击指令进行遍历
            {
                node.child.Add(new nodes());//添加子节点
                node.child[i].movtion = atkandmoveList[i].deepclone();//添加攻击移动指令进指令集
                node.child[i].board = board2.createDeepClone();//复制地图信息进入子节点
                node.child[i].board.executemove(node.child[i].board, atkandmoveList[i]);
                node.child[i].depth = (node.depth) + 1;
                node.child[i].parent = node;
            }
            for(int i = 0; i < flippingList.Count; i++)
            {
                node.child.Add(new nodes());//添加子节点
                node.child[i + atkandmoveList.Count].movtion = flippingList[i].deepclone();//添加翻棋移动指令进指令集
                node.child[i + atkandmoveList.Count].board = board2.createDeepClone();//复制地图信息进入子节点
                node.child[i + atkandmoveList.Count].depth = (node.depth) + 1;
                node.child[i + atkandmoveList.Count].parent = node;
                node.child[i + atkandmoveList.Count].visit = -3;//翻棋节点不会有子节点，表示为翻棋节点
            }
        }

        private double score(nodes node)
        {
            List<Move> flippingList = new List<Move>();
            flippingList = AI_tools.getallfliplist(node.board);
            double point = 0;
            double score = 0;
            for (int i = 0; i < 4; i++)
            {
                for (int j = 0; j < 8; j++)
                {
                    if ((node.board.Matrix[i, j].item.side == node.board.firstplayer && currentside == 0 && node.board.Matrix[i, j].flip == 1) ||
                        (node.board.Matrix[i, j].item.side == node.board.secondplayer && currentside == 1 && node.board.Matrix[i, j].flip == 1))//针对根结点行动方的估值
                                                                                                                                                //确保在相同手时使用最大值，对方手时使用最小值
                    {
                        switch (node.board.Matrix[i, j].item.type)
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
                    if ((node.board.Matrix[i, j].item.side == node.board.firstplayer && currentside == 1 && node.board.Matrix[i, j].flip == 1) ||
                        (node.board.Matrix[i, j].item.side == node.board.secondplayer && currentside == 0 && node.board.Matrix[i, j].flip == 1))
                    {
                        switch (node.board.Matrix[i, j].item.type)
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
                    if (node.board.Matrix[i, j].item.side == 2)
                    {
                        point = 0;
                    }

                    for (int b = 0; b < 4; b++)
                    {
                        for (int a = 0; a < 8; a++)
                        {
                            if (((Math.Abs(b - i) <= 1) && (Math.Abs(a - j) <= 1)) && node.board.Matrix[b, a].flip == 1 && node.board.Matrix[b, a].item.side != 2)
                            {
                                Move movtion = new Move();
                                Move movtion2 = new Move();

                                movtion.froX = i;
                                movtion.froY = j;
                                movtion.desX = b;
                                movtion.desY = a;

                                if (GameController.checkmove(node.board, movtion) == true)
                                {
                                    point = point * 1.25;
                                }

                                movtion2.froX = b;
                                movtion2.froY = a;
                                movtion2.desX = i;
                                movtion2.desY = j;

                                if (GameController.checkmove(node.board, movtion2) == true)
                                {
                                    point = point * 0.75;
                                }
                            }
                        }
                    }
                    score += point;
                }
            }

            if (node.movtion.desX == -1 && node.movtion.desY == -1 && node.movtion.froX != -1 && node.movtion.froY != -1)//节点为翻棋面节点时
            {
                point = cal_flipping(node);
                for (int b = 0; b < 4; b++)
                {
                    for (int a = 0; a < 8; a++)
                    {
                        if (((Math.Abs(b - node.movtion.froX) <= 1) && (Math.Abs(a - node.movtion.froY) <= 1)) && node.board.Matrix[b, a].flip == 1 && node.board.Matrix[b, a].item.side != 2)
                        {
                            Move movtion = new Move();
                            Move movtion2 = new Move();

                            movtion.froX = node.movtion.froX;
                            movtion.froY = node.movtion.froY;
                            movtion.desX = b;
                            movtion.desY = a;

                            if (GameController.checkmove(node.board, movtion) == true)
                            {
                                point = point * 1.25;
                            }

                            movtion2.froX = b;
                            movtion2.froY = a;
                            movtion2.desX = node.movtion.froX;
                            movtion2.desY = node.movtion.froY;

                            if (GameController.checkmove(node.board, movtion2) == true)
                            {
                                point = point * 0.75;
                            }
                        }
                    }
                }
                score += point;
            }
            return score;
        }

        private double cal_flipping(nodes node)
        {
            double point = 0;
            if ((node.board.currentside == 0) && (node.board.firstplayer == 0) || (node.board.currentside == 1) && (node.board.firstplayer == 1))//现在先手且先手为红或者现在后手且先手为蓝
            {
                point = (1) * (node.board.Prjiang * 20 + node.board.Prshi * node.board.Prxiang * 6 + node.board.Prche * 4 + node.board.Prma * 3 + node.board.Przu * 2 + node.board.Prpao * 6) + (-1)
                 * (node.board.Pbjiang * 20 + node.board.Pbshi * node.board.Pbxiang * 6 + node.board.Pbche * 4 + node.board.Pbma * 3 + node.board.Pbzu * 2 + node.board.Pbpao * 6);
            }
            else if ((node.board.currentside == 1) && (node.board.firstplayer == 0) || (node.board.currentside == 0) && (node.board.firstplayer == 1))//现在后手且先手为红或者现在先手且先手为蓝
            {
                point = (-1) * (node.board.Prjiang * 20 + node.board.Prshi * node.board.Prxiang * 6 + node.board.Prche * 4 + node.board.Prma * 3 + node.board.Przu * 2 + node.board.Prpao * 6) + (1)
                 * (node.board.Pbjiang * 20 + node.board.Pbshi * node.board.Pbxiang * 6 + node.board.Pbche * 4 + node.board.Pbma * 3 + node.board.Pbzu * 2 + node.board.Pbpao * 6);
            }
            return point;
        }

        #region sub-AI evaluation
        public nodes subAI_Evaluate(Map map)
        {
            simap map2 = new CDC.simap();
            map2 = map2.createDeepClone(map);//复制一下地图
            List<Move> movtionlist = new List<Move>();//移动指令表
            List<Move> fliplist = new List<Move>();//翻棋指令表
            List<Move> Alllist = new List<Move>();//翻棋指令表
            Move bestmove = new CDC.Move();//最佳动作
            Move bestflip = new Move();//最佳翻棋
            Move startflip = new Move();//起始翻棋
            nodes bestnode = new nodes();

            if (map2.turncount == 1)//一局开始
            {
                fliplist = AI_tools.getallfliplist(map2);//可翻行动列表
                startflip = fliplist[rand.Next(fliplist.Count)];//起始翻
                map2.firstplayer = map.Matrix[startflip.froX, startflip.froY].item.side;//第一个棋子即为第一玩家的颜色
                map2.secondplayer = (map.Matrix[startflip.froX, startflip.froY].item.side + 1) % 2;//第二玩家为另一颜色
                map2.currentside = map2.firstplayer;
                bestnode.movtion.froX = startflip.froX;
                bestnode.movtion.froY = startflip.froY;
                bestnode.movtion.desX = startflip.desX;
                bestnode.movtion.desY = startflip.desY;
                return bestnode;
            }
            else//非开局
            {
                int point;
                int Maxpoint = 0;
                movtionlist = AI_tools.getallmove(map2);//获取所有可行指令
                foreach (Move movtion in movtionlist)//遍历进攻选项
                {
                    point = AI_tools.point_Evaluate(map2, movtion);//计算进攻选项得点
                    if (point > Maxpoint)//判断得点最大
                    {
                        Maxpoint = point;
                        bestmove = movtion;
                        bestnode.movtion.froX = bestmove.froX;
                        bestnode.movtion.froY = bestmove.froY;
                        bestnode.movtion.desX = bestmove.desX;
                        bestnode.movtion.desY = bestmove.desY;
                        return bestnode;
                    }
                }
                if (Maxpoint == 0)//为移动指令时
                {
                    if (movtionlist.Count != 0)
                    {
                        bestmove = getnear(map2, map2.currentside);
                    }
                    Alllist = AI_tools.getmovflip(map2);
                    if (Alllist.Count == 0)
                    {
                        checkboard(map2);
                    }
                    bestmove = Alllist[rand.Next(Alllist.Count)];//无有效步的时候生成随机步数
                }
            }
            bestnode.movtion.froX = bestmove.froX;
            bestnode.movtion.froY = bestmove.froY;
            bestnode.movtion.desX = bestmove.desX;
            bestnode.movtion.desY = bestmove.desY;
            return bestnode;
        }

        private Move getnear(simap map, int currentside)
        {
            double distance = 0;
            double mindistance = 100;
            List<Move> movtionlist = new List<Move>();
            Move nearmove = new Move();
            movtionlist = AI_tools.getallmove(map);
            foreach (Move movtion in movtionlist)
            {
                distance = (Math.Sqrt((Math.Pow(Math.Abs(movtion.desY - movtion.froY), 2) + Math.Pow(Math.Abs(movtion.desX - movtion.froX), 2))));
                if (distance < mindistance)
                {
                    nearmove = movtion;
                }
            }
            if (mindistance == 100)
            {
                nearmove = movtionlist[rand.Next(movtionlist.Count)];
            }
            return nearmove;
        }
        #endregion

        #region check board
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
        #endregion

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
            List<Move> atklist = AI_tools.getallmove(board);
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