using System;
using NServiceBus;

namespace Printmedien.Messages
{
    public class DruckauftragStorniert : IEvent
    {
        public Guid Id { get; set; }
        public Guid WerbekampagneId { get; set; }
    }
}