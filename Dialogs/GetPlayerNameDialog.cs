// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System.Threading;
using System.Threading.Tasks;
using CoreBot.Models;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Schema;

namespace CoreBot.Dialogs
{
    public class GetPlayerNameDialog : ComponentDialog
    {
        public GetPlayerNameDialog(string id = null)
            : base(id ?? nameof(GetPlayerNameDialog))
        {
            AddDialog(new TextPrompt(nameof(TextPrompt)));
            AddDialog(new GamePlayDialog(nameof(GamePlayDialog)));

            AddDialog(new WaterfallDialog(nameof(WaterfallDialog), new WaterfallStep[]
            {
                GetFirstNameAsync,
                GetSecondNameAsync,
                StartTwoPlayerGameAsync
            }));

            // The initial child Dialog to run.
            InitialDialogId = nameof(WaterfallDialog);
        }

        private async Task<DialogTurnResult> GetFirstNameAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            var a = MessageFactory.Text("What is player 1's name?", "What is player 1's name?", InputHints.ExpectingInput);
            //a.Speak = "What is player 1's name?";

            //Create a new board instance at every introStep
            //board = new Board();
            return await stepContext.PromptAsync(nameof(TextPrompt), new PromptOptions { Prompt = a }, cancellationToken);
        }

        private async Task<DialogTurnResult> GetSecondNameAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            GameState.Board.setPlayerName(stepContext.Result.ToString(), 0);
            var a = MessageFactory.Text("What is player 2's name?", "What is player 2's name?", InputHints.ExpectingInput);
            //a.Speak = "What is player 2's name?";

            //Create a new board instance at every introStep
            //board = new Board();
            return await stepContext.PromptAsync(nameof(TextPrompt), new PromptOptions { Prompt = a }, cancellationToken);
        }

        private async Task<DialogTurnResult> StartTwoPlayerGameAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            GameState.Board.setPlayerName(stepContext.Result.ToString(), 1);

            return await stepContext.BeginDialogAsync(nameof(GamePlayDialog), null, cancellationToken);
        }
    }
}
