using System;
using NServiceBus;

namespace Werbekampagne.Messages.Statuswechsel
{
    public class WerbekampagneBeendet : IEvent
    {
        public Guid WerbekampagneId { get; set; }
    }
}