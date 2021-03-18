using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CDC
{
    class Move
    {
        public int froX;//原坐标
        public int froY;//原坐标
        public int desX;//目的坐标
        public int desY;//目的坐标
        public double risk;

        public Move(int froX, int froY, int desX, int desY)//带参构造器
        {
            this.froX = froX;
            this.froY = froY;
            this.desX = desX;
            this.desY = desY;
            this.risk = 0;
        }

        public Move()//无参构造器
        {
            froX = -1;
            froY = -1;
            desX = -1;
            desY = -1;
            risk = 0;
        }

        public Move deepclone()
        {
            Move movtion = new CDC.Move();
            movtion.froX = this.froX;
            movtion.froY = this.froY;
            movtion.desX = this.desX;
            movtion.desY = this.desY;
            movtion.risk = this.risk;
            return movtion;
        }
    }
}
    
    

