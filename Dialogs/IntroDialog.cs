// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Bot.Schema;
using CoreBot.Models;
using Microsoft.Bot.Builder.Dialogs.Choices;
using System.Collections.Generic;

namespace CoreBot.Dialogs
{
    public sealed class IntroDialog : ComponentDialog
    {
        private readonly IConfiguration Configuration;
        private readonly ILogger Logger;

        public IntroDialog(IConfiguration configuration, ILogger<IntroDialog> logger)
            : base(nameof(IntroDialog))
        {
            Configuration = configuration;
            Logger = logger;

            AddDialog(new ChoicePrompt(nameof(ChoicePrompt)));
            AddDialog(new WaterfallDialog(nameof(WaterfallDialog), new WaterfallStep[]
            {
                GameChoiceAsync,
                LaunchGameChoiceAsync
            }));

            AddDialog(new GamePlayDialog(nameof(GamePlayDialog)));
            AddDialog(new GetPlayerNameDialog(nameof(GetPlayerNameDialog)));
            AddDialog(new FinalDialog(nameof(FinalDialog)));

            // The initial child Dialog to run.
            InitialDialogId = nameof(WaterfallDialog);
        }


        private async Task<DialogTurnResult> GameChoiceAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            var a = MessageFactory.Text("", "Please choose a mode to play", InputHints.ExpectingInput);
            var names = new List<string> { "Two Player", "Play Cortana" };
            return await stepContext.PromptAsync(nameof(ChoicePrompt), new PromptOptions
            {
                Choices = ChoiceFactory.ToChoices(names),
                Prompt = a
                //Choices = ChoiceFactory.ToChoices(names)
            }, cancellationToken);
            //return await stepContext.BeginDialogAsync(nameof(WaterfallDialog), null, cancellationToken);
        }

        private async Task<DialogTurnResult> LaunchGameChoiceAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            var res = (FoundChoice)stepContext.Result;

            if (string.Equals(res.Value, "2 Player", StringComparison.OrdinalIgnoreCase) || string.Equals(res.Value, "Two Player", StringComparison.OrdinalIgnoreCase))
            {
                GameState.Board.setVersusCortana(false);
                return await stepContext.BeginDialogAsync(nameof(GetPlayerNameDialog), null, cancellationToken);
            }
            else
            {
                GameState.Board.setPlayerName("Your", 0);
                GameState.Board.setPlayerName("Cortana", 1);
                GameState.Board.setVersusCortana(true);
                return await stepContext.BeginDialogAsync(nameof(GamePlayDialog), null, cancellationToken);
            }
        }
    }
}
