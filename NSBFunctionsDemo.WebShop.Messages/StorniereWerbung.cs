using System;
using NServiceBus;

namespace WebShop.Messages
{
    public class StorniereWerbung : ICommand
    {
        public Guid WerbekampagneId { get; set; }
    }
}