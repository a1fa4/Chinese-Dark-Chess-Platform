using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CDC
{
    enum chesstype //枚举棋子的类型 空白 将 车 马 炮 象 卒 仕
    {
        blank,
        jiang,
        shi,
        xiang,
        che,
        ma,
        pao,
        zu,
    };

    struct chess//定义结构体Chess（包括枚举类型player-side和结构体类型chesstype-type）
    {
        public int side;//0 red, 1 blue, 2 blank
        public chesstype type;
    };
}
