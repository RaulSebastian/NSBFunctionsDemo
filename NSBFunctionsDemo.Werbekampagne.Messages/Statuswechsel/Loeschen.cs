using System;
using NServiceBus;

namespace Werbekampagne.Messages.Statuswechsel
{
    public class Loeschen : ICommand
    {
        public Guid WerbekampagneId { get; set; }
    }
}