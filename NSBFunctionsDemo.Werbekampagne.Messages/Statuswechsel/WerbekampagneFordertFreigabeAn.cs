using System;
using NServiceBus;

namespace Werbekampagne.Messages.Statuswechsel
{
    public class WerbekampagneFordertFreigabeAn : IEvent
    {
        public Guid WerbekampagneId { get; set; }
    }
}