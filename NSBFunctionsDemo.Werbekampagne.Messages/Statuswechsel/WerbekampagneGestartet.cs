using System;
using NServiceBus;

namespace Werbekampagne.Messages.Statuswechsel
{
    public class WerbekampagneGestartet : IEvent
    {
        public Guid WerbekampagneId { get; set; }
    }
}