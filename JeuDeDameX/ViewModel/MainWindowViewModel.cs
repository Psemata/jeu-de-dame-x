using GalaSoft.MvvmLight.Command;
using JeuDeDameX.Model;
using JeuDeDameX.View.Component;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace JeuDeDameX.ViewModel
{
    class MainWindowViewModel : INotifyPropertyChanged, IObserver
    {
        int defaultPiecesValue = 20;
        public int PowerP1 { get { return G.Players[0].PowerGauge; } set { G.Players[0].PowerGauge = value; OnPropertyChanged(); } }
        public int PowerP2 { get { return G.Players[1].PowerGauge; } set { G.Players[1].PowerGauge = value; OnPropertyChanged(); } }
        private int _numberPiecesP1;
        public int NumberPiecesP1 { get { return _numberPiecesP1; } set { _numberPiecesP1 = value; OnPropertyChanged(); } }
        private int _numberPiecesP2 = 1;
        public int NumberPiecesP2 { get { return _numberPiecesP2; } set { _numberPiecesP2 = value; OnPropertyChanged(); } }
        private bool end = false;
        public bool IsFinished { get { return this.end; } set { this.end = value; OnPropertyChanged(); } }
        private String _playerTurn = "Tour Joueur 1";
        public String PlayerTurn { get { return _playerTurn; } set { _playerTurn = value; OnPropertyChanged(); } }

        private bool _playerTurnBool = false;
        public bool PlayerTurnBool { get { return _playerTurnBool; } set { _playerTurnBool = value; OnPropertyChanged(); } }

        public Game G { get; set; }
        public Board board;

        private ICommand _PowerClick;
        /// <summary>
        /// Function used to get the click on the power button
        /// </summary>
        public ICommand PowerClick
        {
            get
            {
                if (_PowerClick == null)
                {
                    _PowerClick = new RelayCommand(() =>
                    {
                        UsePower(G.CurrentPlayer.Team);
                    });
                }
                return _PowerClick;
            }
        }

        /// <summary>
        /// Function that checks what power is used by which player and sets the conditions to it
        /// </summary>
        /// <param name="turn"></param>
        public void UsePower(int turn)
        {
            board.boardViewModel.Unselect();
            if (turn == 1)
            {
                if (PowerP1 == 100)
                {
                    PowerP1 = 0;
                    board.boardViewModel.CanDestroyPawn = true;
                }
                else if (PowerP1 >= 50)
                {
                    PowerP1 -= 50;
                    board.boardViewModel.CanPawnBecomeQueen = true;
                }
                else if (PowerP1 >= 15)
                {
                    PowerP1 -= 15;
                    board.boardViewModel.CanPlayTwice = true;
                }
            }
            else
            {
                if (PowerP2 == 100)
                {
                    PowerP2 = 0;
                    board.boardViewModel.CanDestroyPawn = true;
                }
                else if (PowerP2 >= 50)
                {
                    PowerP2 -= 50;
                    board.boardViewModel.CanPawnBecomeQueen = true;
                }
                else if (PowerP2 >= 15)
                {
                    PowerP2 -= 15;
                    board.boardViewModel.CanPlayTwice = true;
                }
            }
        }

        public MainWindowViewModel(Board board)
        {
            NumberPiecesP1 = defaultPiecesValue;
            NumberPiecesP2 = defaultPiecesValue;
            G = Game.GetInstance();
            this.board = board;
            this.board.boardViewModel.Attach(this);
            BoardViewModel.OnFinish += ShowEndMenu;
            EndMenuViewModel.OnReplay += ResetGame;
        }

        private void ShowEndMenu() {
            this.IsFinished = true;
        }

        /// <summary>
        /// Function used to reset the game when the player clicks replay
        /// </summary>
        private void ResetGame()
        {
            IsFinished = false;
            board.boardViewModel.ResetBoard();
            G.ResetGame();
            PowerP1 = 0;
            PowerP2 = 0;
            NumberPiecesP1 = defaultPiecesValue;
            NumberPiecesP2 = defaultPiecesValue;
            PlayerTurn = "Tour Joueur 1";
        }

        /// <summary>
        /// Function called from boardViewModel, notify if a token eats another so we can update power gauge
        /// </summary>
        public void Update() {
            if (G.CurrentPlayer.Team == 1) {
                if (PowerP1 + 10 <= 100)
                    PowerP1 += 10;
                if (PowerP2 + 5 <= 100)
                    PowerP2 += 5;
                NumberPiecesP2--;
            } else {
                if (PowerP1 + 10 <= 100)
                    PowerP2 += 10;
                if (PowerP2 + 5 <= 100)
                    PowerP1 += 5;
                NumberPiecesP1--;
            }
        }

        /// <summary>
        /// Function called from boardViewModel, notify if turn is changed
        /// </summary>
        public void ChangeTurn() {
            G.Play();
            PlayerTurn = "Tour Joueur " + G.CurrentPlayer.Team;
            PlayerTurnBool = G.CurrentPlayer.Team == 1 ? false : true;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged([CallerMemberName] string name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }
}
