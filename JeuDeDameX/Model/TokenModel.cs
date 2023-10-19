using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace JeuDeDameX.Model
{
    public class TokenModel
    {
        public int X { get; set; }
        public int Y { get; set; }
        public SolidColorBrush Color { get; set; }
        public int Team { get; set; }

        public TokenModel(int x, int y, SolidColorBrush color, int team)
        {
            X = x;
            Y = y;
            Color = color;
            Team = team;
        }

        public virtual bool IsMoveLegal(int x, int y, bool eat) { return true; }
    }
}
