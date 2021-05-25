using System;
using NServiceBus;

namespace Werbekampagne.Messages.Statuswechsel
{
    public class Starten : ICommand
    {
        public Guid WerbekampagneId { get; set; }
    }
}