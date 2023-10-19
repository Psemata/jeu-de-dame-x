using JeuDeDameX.Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;

namespace JeuDeDameX.ViewModel
{
    public class BoardViewModel : ISubject
    {
        private const int HEIGHT = 10;
        private const int WIDTH = 10;
        //List of Tokens
        public ObservableCollection<TokenViewModel> TokenViewModels { get; set; }
        //List of Cells
        public ObservableCollection<Cell> Cells { get; set; }
        //List of object that can be used in ItemsControl
        public CompositeCollection Collection { get; set; }
        //Board of the game
        public Grid Board { get; set; }
        //Selected Token
        TokenViewModel Selected { get; set; }
        //Color of selected token
        SolidColorBrush SelectedColor { get; set; }
        //Indication to know if player can play twice because of power
        public bool CanPlayTwice { get; set; }
        //Indication to know if pawn can become queen because of power
        public bool CanPawnBecomeQueen { get; set; }
        //Indication to know if player can destroy pawn because of power
        public bool CanDestroyPawn { get; set; }
        //Temporary queen for power kept because at the end of turn it turns into a pawn
        TokenViewModel TemporaryQueen { get; set; }
        //Instance of game
        Game G { get; set; }
        //List of observers to notify, in use the only observer is mainWindowViewModel
        private List<IObserver> _observers = new List<IObserver>();
        //Event to indicate that game is finished
        public delegate void HasFinished();
        public static event HasFinished OnFinish;

        public BoardViewModel(Grid board)
        {
            G = Game.GetInstance();
            CanPlayTwice = false;
            CanPawnBecomeQueen = false;
            CanDestroyPawn = false;
            Board = board;
            //Adds the mouse handler for click
            this.Board.MouseDown += new MouseButtonEventHandler(GridMouseDown);
            CreateListTokens();
            CreateListCells();
            Collection = new CompositeCollection
            {
                new CollectionContainer() { Collection = Cells },
                new CollectionContainer() { Collection = TokenViewModels }
            };
        }
        /// <summary>
        /// Function to reset tokens, called when player wants to replay
        /// </summary>
        public void ResetBoard()
        {
            Collection.RemoveAt(1);
            CreateListTokens();
            Collection.Add(new CollectionContainer() { Collection = TokenViewModels });
        }

        /// <summary>
        /// Function used to get the position of the cell where the user clicks
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void GridMouseDown(object sender, MouseButtonEventArgs e)
        {
            // If the sender, the grid, exixts
            if (sender != null)
            {
                // Get the position of the mouse
                var mousePosition = Mouse.GetPosition(this.Board);

                // Final cell position
                int row = 0;
                int col = 0;
                // Calculation value to help get the position
                double accumulatedHeight = 0.0;
                double accumulatedWidth = 0.0;
                // Get the definitions of the row and columns of the grid
                var rowDefinitions = this.Board.RowDefinitions;
                var columnDefinitions = this.Board.ColumnDefinitions;
                // Process all the rows - Add the actual height of the row and check if your mouse is over it or not
                foreach (var rowDefinition in rowDefinitions)
                {
                    accumulatedHeight += rowDefinition.ActualHeight;
                    if (accumulatedHeight >= mousePosition.Y)
                        break;
                    row++;
                }
                // Process all the columns - Add the actual height of the column and check if your mouse is over it or not
                foreach (var columnDefinition in columnDefinitions)
                {
                    accumulatedWidth += columnDefinition.ActualWidth;
                    if (accumulatedWidth >= mousePosition.X)
                        break;
                    col++;
                }
                bool isToken = false;
                foreach (TokenViewModel t in TokenViewModels)
                {
                    if (t.X == col && t.Y == row)
                    {
                        //If the token is ours
                        if (Game.GetInstance().IsTokenPlayable(t.Token))
                        {
                            //If the power to make a pawn become queen is not active
                            if (!CanPawnBecomeQueen)
                            {
                                //If we already had a token selected we reset its color
                                if (Selected != null)
                                {
                                    Selected.Color = SelectedColor;
                                }
                                Selected = t;
                                SelectedColor = new SolidColorBrush(t.Color.Color);
                                t.Color.Color = (Color)ColorConverter.ConvertFromString("#EBCB8B");
                            }
                            //If it is active
                            else
                            {
                                t.ChangeToQueen();
                                TemporaryQueen = t;
                                CanPawnBecomeQueen = false;
                            }
                        }
                        //If the token is not ours
                        else
                        {
                            //The only case here is if player wants to destroy with power
                            if (CanDestroyPawn)
                            {
                                Collection.Remove(t);
                                TokenViewModels.Remove(t);
                                CanDestroyPawn = false;
                            }
                        }
                        isToken = true;
                        break;
                    }
                }
                //If a token is selected and we did not click on a token this time
                if (!isToken && Selected != null)
                {
                    //Check if there is tokens aligned between the selected token and our click
                    List<TokenViewModel> alignedTokens = IsEnemyTokenAligned(col, row);
                    //If alignedTokens is null it's because the click is not aligned with the token
                    if (alignedTokens == null)
                    {
                        //In this case we just unselect the current token
                        Selected.Color = SelectedColor;
                        Selected = null;
                        return;
                    }
                    //If there is a single token aligned
                    if (alignedTokens.Count == 1)
                    {
                        //We check if token can normally go on the position of the aligned token
                        if (Selected.Token.IsMoveLegal(alignedTokens[0].X, alignedTokens[0].Y, true))
                        {
                            //If so then we remove the aligned token and move where we clicked
                            Collection.Remove(alignedTokens[0]);
                            TokenViewModels.Remove(alignedTokens[0]);
                            Selected.Move(col, row);
                            Selected.Color = SelectedColor;
                            IsOnLastRow();
                            Notify();
                            Selected = null;
                            if (IsFinished())
                            {
                                OnFinish?.Invoke();
                            }
                        }
                        //If token cannot go there normally, then we asusme that the player wanted to unselect
                        else
                        {
                            Selected.Color = SelectedColor;
                            Selected = null;
                        }
                    }
                    //If there is no aligned token
                    else if (alignedTokens.Count == 0)
                    {
                        //If the click position is legal
                        if (Selected.Token.IsMoveLegal(col, row, false))
                        {
                            //If we cannot play twice then we change turn
                            if (!CanPlayTwice)
                            {
                                NotifyChangeTurn();
                                if (TemporaryQueen != null)
                                {
                                    TemporaryQueen.ChangeToPawn();
                                }
                            }
                            //If we can play twice then we don't change turn and put the indicaiton to false
                            else
                            {
                                CanPlayTwice = false;
                            }
                            Selected.Move(col, row);
                            Selected.Color = SelectedColor;
                            IsOnLastRow();
                            Selected = null;
                        }
                        //If token cannot go there normally, then we asusme that the player wanted to unselect
                        else
                        {
                            Selected.Color = SelectedColor;
                            Selected = null;
                        }
                    }
                }
            }
        }
        /// <summary>
        /// Function used to unselect current token
        /// </summary>
        public void Unselect()
        {
            if(Selected != null)
            {
                Selected.Color = SelectedColor;
                Selected = null;
            }
        }
        /// <summary>
        /// Function used to know if a player has no more tokens
        /// </summary>
        /// <returns>true -> game is finished, false -> game is not finished</returns>
        public bool IsFinished()
        {
            int team = TokenViewModels[0].Token.Team;
            foreach (TokenViewModel t in TokenViewModels)
            {
                if (t.Token.Team != team)
                {
                    return false;
                }
            }
            return true;
        }
        /// <summary>
        /// Function used to know if a pawn is on the last row, if it is then it transforms into a Queen
        /// </summary>
        public void IsOnLastRow()
        {
            if (Selected.Token.GetType() == typeof(Pawn))
            {
                if (Selected.Token.Team == 1 && Selected.Y == 9)
                {
                    Selected.ChangeToQueen();
                }
                else if (Selected.Token.Team == 2 && Selected.Y == 0)
                {
                    Selected.ChangeToQueen();
                }
            }
        }
        /// <summary>
        /// Function used to know if a token is aligned between our click and the selected token
        /// </summary>
        /// <param name="x">Column</param>
        /// <param name="y">Row</param>
        /// <returns>List of tokens that are aligned, can be null if click and token are not aligned</returns>
        public List<TokenViewModel> IsEnemyTokenAligned(int x, int y)
        {
            int differenceX = Math.Abs(x - Selected.X);
            int differenceY = Math.Abs(y - Selected.Y);
            if (differenceX != 0 && differenceY != 0 && differenceX == differenceY)
            {
                List<Tuple<int, int>> cells = new List<Tuple<int, int>>();
                int xUnit = (x - Selected.X) / differenceX;
                int yUnit = (y - Selected.Y) / differenceY;
                Tuple<int, int> cell = new Tuple<int, int>(Selected.X + xUnit, Selected.Y + yUnit);
                while (cell.Item1 != x && cell.Item2 != y)
                {
                    cells.Add(cell);
                    cell = new Tuple<int, int>(cell.Item1 + xUnit, cell.Item2 + yUnit);
                }
                List<TokenViewModel> tokens = new List<TokenViewModel>();
                foreach (TokenViewModel t in TokenViewModels)
                {
                    if (t.Token.Team != Selected.Token.Team)
                    {
                        if (cells.Contains(new Tuple<int, int>(t.X, t.Y)))
                        {
                            tokens.Add(t);
                        }
                    }
                }
                return tokens;
            }
            return null;
        }
        /// <summary>
        /// Function used to create list of tokens with their positions, color and team
        /// </summary>
        public void CreateListTokens()
        {
            TokenViewModels = new ObservableCollection<TokenViewModel>();
            int line = 0;
            int offset;
            //Black Tokens
            for (int i = 0; i < 4; i++)
            {
                if (line % 2 == 0)
                    offset = 1;
                else
                    offset = 0;

                for (int j = (0 + offset); j < WIDTH; j += 2)
                {
                    TokenViewModels.Add(new TokenViewModel(new Pawn(j, i, new SolidColorBrush((Color)ColorConverter.ConvertFromString("#3B4252")), 1)));
                }
                line++;
            }
            //For the presentation
            //TokenViewModels.Add(new TokenViewModel(new Pawn(3, 4, new SolidColorBrush((Color)ColorConverter.ConvertFromString("#3B4252")), 1)));

            line = 0;
            //For the presentation
            //TokenViewModels.Add(new TokenViewModel(new Pawn(5, 6, new SolidColorBrush((Color)ColorConverter.ConvertFromString("#E5E9F0")), 2)));
            //White Tokens
            for (int i = (HEIGHT - 1); i > (HEIGHT - 5); i--)
            {
                if (line % 2 == 0)
                    offset = 0;
                else
                    offset = 1;

                for (int j = (0 + offset); j < WIDTH; j += 2)
                {
                    TokenViewModels.Add(new TokenViewModel(new Pawn(j, i, new SolidColorBrush((Color)ColorConverter.ConvertFromString("#E5E9F0")), 2)));
                }
                line++;
            }
        }
        /// <summary>
        /// Function used to create the list of cells
        /// </summary>
        private void CreateListCells()
        {
            Cells = new ObservableCollection<Cell>();
            for (int i = 0; i < HEIGHT; i++)
            {
                for (int j = 0; j < WIDTH; j++)
                {
                    //var value = ((i + j) % 2 == 0) ? new SolidColorBrush(Colors.White) : new SolidColorBrush(Colors.DarkGray);
                    var value = ((i + j) % 2 == 0) ? new SolidColorBrush((Color)ColorConverter.ConvertFromString("#E9D7B1")) : new SolidColorBrush((Color)ColorConverter.ConvertFromString("#8E754E"));
                    Cell r = new Cell(value, i, j);
                    Cells.Add(r);
                }
            }
        }
        /// <summary>
        /// Function used for the observer pattern
        /// </summary>
        /// <param name="observer">The observer that wants to get notified</param>
        public void Attach(IObserver observer)
        {
            this._observers.Add(observer);
        }
        /// <summary>
        /// Function used for the observer pattern
        /// </summary>
        /// <param name="observer">The observer that wants to not get notified anymore</param>
        public void Detach(IObserver observer)
        {
            this._observers.Remove(observer);
        }
        /// <summary>
        /// Function used to notify all observers, used when a token eats another
        /// </summary>
        public void Notify()
        {
            foreach (var observer in _observers)
            {
                observer.Update();
            }
        }
        /// <summary>
        /// Function used to notify observers that the turn is changing
        /// </summary>
        public void NotifyChangeTurn()
        {
            foreach (var observer in _observers)
            {
                observer.ChangeTurn();
            }
        }
    }
}
