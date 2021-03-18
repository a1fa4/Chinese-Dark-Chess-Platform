using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;


namespace CDC
{
    class Map
    {
        public block[,] Matrix;
        public int unflipped = 32;//用于显示还有多少棋子未被翻开
        public int redCount = 16;//用于显示红方剩下的棋子数
        public int blueCount = 16;//用于显示蓝方剩下的棋子数
        public int flag = 0;//用于显示本局比赛是否结束
        public int turncount = 1;
        public int currentside = 0;//先手为0后手为1
        public int firstplayer = 2;//0为红方，1为蓝方，2为空
        public int secondplayer = 2;//0为红方，1为蓝方，2为空
        public List<nodes> Plyr_move = new List<nodes>();//用于记录两个玩家的步数数据
        public List<nodes> Plyr1move = new List<nodes>();//用于记录玩家一的步数数据
        public List<nodes> Plyr2move = new List<nodes>();//用于记录玩家二的步数数据

        //Detail
        //初始化各项数值，皆初始化为零
        //以下为红方
        public int redjiang = 0;
        public int redshi = 0;
        public int redxiang = 0;
        public int redche = 0;
        public int redma = 0;
        public int redpao = 0;
        public int redzu = 0;
        public int redremain = 0;
        public int unflipp_red = 0;
        public double Prjiang = 0;
        public double Prshi = 0;
        public double Prxiang = 0;
        public double Prma = 0;
        public double Prche = 0;
        public double Przu = 0;
        public double Prpao = 0;
        //以下为蓝方
        public int bluejiang = 0;
        public int blueshi = 0;
        public int bluexiang = 0;
        public int blueche = 0;
        public int bluema = 0;
        public int bluepao = 0;
        public int bluezu = 0;
        public int topred = 0;//用于代表红方位阶最高棋子
        public int topblue = 0;//用于代表蓝方位阶最高棋子
        public int blueremain = 0;
        public int unflipp_blue = 0;
        public double Pbjiang = 0;
        public double Pbshi = 0;
        public double Pbxiang = 0;
        public double Pbma = 0;
        public double Pbche = 0;
        public double Pbzu = 0;
        public double Pbpao = 0;

        public struct block
        {
            public PictureBox container;
            public chess item;
            public int flip;//0 表示未翻，翻后值变为1
        };

        public Map()
        {
            Matrix = new block[4, 8];
        }

        public Map createDeepClone()
        {
            Map copied = new Map();
            copied.Matrix = new block[4, 8];
            for (int x = 0; x < 4; x++)
            {
                for (int y = 0; y < 8; y++)
                {
                    copied.Matrix[x, y].container = new PictureBox();
                    copied.Matrix[x, y].item = new CDC.chess();
                    copied.Matrix[x, y].item.type = new chesstype();
                    copied.Matrix[x, y] = this.Matrix[x, y];
                    copied.Matrix[x, y].container = this.Matrix[x, y].container;
                    copied.Matrix[x, y].item = this.Matrix[x, y].item;
                    copied.Matrix[x, y].item.type = this.Matrix[x, y].item.type;
                    copied.Matrix[x, y].flip = this.Matrix[x, y].flip;
                }
            }
            copied.turncount = this.turncount;
            copied.currentside = this.currentside;
            copied.firstplayer = this.firstplayer;
            copied.secondplayer = this.secondplayer;
            copied.blueche = this.blueche;
            copied.bluezu = this.bluezu;
            copied.bluejiang = this.bluejiang;
            copied.blueshi = this.blueshi;
            copied.bluepao = this.bluepao;
            copied.bluexiang = this.bluexiang;
            copied.bluema = this.bluema;
            copied.blueCount = this.blueCount;
            copied.blueremain = this.blueremain;
            copied.unflipp_blue = this.unflipp_blue;
            copied.redche = this.redche;
            copied.redjiang = this.redjiang;
            copied.redma = this.redma;
            copied.redzu = this.redzu;
            copied.redxiang = this.redxiang;
            copied.redpao = this.redpao;
            copied.redshi = this.redshi;
            copied.redCount = this.redCount;
            copied.redremain = this.redremain;
            copied.unflipp_red = this.unflipp_red;
            return copied;
        }

        public void executemove(Map map, Move movtion)
        {
            if (movtion.desX == -1 && movtion.desY == -1 && movtion.froX != -1 && movtion.froY != -1)//AI给出的行动为翻棋
            {
                map.flipchess(map, movtion.froX, movtion.froY);
            }
            if (movtion.froX != -1 && movtion.froY != -1 && movtion.desX != -1 && movtion.desY != -1)//都选定的情况
            {
                if (GameController.checkmove(map, movtion) == true)
                {
                    map.setmove(map, movtion);
                }
                else
                {
                    movtion.froX = -1;
                    movtion.froY = -1;
                    movtion.desX = -1;
                    movtion.desY = -1;
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

        private void flipchess(Map map, int i, int j)
        {
            int chozenX = i;
            int chozenY = j;
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
            if (map.turncount == 0)
            {
                map.firstplayer = map.Matrix[chozenX, chozenY].item.side;//第一个棋子即为第一玩家的颜色
                map.secondplayer = (map.Matrix[chozenX, chozenY].item.side + 1) % 2;//第二玩家为另一颜色
            }
            map.turncount += 1;
        }
       
        #region 拷贝主体
        /// <summary>
        /// 深度拷贝
        /// </summary>
        /// <returns></returns>
        public Map DeepClone()
        {
            using (Stream objectStream = new MemoryStream())
            {
                IFormatter formatter = new BinaryFormatter();
                formatter.Serialize(objectStream, this);
                objectStream.Seek(0, SeekOrigin.Begin);
                return formatter.Deserialize(objectStream) as Map;
            }
        }

        public object Clone()
        {
            return this.MemberwiseClone();
        }
        #endregion

    }
}


