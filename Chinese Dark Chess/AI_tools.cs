using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CDC
{
    class AI_tools
    {
        Random rand = new Random();
        public static List<Move> getallmove(simap map)
        {

            List<Move> movtionlist = new List<Move>();
            for (int i = 0; i < 4; i++)
            {
                for (int j = 0; j < 8; j++)//遍历棋盘
                {
                    if ((map.Matrix[i, j].flip == 1 && map.Matrix[i, j].item.side == map.firstplayer && map.currentside == 0) ||
                        (map.Matrix[i, j].flip == 1 && map.Matrix[i, j].item.side == map.secondplayer && map.currentside == 1))//遍历已翻开的棋子里面的己方棋子
                    {
                        for (int a = 0; a < 4; a++)
                        {
                            for (int b = 0; b < 8; b++)
                            {
                                if ((map.Matrix[a, b].flip == 1 && map.Matrix[a, b].item.side == map.firstplayer && map.currentside == 1) ||
                                    (map.Matrix[a, b].flip == 1 && map.Matrix[a, b].item.side == map.secondplayer && map.currentside == 0) ||
                                     (map.Matrix[a, b].flip == 1 && map.Matrix[a, b].item.side == 2))//遍历已翻开的棋子里面的非己方棋子
                                {
                                    Move movtion = new CDC.Move();
                                    movtion.froX = i;
                                    movtion.froY = j;
                                    movtion.desX = a;
                                    movtion.desY = b;

                                    if (GameController.checkmove(map, movtion) == true)//合法步的话加入List
                                    {
                                        movtionlist.Add(movtion);
                                    }
                                }
                            }
                        }
                    }
                }
            }
            return movtionlist;
        }

        public static bool getcheckone(simap map, Move move)
        {
            List<Move> movtionlist = new List<Move>();
            bool value = false;
            for (int i = 0; i < 4; i++)
            {
                for (int j = 0; j < 8; j++)//遍历棋盘
                {
                    if ((map.Matrix[i, j].flip == 1 && map.Matrix[i, j].item.side == map.firstplayer && map.currentside == 0) ||
                        (map.Matrix[i, j].flip == 1 && map.Matrix[i, j].item.side == map.secondplayer && map.currentside == 1))//遍历已翻开的棋子里面的己方棋子
                    {
                        Move movtion = new CDC.Move();
                        movtion.froX = i;
                        movtion.froY = j;
                        movtion.desX = move.desX;
                        movtion.desY = move.desY;

                        if (GameController.checkmove(map, movtion) == true)//合法步的话加入List
                        {
                            movtionlist.Add(movtion);
                        }
                    }
                }
            }
            if(movtionlist.Count != 0)
            {
                value = true;
            }
            return value;
        }

        public static bool checktac(simap map)//是否可以攻击
        {
            bool isit = false;
            List<Move> movtionlist = new List<Move>();

            for (int i = 0; i < 4; i++)
            {
                for (int j = 0; j < 8; j++)//遍历棋盘
                {
                    if ((map.Matrix[i, j].flip == 1 && map.Matrix[i, j].item.side == map.firstplayer && map.currentside == 0) ||
                        (map.Matrix[i, j].flip == 1 && map.Matrix[i, j].item.side == map.secondplayer && map.currentside == 1))//遍历已翻开的棋子里面的己方棋子
                    {
                        for (int a = 0; a < 4; a++)
                        {
                            for (int b = 0; b < 8; b++)
                            {
                                if ((map.Matrix[a, b].flip == 1 && map.Matrix[a, b].item.side == map.firstplayer && map.currentside == 1) ||
                                    (map.Matrix[a, b].flip == 1 && map.Matrix[a, b].item.side == map.secondplayer && map.currentside == 0))//遍历已翻开的棋子里面的非己方棋子
                                {
                                    Move movtion = new CDC.Move();
                                    movtion.froX = i;
                                    movtion.froY = j;
                                    movtion.desX = a;
                                    movtion.desY = b;

                                    if (GameController.checkmove(map, movtion) == true)//合法步的话加入List
                                    {
                                        movtionlist.Add(movtion);
                                    }
                                }
                            }
                        }
                    }
                }
            }

            if (movtionlist.Count != 0)
            {
                isit = true;
            }
            else if (movtionlist.Count == 0)
            {
                isit = false;
            }
            return isit;
        }

        public static bool checktac_op(simap map)//是否可以攻击
        {
            bool isit = false;
            List<Move> movtionlist = new List<Move>();
            int currentside = (map.currentside + 1) % 2;

            for (int i = 0; i < 4; i++)
            {
                for (int j = 0; j < 8; j++)//遍历棋盘
                {
                    if ((map.Matrix[i, j].flip == 1 && map.Matrix[i, j].item.side == map.firstplayer && currentside == 0) ||
                        (map.Matrix[i, j].flip == 1 && map.Matrix[i, j].item.side == map.secondplayer && currentside == 1))//遍历已翻开的棋子里面的己方棋子
                    {
                        for (int a = 0; a < 4; a++)
                        {
                            for (int b = 0; b < 8; b++)
                            {
                                if ((map.Matrix[a, b].flip == 1 && map.Matrix[a, b].item.side == map.firstplayer && currentside == 1) ||
                                    (map.Matrix[a, b].flip == 1 && map.Matrix[a, b].item.side == map.secondplayer && currentside == 0))//遍历已翻开的棋子里面的非己方棋子
                                {
                                    Move movtion = new CDC.Move();
                                    movtion.froX = i;
                                    movtion.froY = j;
                                    movtion.desX = a;
                                    movtion.desY = b;

                                    if (GameController.checkmove(map, movtion) == true)//合法步的话加入List
                                    {
                                        movtionlist.Add(movtion);
                                    }
                                }
                            }
                        }
                    }
                }
            }

            if (movtionlist.Count != 0)
            {
                isit = true;
            }
            else if (movtionlist.Count == 0)
            {
                isit = false;
            }
            return isit;
        }

        public static List<Move> getallatk(simap map)
        {
            List<Move> atklist = new List<Move>();
            for (int i = 0; i < 4; i++)
            {
                for (int j = 0; j < 8; j++)//遍历棋盘
                {
                    if (map.Matrix[i, j].flip != 0 && map.Matrix[i, j].item.side == map.currentside)//遍历已翻开的棋子里面的己方棋子
                    {
                        for (int a = 0; a < 4; a++)
                        {
                            for (int b = 0; b < 8; b++)
                            {
                                if (map.Matrix[a, b].flip != 0 && map.Matrix[a, b].item.side != map.currentside)//遍历已翻开的棋子里面的非己方棋子
                                {
                                    Move movtion = new CDC.Move();
                                    movtion.froX = i;
                                    movtion.froY = j;
                                    movtion.desX = a;
                                    movtion.desY = b;
                                    if (map.Matrix[a, b].item.side == (map.currentside + 1) % 2)//目的地为对方棋子
                                    {
                                        if (GameController.checkmove(map, movtion) == true)//合法步的话加入List
                                        {
                                            atklist.Add(movtion);
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
            return atklist;
        }

        public static List<Move> getonlymove(simap map)
        {
            List<Move> movelist = new List<Move>();
            for (int i = 0; i < 4; i++)
            {
                for (int j = 0; j < 8; j++)//遍历棋盘
                {
                    if (map.Matrix[i, j].flip != 0 && map.Matrix[i, j].item.side == map.currentside)//遍历已翻开的棋子里面的己方棋子
                    {
                        for (int a = 0; a < 4; a++)
                        {
                            for (int b = 0; b < 8; b++)
                            {
                                if (map.Matrix[a, b].flip != 0 && map.Matrix[a, b].item.side != map.currentside)//遍历已翻开的棋子里面的非己方棋子
                                {
                                    Move movtion = new CDC.Move();
                                    movtion.froX = i;
                                    movtion.froY = j;
                                    movtion.desX = a;
                                    movtion.desY = b;
                                    if (map.Matrix[a, b].item.side == 2)//目的地空
                                    {
                                        if (GameController.checkmove(map, movtion) == true)//合法步的话加入List
                                        {
                                            movelist.Add(movtion);
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
            return movelist;
        }

        public static List<Move> getmovflip(simap map)
        {
            List<Move> fliplist = new List<Move>();
            List<Move> movtionlist = new List<Move>();
            fliplist = getallfliplist(map);
            movtionlist = getallmove(map);
            movtionlist.AddRange(fliplist);
            return movtionlist;
        }

        public static List<Move> getallfliplist(simap map)
        {
            List<Move> fliplist = new List<Move>();
            for (int i = 0; i < 4; i++)
            {
                for (int j = 0; j < 8; j++)
                {
                    if (map.Matrix[i, j].flip == 0)//未翻开的状态的话
                    {
                        Move flip = new Move();
                        flip.froX = i;
                        flip.froY = j;
                        fliplist.Add(flip);
                    }
                }
            }
            return fliplist;
        }

        public static int point_Evaluate(simap map, Move move)
        {
            int point = -1;
            chesstype target_type = map.Matrix[move.desX, move.desY].item.type;
            switch (target_type)
            {
                case chesstype.jiang:
                    point = 10;
                    break;
                case chesstype.shi:
                    point = 7;
                    break;
                case chesstype.xiang:
                    point = 5;
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
            return point;
        }

        public static void resetground(Map map)//将棋盘随机化成初始状态
        {
            //设置矩阵内BLOCK
            map.Matrix[0, 0].item.side = 1;
            map.Matrix[0, 0].item.type = chesstype.che;
            map.Matrix[0, 1].item.side = 1;
            map.Matrix[0, 1].item.type = chesstype.che;
            map.Matrix[0, 2].item.side = 1;
            map.Matrix[0, 2].item.type = chesstype.ma;
            map.Matrix[0, 3].item.side = 1;
            map.Matrix[0, 3].item.type = chesstype.ma;
            map.Matrix[0, 4].item.side = 1;
            map.Matrix[0, 4].item.type = chesstype.xiang;
            map.Matrix[0, 5].item.side = 1;
            map.Matrix[0, 5].item.type = chesstype.xiang;
            map.Matrix[0, 6].item.side = 1;
            map.Matrix[0, 6].item.type = chesstype.shi;
            map.Matrix[0, 7].item.side = 1;
            map.Matrix[0, 7].item.type = chesstype.shi;
            map.Matrix[1, 0].item.side = 1;
            map.Matrix[1, 0].item.type = chesstype.pao;
            map.Matrix[1, 1].item.side = 1;
            map.Matrix[1, 1].item.type = chesstype.pao;
            map.Matrix[1, 2].item.side = 1;
            map.Matrix[1, 2].item.type = chesstype.jiang;
            map.Matrix[1, 3].item.side = 1;
            map.Matrix[1, 3].item.type = chesstype.zu;
            map.Matrix[1, 4].item.side = 1;
            map.Matrix[1, 4].item.type = chesstype.zu;
            map.Matrix[1, 5].item.side = 1;
            map.Matrix[1, 5].item.type = chesstype.zu;
            map.Matrix[1, 6].item.side = 1;
            map.Matrix[1, 6].item.type = chesstype.zu;
            map.Matrix[1, 7].item.side = 1;
            map.Matrix[1, 7].item.type = chesstype.zu;
            map.Matrix[2, 0].item.side = 0;
            map.Matrix[2, 0].item.type = chesstype.che;
            map.Matrix[2, 1].item.side = 0;
            map.Matrix[2, 1].item.type = chesstype.che;
            map.Matrix[2, 2].item.side = 0;
            map.Matrix[2, 2].item.type = chesstype.ma;
            map.Matrix[2, 3].item.side = 0;
            map.Matrix[2, 3].item.type = chesstype.ma;
            map.Matrix[2, 4].item.side = 0;
            map.Matrix[2, 4].item.type = chesstype.pao;
            map.Matrix[2, 5].item.side = 0;
            map.Matrix[2, 5].item.type = chesstype.pao;
            map.Matrix[2, 6].item.side = 0;
            map.Matrix[2, 6].item.type = chesstype.xiang;
            map.Matrix[2, 7].item.side = 0;
            map.Matrix[2, 7].item.type = chesstype.xiang;
            map.Matrix[3, 0].item.side = 0;
            map.Matrix[3, 0].item.type = chesstype.shi;
            map.Matrix[3, 1].item.side = 0;
            map.Matrix[3, 1].item.type = chesstype.shi;
            map.Matrix[3, 2].item.side = 0;
            map.Matrix[3, 2].item.type = chesstype.jiang;
            map.Matrix[3, 3].item.side = 0;
            map.Matrix[3, 3].item.type = chesstype.zu;
            map.Matrix[3, 4].item.side = 0;
            map.Matrix[3, 4].item.type = chesstype.zu;
            map.Matrix[3, 5].item.side = 0;
            map.Matrix[3, 5].item.type = chesstype.zu;
            map.Matrix[3, 6].item.side = 0;
            map.Matrix[3, 6].item.type = chesstype.zu;
            map.Matrix[3, 7].item.side = 0;
            map.Matrix[3, 7].item.type = chesstype.zu;

            //随机种子生成
            List<Map.block> aboard = new List<Map.block>();
            for (int i = 0; i < 4; i++)
            {
                for (int j = 0; j < 8; j++)
                {
                    aboard.Add(map.Matrix[i, j]);
                }
            }
            aboard.Shuffle();

            //取出List第一个文件，并从List中移除
            for (int i = 0; i < 4; i++)
            {
                for (int j = 0; j < 8; j++)
                {
                    map.Matrix[i, j].container.Image = global::CDC.Properties.Resources.back;
                    map.Matrix[i, j].item.type = aboard[0].item.type;
                    map.Matrix[i, j].item.side = aboard[0].item.side;
                    map.Matrix[i, j].flip = 0;//0 表示未翻，翻后值变为1
                    aboard.RemoveAt(0);
                }
            }
            map.flag = 0;
            map.turncount = 1;
            map.unflipped = 32;
            map.currentside = 0;//firstplayer为0，secondplayer为1
            map.firstplayer = 2;//0为红方，1为蓝方，2为空
            map.secondplayer = 2;
            map.Plyr1move.Clear();
            map.Plyr2move.Clear();
        }

        public static void resetgroundCI(Map map)//将棋盘随机化成初始状态
        {
            int i, j;
            for (i = 0; i < 4; i++)
            {
                for (j = 0; j < 8; j++)
                {
                    map.Matrix[i, j].container.Image = null;//重置容器至空
                    map.Matrix[i, j].item.side = 2;//重置玩家颜色
                    map.Matrix[i, j].item.type = chesstype.blank;//重置棋子类型
                    map.Matrix[i, j].flip = 1;//0 表示未翻，翻后值变为1

                }
            }

            //设置矩阵内BLOCK
            map.Matrix[0, 0].item.side = 1;
            map.Matrix[0, 0].item.type = chesstype.che;
            map.Matrix[0, 1].item.side = 1;
            map.Matrix[0, 1].item.type = chesstype.che;
            map.Matrix[0, 2].item.side = 1;
            map.Matrix[0, 2].item.type = chesstype.ma;
            map.Matrix[0, 3].item.side = 1;
            map.Matrix[0, 3].item.type = chesstype.ma;
            map.Matrix[0, 4].item.side = 1;
            map.Matrix[0, 4].item.type = chesstype.xiang;
            map.Matrix[0, 5].item.side = 1;
            map.Matrix[0, 5].item.type = chesstype.xiang;
            map.Matrix[0, 6].item.side = 1;
            map.Matrix[0, 6].item.type = chesstype.shi;
            map.Matrix[0, 7].item.side = 1;
            map.Matrix[0, 7].item.type = chesstype.shi;
            map.Matrix[1, 0].item.side = 1;
            map.Matrix[1, 0].item.type = chesstype.pao;
            map.Matrix[1, 1].item.side = 1;
            map.Matrix[1, 1].item.type = chesstype.pao;
            map.Matrix[1, 2].item.side = 1;
            map.Matrix[1, 2].item.type = chesstype.jiang;
            map.Matrix[1, 3].item.side = 1;
            map.Matrix[1, 3].item.type = chesstype.zu;
            map.Matrix[1, 4].item.side = 1;
            map.Matrix[1, 4].item.type = chesstype.zu;
            map.Matrix[1, 5].item.side = 1;
            map.Matrix[1, 5].item.type = chesstype.zu;
            map.Matrix[1, 6].item.side = 1;
            map.Matrix[1, 6].item.type = chesstype.zu;
            map.Matrix[1, 7].item.side = 1;
            map.Matrix[1, 7].item.type = chesstype.zu;
            map.Matrix[2, 0].item.side = 0;
            map.Matrix[2, 0].item.type = chesstype.che;
            map.Matrix[2, 1].item.side = 0;
            map.Matrix[2, 1].item.type = chesstype.che;
            map.Matrix[2, 2].item.side = 0;
            map.Matrix[2, 2].item.type = chesstype.ma;
            map.Matrix[2, 3].item.side = 0;
            map.Matrix[2, 3].item.type = chesstype.ma;
            map.Matrix[2, 4].item.side = 0;
            map.Matrix[2, 4].item.type = chesstype.pao;
            map.Matrix[2, 5].item.side = 0;
            map.Matrix[2, 5].item.type = chesstype.pao;
            map.Matrix[2, 6].item.side = 0;
            map.Matrix[2, 6].item.type = chesstype.xiang;
            map.Matrix[2, 7].item.side = 0;
            map.Matrix[2, 7].item.type = chesstype.xiang;
            map.Matrix[3, 0].item.side = 0;
            map.Matrix[3, 0].item.type = chesstype.shi;
            map.Matrix[3, 1].item.side = 0;
            map.Matrix[3, 1].item.type = chesstype.shi;
            map.Matrix[3, 2].item.side = 0;
            map.Matrix[3, 2].item.type = chesstype.jiang;
            map.Matrix[3, 3].item.side = 0;
            map.Matrix[3, 3].item.type = chesstype.zu;
            map.Matrix[3, 4].item.side = 0;
            map.Matrix[3, 4].item.type = chesstype.zu;
            map.Matrix[3, 5].item.side = 0;
            map.Matrix[3, 5].item.type = chesstype.zu;
            map.Matrix[3, 6].item.side = 0;
            map.Matrix[3, 6].item.type = chesstype.zu;
            map.Matrix[3, 7].item.side = 0;
            map.Matrix[3, 7].item.type = chesstype.zu;

            //随机种子生成
            List<Map.block> aboard = new List<Map.block>();
            for (i = 0; i < 4; i++)
            {
                for (j = 0; j < 8; j++)
                {
                    aboard.Add(map.Matrix[i, j]);
                }
            }
            aboard.Shuffle();

            //取出List第一个文件，并从List中移除
            for (i = 0; i < 4; i++)
            {
                for (j = 0; j < 8; j++)
                {
                    map.Matrix[i, j].item.type = aboard[0].item.type;
                    map.Matrix[i, j].item.side = aboard[0].item.side;
                    if (map.Matrix[i, j].item.side == 1)
                    {
                        switch (map.Matrix[i, j].item.type)
                        {
                            case chesstype.che:
                                map.Matrix[i, j].container.Image = global::CDC.Properties.Resources.BlueChe;
                                map.Matrix[i, j].flip = 1;
                                break;
                            case chesstype.ma:
                                map.Matrix[i, j].container.Image = global::CDC.Properties.Resources.BlueMa;
                                map.Matrix[i, j].flip = 1;
                                break;
                            case chesstype.jiang:
                                map.Matrix[i, j].container.Image = global::CDC.Properties.Resources.BlueJiang;
                                map.Matrix[i, j].flip = 1;
                                break;
                            case chesstype.pao:
                                map.Matrix[i, j].container.Image = global::CDC.Properties.Resources.BluePao;
                                map.Matrix[i, j].flip = 1;
                                break;
                            case chesstype.zu:
                                map.Matrix[i, j].container.Image = global::CDC.Properties.Resources.BlueZu;
                                map.Matrix[i, j].flip = 1;
                                break;
                            case chesstype.shi:
                                map.Matrix[i, j].container.Image = global::CDC.Properties.Resources.BlueShi;
                                map.Matrix[i, j].flip = 1;
                                break;
                            case chesstype.xiang:
                                map.Matrix[i, j].container.Image = global::CDC.Properties.Resources.BlueXiang;
                                map.Matrix[i, j].flip = 1;
                                break;
                        }
                    }

                    if (map.Matrix[i, j].item.side == 0)
                    {
                        switch (map.Matrix[i, j].item.type)
                        {
                            case chesstype.che:
                                map.Matrix[i, j].container.Image = global::CDC.Properties.Resources.RedChe;
                                map.Matrix[i, j].flip = 1;
                                break;
                            case chesstype.ma:
                                map.Matrix[i, j].container.Image = global::CDC.Properties.Resources.RedMa;
                                map.Matrix[i, j].flip = 1;
                                break;
                            case chesstype.jiang:
                                map.Matrix[i, j].container.Image = global::CDC.Properties.Resources.RedShuai;
                                map.Matrix[i, j].flip = 1;
                                break;
                            case chesstype.pao:
                                map.Matrix[i, j].container.Image = global::CDC.Properties.Resources.RedPao;
                                map.Matrix[i, j].flip = 1;
                                break;
                            case chesstype.zu:
                                map.Matrix[i, j].container.Image = global::CDC.Properties.Resources.RedBin;
                                map.Matrix[i, j].flip = 1;
                                break;
                            case chesstype.xiang:
                                map.Matrix[i, j].container.Image = global::CDC.Properties.Resources.RedXiang;
                                map.Matrix[i, j].flip = 1;
                                break;
                            case chesstype.shi:
                                map.Matrix[i, j].container.Image = global::CDC.Properties.Resources.RedShi;
                                map.Matrix[i, j].flip = 1;
                                break;
                        }
                    }
                    aboard.RemoveAt(0);
                }
            }
            map.flag = 0;
            map.turncount = 1;
            map.unflipped = 0;
            map.currentside = 0;//firstplayer为0，secondplayer为1
            map.firstplayer = 0;//0为红方，1为蓝方，明棋红方先手
            map.secondplayer = 1;
        }
    }
}
