using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace JeuDeDameX.Model
{
    class Queen : TokenModel
    {
        public Queen(int x, int y, SolidColorBrush color, int team) : base(x, y, color, team)
        {

        }

        public override bool IsMoveLegal(int x, int y, bool eat)
        {
            //A queen can go everywhere diagonnaly so we check if x and y are the same
            return Math.Abs(Y - y) == Math.Abs(X - x) ? true : false;
        }
    }
}
