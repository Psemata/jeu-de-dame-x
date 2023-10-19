using JeuDeDameX.Model;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace JeuDeDameX.ViewModel
{
    public class TokenViewModel : INotifyPropertyChanged
    {
        public TokenModel Token { get; set; }
        private bool isQueen;
        public bool IsQueen { get { return isQueen; } set { isQueen = value; OnPropertyChanged(); } }
        public int X { get { return Token.X; } set { Token.X = value; OnPropertyChanged(); } }
        public int Y { get { return Token.Y; } set { Token.Y = value; OnPropertyChanged(); } }
        public SolidColorBrush Color { get { return Token.Color; } set { Token.Color = value; OnPropertyChanged(); } }
        public TokenViewModel(TokenModel token)
        {
            IsQueen = false;
            Token = token;
        }

        public void Move(int x, int y)
        {
            X = x;
            Y = y;
        }

        public void ChangeToQueen()
        {
            TokenModel t = new Queen(X, Y, Color, Token.Team);
            Token = t;
            IsQueen = true;
        }

        public void ChangeToPawn()
        {
            TokenModel t = new Pawn(X, Y, Color, Token.Team);
            Token = t;
            IsQueen = false;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged([CallerMemberName] string name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }
}
