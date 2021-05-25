using System;
using NServiceBus;

namespace Printmedien.Messages
{
    public class DruckauftragBestaetigt : IEvent
    {
        public Guid Id { get; set; }
        public Guid WerbekampagneId { get; set; }
    }
}