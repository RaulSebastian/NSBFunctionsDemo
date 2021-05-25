using System;
using NServiceBus;

namespace Werbekampagne.Messages.Statuswechsel
{
    public class Stornieren : ICommand
    {
        public Guid WerbekampagneId { get; set; }
    }
}