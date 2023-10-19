using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using JeuDeDameX.Model;
using System.Windows.Input;
using GalaSoft.MvvmLight.Command;

namespace JeuDeDameX.ViewModel {
    class EndMenuViewModel : INotifyPropertyChanged {

        // End sentence showing who is the winner
        private string winner;
        // Property used for the binding of the winner sentence
        public string Winner { get { return winner; } set { winner = value; OnPropertyChanged(); } }

        // Event used to trigger the replay function
        public delegate void Replay();
        public static event Replay OnReplay;

        public EndMenuViewModel() {
            // Subscribe to the event
            BoardViewModel.OnFinish += GetWinnerName;
        }

        /// <summary>
        /// Function used to form the winner sentence
        /// </summary>
        private void GetWinnerName() {
            this.Winner = Game.GetInstance().CurrentPlayer.Name + " a gagné !";
        }

        // ICommand pattern used for the quit button
        private ICommand _QuitCommand;
        public ICommand QuitCommand {
            get {
                if (_QuitCommand == null) {
                    _QuitCommand = new RelayCommand(() =>
                    {
                        System.Windows.Application.Current.Shutdown();
                    });
                }
                return _QuitCommand;
            }
        }

        // ICommand pattern used for the replay button
        private ICommand _ReplayCommand;
        public ICommand ReplayCommand
        {
            get
            {
                if (_ReplayCommand == null)
                {
                    _ReplayCommand = new RelayCommand(() =>
                    {
                        OnReplay?.Invoke();
                    });
                }
                return _ReplayCommand;
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string name = null) {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }
}
