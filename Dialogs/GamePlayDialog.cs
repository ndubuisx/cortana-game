// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using CoreBot.Cards;
using CoreBot.Models;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Schema;

namespace CoreBot.Dialogs
{
    public class GamePlayDialog : ComponentDialog
    {
        public GamePlayDialog(string id = null)
            : base(id ?? nameof(GamePlayDialog))
        {
            AddDialog(new NumberPrompt<int>(nameof(NumberPrompt<int>)));
            AddDialog(new FinalDialog(nameof(FinalDialog)));
            AddDialog(new WaterfallDialog(nameof(WaterfallDialog), new WaterfallStep[]
            {
                AskStepAsync,
                UpdateBoardStepAsync,
            }));

            // The initial child Dialog to run.
            InitialDialogId = nameof(WaterfallDialog);
        }

        private async Task<DialogTurnResult> AskStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            Activity activity;
            //display board
            if (stepContext.Options == null || (bool)stepContext.Options == true)
            {
                if (GameState.Board.getCurrentPlayerName().Equals("Your"))
                {
                    activity = MessageFactory.Text("", $"{GameState.Board.getCurrentPlayerName()} turn", InputHints.ExpectingInput);
                }
                else
                {
                    activity = MessageFactory.Text("", $"{GameState.Board.getCurrentPlayerName()}'s turn", InputHints.ExpectingInput);
                }
                //prompt user to get the place they want to put their piece
                //activity.Attachments = new List<Attachment> { (CreateAdaptiveCardAttachment(board)) };
                //activity.Speak = $"Where would {board.getCurrentPlayerName()} like to place their {board.getCurrentPlayerToken()}";
                //return await stepContext.PromptAsync(nameof(NumberPrompt<int>), new PromptOptions { Prompt = activity }, cancellationToken);
            }
            else
            {
                activity = MessageFactory.Text("", $"That spots already taken, choose again", InputHints.ExpectingInput);
                //return await stepContext.PromptAsync(nameof(NumberPrompt<int>), new PromptOptions { Prompt = activity }, cancellationToken);
            }
            activity.Attachments = new List<Attachment> { (BoardAdaptiveCard.CreateAdaptiveCardAttachment(GameState.Board)) };
            //activity.Speak = $"Where would {board.getCurrentPlayerName()} like to place their {board.getCurrentPlayerToken()}";
            return await stepContext.PromptAsync(nameof(NumberPrompt<int>), new PromptOptions { Prompt = activity }, cancellationToken);
        }

        private async Task<DialogTurnResult> UpdateBoardStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            //process user input, result

            var isValid = GameState.Board.playerMove(int.Parse(stepContext.Result.ToString()) - 1);

            //use static variable to capture if the move is valid, then use the string static variable as input to activity speak above.

            var currentGameState = GameState.Board.getStatus();
            if (currentGameState == Board.GameState.INPROGRESS)
            {
                if (GameState.Board.isVersusCortana())
                {
                    GameState.Board.playerMove(GameState.Board.getBestIndex());
                    currentGameState = GameState.Board.getStatus();
                    if (currentGameState == Board.GameState.INPROGRESS)
                    {
                        return await stepContext.BeginDialogAsync(nameof(WaterfallDialog), null, cancellationToken);
                    }
                }
                //go back to previous dialog
                return await stepContext.BeginDialogAsync(nameof(WaterfallDialog), isValid, cancellationToken);
            }
            else
            {
                //move to final state + winner message
                return await stepContext.BeginDialogAsync(nameof(FinalDialog), null, cancellationToken);
            }
        }
    }
}
