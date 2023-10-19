using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace JeuDeDameX.Model
{
    public class Cell
    {
        public SolidColorBrush Color { get; set; }
        public int X { get; set; }
        public int Y { get; set; }

        public Cell(SolidColorBrush color, int x, int y)
        {
            Color = color;
            X = x;
            Y = y;
        }
    }
}
