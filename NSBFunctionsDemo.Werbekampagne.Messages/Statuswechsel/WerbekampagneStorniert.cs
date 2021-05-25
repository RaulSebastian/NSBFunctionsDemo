using System;
using NServiceBus;

namespace Werbekampagne.Messages.Statuswechsel
{
    public class WerbekampagneStorniert : IEvent
    {
        public Guid WerbekampagneId { get; set; }
    }
}