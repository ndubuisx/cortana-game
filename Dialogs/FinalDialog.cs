// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System.Threading;
using System.Threading.Tasks;
using CoreBot.Models;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Schema;
using CoreBot.Cards;

namespace CoreBot.Dialogs
{
    public class FinalDialog : ComponentDialog
    {
        public FinalDialog(string id = null)
            : base(id ?? nameof(GamePlayDialog))
        {
            AddDialog(new TextPrompt(nameof(TextPrompt)));
            AddDialog(new WaterfallDialog(nameof(WaterfallDialog), new WaterfallStep[]
            {
                 FinalStateStepAsync,
                 EndGameStepAsync
            }));

            // The initial child Dialog to run.
            InitialDialogId = nameof(WaterfallDialog);
        }

        private async Task<DialogTurnResult> FinalStateStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            var currentGameState = GameState.Board.getStatus();
            if (currentGameState == Board.GameState.TIE)
            {
                var activity = (Activity)MessageFactory.Attachment(BoardAdaptiveCard.CreateAdaptiveCardAttachment(GameState.Board));
                activity.Speak = $"Game Tied. All of you lose";
                GameState.Board = new Board();
                return await stepContext.PromptAsync(nameof(TextPrompt), new PromptOptions { Prompt = activity }, cancellationToken);
            }
            else
            {
                //handle winner
                var activity = (Activity)MessageFactory.Attachment(BoardAdaptiveCard.CreateAdaptiveCardAttachment(GameState.Board));
                string winner = GameState.Board.getWinnerName() + " wins";
                if (winner.Equals("Your wins"))
                {
                    winner = "You win";
                }
                activity.Speak = $"Game Over. " + winner;
                GameState.Board = new Board();
                return await stepContext.PromptAsync(nameof(TextPrompt), new PromptOptions { Prompt = activity }, cancellationToken);
            }
        }

        private async Task<DialogTurnResult> EndGameStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            return await stepContext.EndDialogAsync(null, cancellationToken);
        }
    }
}
