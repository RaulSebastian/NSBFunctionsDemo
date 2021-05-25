using System;
using NServiceBus;

namespace Printmedien.Messages
{
    public class DruckauftragAngefordert : IEvent
    {
        public Guid Id { get; set; }
        public Guid WerbekampagneId { get; set; }
    }
}