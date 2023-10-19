using JeuDeDameX.Model;
using JeuDeDameX.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace JeuDeDameX.View.Component {
    /// <summary>
    /// Interaction logic for Board.xaml
    /// </summary>
    public partial class Board : UserControl
    {
        public BoardViewModel boardViewModel;
        public Board() {
            InitializeComponent();
            //Gets the Grid that is used as template in ItemsControl
            ItemControl.ApplyTemplate();
            Border b = VisualTreeHelper.GetChild(ItemControl, 0) as Border;
            ItemsPresenter ip = VisualTreeHelper.GetChild(b, 0) as ItemsPresenter;
            ip.ApplyTemplate();
            //Gives DataContext to its ViewModel
            boardViewModel = new BoardViewModel(VisualTreeHelper.GetChild(ip, 0) as Grid);
            DataContext = boardViewModel;
        }
    }
}
