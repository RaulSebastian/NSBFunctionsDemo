using System;
using NServiceBus;

namespace Werbekampagne.Messages.Statuswechsel
{
    public class WerbekampagneAbgebrochen : IEvent
    {
        public Guid WerbekampagneId { get; set; }
    }
}