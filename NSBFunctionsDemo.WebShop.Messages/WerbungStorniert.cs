using System;
using NServiceBus;

namespace WebShop.Messages
{
    public class WerbungStorniert : IEvent
    {
        public Guid Id { get; set; }
        public Guid WerbekampagneId { get; set; }
    }
}