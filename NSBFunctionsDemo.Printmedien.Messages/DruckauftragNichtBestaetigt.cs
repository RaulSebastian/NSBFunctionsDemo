using System;
using NServiceBus;

namespace Printmedien.Messages
{
    public class DruckauftragNichtBestaetigt : IEvent
    {
        public Guid Id { get; set; }
        public Guid WerbekampagneId { get; set; }
        public string Grund { get; set; }
    }
}