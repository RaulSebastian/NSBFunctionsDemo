using System;
using NServiceBus;

namespace Werbekampagne.Messages.Statuswechsel
{
    public class Freigeben : ICommand
    {
        public Guid WerbekampagneId { get; set; }
    }
}