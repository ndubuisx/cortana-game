// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace CoreBot.Models
{
    public class Board
    {
        public enum CellState
        {
            BLANK,
            X,
            O,
            ERROR,
            WINNERX,
            WINNERO,
            TIE
        }

        public enum GameState
        {
            INPROGRESS,
            TIE,
            WIN
        }

        private bool versusCortana;

        private CellState[] boardState { get; set; }

        private string[] playerNames { get; set; }

        private Dictionary<CellState, string> iconCDN { get; set; }

        private Dictionary<int, string> baseImageCDN { get; set; }

        private int currentPlayer;

        private CellState currentToken;

        private string winnerName;

        public Board()
        {
            boardState = new CellState[9];
            playerNames = new string[] { "Player 1", "Player 2" };
            iconCDN = new Dictionary<CellState, string>
            {
                { CellState.X, "https://i.ibb.co/xhRYQbJ/x-piece.png" },
                { CellState.O, "https://i.ibb.co/PDZDQjY/o-piece.png" },
                { CellState.WINNERX, "https://i.ibb.co/fS3RyCR/x-piece-winner.png" },
                { CellState.WINNERO, "https://i.ibb.co/6r2ZKp2/o-piece-winner.png" },
            };

            baseImageCDN = new Dictionary<int, string>
            {
              {1, "https://i.ibb.co/Ctz7nd5/1-s-piece.png"},
              {2, "https://i.ibb.co/T18XHWP/2-s-piece.png"},
              {3, "https://i.ibb.co/4YQ9Vy3/3-s-piece.png"},
              {4, "https://i.ibb.co/kGRHSmn/4-s-piece.png"},
              {5, "https://i.ibb.co/7gJQLfH/5-s-piece.png"},
              {6, "https://i.ibb.co/jLzGxFg/6-s-piece.png"},
              {7, "https://i.ibb.co/2dgRvFq/7-s-piece.png"},
              {8, "https://i.ibb.co/chqzbGW/8-s-piece.png"},
              {9, "https://i.ibb.co/pZ3JN9K/9-s-piece.png"}
            };
            currentToken = CellState.X;
            currentPlayer = 0;
            versusCortana = false;
            winnerName = "";
        }

        internal SerializationInfo getCDN(CellState cellState)
        {
            throw new NotImplementedException();
        }

        public void setVersusCortana(bool versus)
        {
            versusCortana = versus;
        }
        
        public bool isVersusCortana()
        {
            return versusCortana;
        }
        
        public int getBestIndex()
        {
            Random random = new Random();
            int index = random.Next(0,9);
            if(validIndex(index))
            {
                if(boardState[index] == CellState.BLANK)
                {
                    return index;
                }
            }
            
            return getBestIndex();
            
        }

        public void setPlayerName(string name, int index)
        {
            name = Char.ToUpper(name[0]) + name.Substring(1);
            playerNames[index] = name;
        }

        public string getCurrentPlayerToken()
        {
            return currentToken.ToString();
        }

        private void switchToken()
        {
            if (currentToken == CellState.X)
                currentToken = CellState.O;
            else if (currentToken == CellState.O)
                currentToken = CellState.X;
        }

        private void switchPlayer()
        {
            if (currentPlayer == 0)
                currentPlayer = 1;
            else if (currentPlayer == 1)
                currentPlayer = 0;
        }

        public bool playerMove(int index)
        {
            if (setCellState(index, currentToken))
            {
                switchToken();
                switchPlayer();
                return true;
            }

            return false;
        }

        private bool setCellState(int index, CellState state)
        {
            if (validIndex(index) && boardState[index] == CellState.BLANK)
            {
                boardState[index] = state;
                return true;
            }

            return false;
        }

        public CellState getCellState(int index)
        {
            if (validIndex(index))
            {
                return boardState[index];
            }

            return CellState.ERROR;
        }

        public string getCDN(CellState state, int index)
        {
            if (state == CellState.BLANK)
            {
                return baseImageCDN[index + 1];
            }
            return iconCDN[state];
        }

        private bool validIndex(int index)
        {
            if (index >= 0 && index <= 8)
            {
                return true;
            }

            return false;
        }

        public string getCurrentPlayerName()
        {
            return playerNames[currentPlayer];
        }

        private bool calculateWinner()
        {
            int[,] wins = {
            { 0, 1, 2 },
            { 3, 4, 5 },
            { 6, 7, 8 },
            { 0, 3, 6 },
            { 1, 4, 7 },
            { 2, 5, 8 },
            { 0, 4, 8 },
            { 2, 4, 6 }}; // An array of possible wins

            for (int i = 0; i < wins.Length / 3; i++)
            {
                int firstIndex = wins[i, 0];
                int secondIndex = wins[i, 1];
                int thirdIndex = wins[i, 2];
                if ((boardState[firstIndex] != CellState.BLANK) && (boardState[firstIndex] == boardState[secondIndex]) && (boardState[firstIndex] == boardState[thirdIndex]))
                {
                    //checking if value in squares based on index

                    //Update board state
                    if (currentPlayer == 1)
                    {
                        updateWinnerBoardState(CellState.WINNERX, firstIndex, secondIndex, thirdIndex);
                    }
                    else if (currentPlayer == 0)
                    {
                        updateWinnerBoardState(CellState.WINNERO, firstIndex, secondIndex, thirdIndex);
                    }
                    return true;
                }
            }
            return false;
        }

        private void updateWinnerBoardState(CellState cstate, int first, int second, int third)
        {
            boardState[first] = boardState[second] = boardState[third] = cstate;
            if (currentPlayer == 0)
                winnerName = playerNames[1];
            else
                winnerName = playerNames[0];
        }

        public string getWinnerName()
        {
            switchPlayer();
            return winnerName;
        }

        private bool calculateFullBoard()
        {
            for (int i = 0; i < 9; i++)
            {
                if (boardState[i] == CellState.BLANK)
                {
                    return false;
                }
            }
            return true;
        }

        private bool calculateTie()
        {
            if (!calculateWinner() && calculateFullBoard())
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public GameState getStatus()
        {
            if (calculateWinner())
            {
                return GameState.WIN;
            }
            else if (calculateTie())
            {
                return GameState.TIE;
            }
            else
            {
                return GameState.INPROGRESS;
            }
        }
    }
}
