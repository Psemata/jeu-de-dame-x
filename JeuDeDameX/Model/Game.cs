using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JeuDeDameX.Model
{
    class Game
    {
        public List<Player> Players { get; set; }
        public Player CurrentPlayer { get; set; }
        static Game Instance { get; set; }
        /// <summary>
        /// Private constructor
        /// </summary>
        private Game()
        {
            Players = new List<Player>
            {
                new Player("Player 1", 1),
                new Player("Player 2", 2)
            };
            CurrentPlayer = Players[0];
        }
        /// <summary>
        /// Function used to reset game, only used for the current player but have other uses in the future
        /// </summary>
        public void ResetGame()
        {
            CurrentPlayer = Players[0];
        }
        /// <summary>
        /// Get Instance of the game, singleton pattern
        /// </summary>
        /// <returns>instance of the game</returns>
        public static Game GetInstance()
        {
            if(Instance == null)
            {
                Instance = new Game();
            }
            return Instance;
        }
        /// <summary>
        /// Function used to know if a token is currently playable
        /// </summary>
        /// <param name="token">token played</param>
        /// <returns>true -> token is playable, false -> token is not playable</returns>
        public bool IsTokenPlayable(TokenModel token)
        {
            return token.Team == CurrentPlayer.Team;
        }
        /// <summary>
        /// Function to change the currentPlayer, used when turn changes
        /// </summary>
        public void Play()
        {
            CurrentPlayer = Players[CurrentPlayer.Team % 2];
        }
    }
}
