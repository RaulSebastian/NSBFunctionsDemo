using System;
using NServiceBus;

namespace Werbekampagne.Messages.Statuswechsel
{
    public class WerbekampagneFreigegeben : IEvent
    {
        public Guid WerbekampagneId { get; set; }
    }
}