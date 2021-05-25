using System;
using NServiceBus;

namespace Werbekampagne.Messages.Statuswechsel
{
    public class Beenden : ICommand
    {
        public Guid WerbekampagneId { get; set; }
    }
}