﻿using InteractiveSeven.Core.Events;
using InteractiveSeven.Twitch.Model;
using InteractiveSeven.Twitch.Payments;

namespace InteractiveSeven.Twitch.Commands
{
    public class RainbowCommand : BaseCommand
    {
        private readonly PaymentProcessor _paymentProcessor;

        public RainbowCommand(PaymentProcessor paymentProcessor)
            : base(x => x.RainbowCommandWords, x => x.MenuSettings.EnableRainbowCommand)
        {
            _paymentProcessor = paymentProcessor;
        }

        public override void Execute(in CommandData commandData)
        {
            if (!commandData.User.IsBroadcaster)
            {
                return;
            }

            _paymentProcessor.ProcessPayment(commandData,
                Settings.MenuSettings.RainbowModeCost,
                Settings.MenuSettings.AllowModOverride);

            DomainEvents.Raise(new RainbowModeStarted());
        }
    }
}