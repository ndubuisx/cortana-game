namespace CoreBot.Cards
{
    using AdaptiveCards;
    using CoreBot.Models;
    using Microsoft.Bot.Schema;
    using Newtonsoft.Json;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    /// <summary>
    /// Adaptive Card Model for the Board.
    /// </summary>
    public class BoardAdaptiveCard
    {
        Board board = new Board();

        public static string GetBoardAdaptiveCard(Board board)
        {
            string text = "";
            var adaptiveCard = new AdaptiveCard("1.0")
            {
                Body = new List<AdaptiveElement>()
            };
            
            if (board.getStatus() == Board.GameState.INPROGRESS)
            {
                if (board.getCurrentPlayerName().Equals("Your"))
                {
                    text = board.getCurrentPlayerName() + " Turn [" + board.getCurrentPlayerToken() + "]";
                }
                else
                {
                text = board.getCurrentPlayerName() + "'s Turn [" + board.getCurrentPlayerToken() + "]";
                }
            }
            else if (board.getStatus() == Board.GameState.WIN)
            {
                if (board.getWinnerName().Equals("Your"))
                {
                    text = "You Win";
                }
                else
                {
                    text = board.getWinnerName() + " Wins!";
                }
            }
            else
            {
                text = "No winner";
            }

            var textBlock = new AdaptiveTextBlock()
            {
                Text = text,
                HorizontalAlignment = AdaptiveHorizontalAlignment.Center,
                Size = AdaptiveTextSize.Large,
                Spacing = AdaptiveSpacing.Small
            };

            var horizontalAlignment = new AdaptiveHorizontalAlignment[] { AdaptiveHorizontalAlignment.Right, AdaptiveHorizontalAlignment.Center, AdaptiveHorizontalAlignment.Left };
            var index = 0;
            adaptiveCard.Body.Add(textBlock);
            for (var i = 0; i < 3; i++)
            {
                var columnSet = new AdaptiveColumnSet()
                {
                    Separator = false,
                    Spacing = AdaptiveSpacing.Medium
                };

                for (var j = 0; j < 3; j++)
                {
                    columnSet.Columns.Add(new AdaptiveColumn()
                    {
                        Width = AdaptiveColumnWidth.Stretch,
                        Items = new List<AdaptiveElement>()
                        {
                            new AdaptiveImage()
                            {
                                AltText = "",
                                Url = new Uri(board.getCDN(board.getCellState(index),index++)),
                                Size = AdaptiveImageSize.Medium,
                                HorizontalAlignment = horizontalAlignment[j]
                            }
                        },
                        Spacing = AdaptiveSpacing.Small,
                    });
                }

                adaptiveCard.Body.Add(columnSet);
            }

            return adaptiveCard.ToJson();
        }

        // Load attachment from file.
        public static Attachment CreateAdaptiveCardAttachment(Board boardInput)
        {
            var adaptiveCard = GetBoardAdaptiveCard(boardInput);
            return new Attachment()
            {
                ContentType = "application/vnd.microsoft.card.adaptive",
                Content = JsonConvert.DeserializeObject(adaptiveCard),
            };
        }
    }
}
