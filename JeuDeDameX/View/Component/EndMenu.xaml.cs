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
    /// Interaction logic for EndMenu.xaml
    /// </summary>
    public partial class EndMenu : UserControl {
        public EndMenu() {
            InitializeComponent();
            this.DataContext = new EndMenuViewModel();
        }
    }
}
