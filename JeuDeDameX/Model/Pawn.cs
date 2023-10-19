using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace JeuDeDameX.Model
{
    class Pawn : TokenModel
    {
        public Pawn(int x, int y, SolidColorBrush color, int team) : base(x, y, color, team)
        {
        }

        public override bool IsMoveLegal(int x, int y, bool eat)
        {
            //A black pawn can only go down and right/left so only legal moves are x/y -> -1/1 and 1/1
            if(Team == 2)
            {
                if (eat == false)
                {
                    return Y - y == 1 && Math.Abs(X - x) == 1 ? true : false;
                }
                else
                {
                    return Math.Abs(Y - y) == 1 && Math.Abs(X - x) == 1 ? true : false;
                }
            }
            //A white pawn can only go up and right/left so only legal moves are x/y -> -1/-1 and 1/-1
            else
            {
                if (eat == false)
                {
                    return Y - y == -1 && Math.Abs(X - x) == 1 ? true : false;
                }
                else
                {
                    return Math.Abs(Y - y) == 1 && Math.Abs(X - x) == 1 ? true : false;
                }
            }
        }
    }
}
