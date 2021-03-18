using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CDC
{
    class GameController
    {
        public static bool checkmove(Map map, Move move)//检查步数是否合法
        {
            chesstype my_item_type = map.Matrix[move.froX,move.froY].item.type;
            chesstype target_type = map.Matrix[move.desX,move.desY].item.type;

            if (map.Matrix[move.froX,move.froY].container.Image != global::CDC.Properties.Resources.back)//选中区域不为不为背面的状态。
            {
                if ((Math.Abs(move.desY - move.froY) <= 1 && Math.Abs(move.desX - move.froX) == 0) 
                    || (Math.Abs(move.desY - move.froY) == 0 && Math.Abs(move.desX - move.froX) <= 1))//只能移动一格
                {
                    if (map.Matrix[move.desX,move.desY].item.type == chesstype.blank)//移动棋子
                    {
                        return true;
                    }
                    else if (my_item_type > target_type)//我的棋子rank更小则返回错误除非为卒和将
                    {
                        if (my_item_type == chesstype.zu && target_type == chesstype.jiang)
                        {
                            return true;
                        }
                        return false;
                    }
                    else if (my_item_type == chesstype.jiang && target_type == chesstype.zu)//将不能吃卒
                    {
                        return false;
                    }
                    else if (my_item_type == chesstype.pao && target_type == chesstype.zu)
                    {
                        return false;
                    }
                    else if (my_item_type == chesstype.pao && target_type == chesstype.pao)
                    {
                        return false;
                    }
                    return true;
                }
                else if (my_item_type == chesstype.pao)
                {
                    if (judge_pao(map, move) == true)
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        private static bool judge_pao(Map map, Move move)
        {
            int count = 0;

            if((map.Matrix[move.desX,move.desY].item.side == 2))
            {
                return false;
            }

            if (move.froX != move.desX && move.froY != move.desY)//炮的目标不在同一行或者同一列时
            {
                return false;
            }
            else//在同一行或者同一列
            {
                if (move.desX > move.froX && move.froY == move.desY)//同列不同行且在下
                {
                    for(int i = move.froX + 1; i < move.desX; i++)
                    {
                        if(map.Matrix[i,move.froY].container.Image != null)
                        {
                            count++;
                        }
                    }
                }
                if (move.desX < move.froX && move.froY == move.desY)//同列不同行且在上
                {
                    for (int i = move.desX + 1; i < move.froX; i++)
                    {
                        if (map.Matrix[i,move.froY].container.Image != null)
                        {
                            count++;
                        }
                    }
                }
                if (move.desY < move.froY && move.froX == move.desX)//同行不同列且在左
                {
                    for (int j = move.desY + 1; j < move.froY; j++)
                    {
                        if (map.Matrix[move.froX,j].container.Image != null)
                        {
                            count++;
                        }
                    }
                }
                if (move.desY > move.froY && move.froX == move.desX)//同行不同列且在右
                {
                    for (int j = move.froY + 1; j < move.desY; j++)
                    {
                        if (map.Matrix[move.froX,j].container.Image != null)
                        {
                            count++;
                        }
                    }
                }
                if(count == 1)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }

        #region simap methods

        public static bool checkmove(simap map, Move move)//检查步数是否合法
        {
            chesstype my_item_type = map.Matrix[move.froX, move.froY].item.type;
            chesstype target_type = map.Matrix[move.desX, move.desY].item.type;

            if (map.Matrix[move.froX, move.froY].flip != 0)//选中区域不为不为背面的状态。
            {
                if ((Math.Abs(move.desY - move.froY) <= 1 && Math.Abs(move.desX - move.froX) == 0)
                    || (Math.Abs(move.desY - move.froY) == 0 && Math.Abs(move.desX - move.froX) <= 1))//只能移动一格
                {
                    if (map.Matrix[move.desX, move.desY].item.type == chesstype.blank)//移动棋子
                    {
                        return true;
                    }
                    else if (my_item_type > target_type)//我的棋子rank更小则返回错误除非为卒和将
                    {
                        if (my_item_type == chesstype.zu && target_type == chesstype.jiang)
                        {
                            return true;
                        }
                        return false;
                    }
                    else if (my_item_type == chesstype.jiang && target_type == chesstype.zu)//将不能吃卒
                    {
                        return false;
                    }
                    else if (my_item_type == chesstype.pao && target_type == chesstype.zu)
                    {
                        return false;
                    }
                    else if(my_item_type == chesstype.pao && target_type == chesstype.pao)
                    {
                        return false;
                    }
                    return true;
                }
                else if (my_item_type == chesstype.pao)
                {
                    if (judge_pao(map, move) == true)
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        private static bool judge_pao(simap map, Move move)
        {
            int count = 0;

            if ((map.Matrix[move.desX, move.desY].item.side == 2))
            {
                return false;
            }
            if (move.froX != move.desX && move.froY != move.desY)//炮的目标不在同一行或者同一列时
            {
                return false;
            }
            else//在同一行或者同一列
            {
                if (move.desX > move.froX && move.froY == move.desY)//同列不同行且在下
                {
                    for (int i = move.froX + 1; i < move.desX; i++)
                    {
                        if (map.Matrix[i, move.froY].item.type != chesstype.blank)
                        {
                            count++;
                        }
                    }
                }
                if (move.desX < move.froX && move.froY == move.desY)//同列不同行且在上
                {
                    for (int i = move.desX + 1; i < move.froX; i++)
                    {
                        if (map.Matrix[i, move.froY].item.type != chesstype.blank)
                        {
                            count++;
                        }
                    }
                }
                if (move.desY < move.froY && move.froX == move.desX)//同行不同列且在左
                {
                    for (int j = move.desY + 1; j < move.froY; j++)
                    {
                        if (map.Matrix[move.froX, j].item.type != chesstype.blank)
                        {
                            count++;
                        }
                    }
                }
                if (move.desY > move.froY && move.froX == move.desX)//同行不同列且在右
                {
                    for (int j = move.froY + 1; j < move.desY; j++)
                    {
                        if (map.Matrix[move.froX, j].item.type != chesstype.blank)
                        {
                            count++;
                        }
                    }
                }
                if (count == 1)
                {
                    return true;
                }
            }
            return false;
        }
        #endregion
    }
}
