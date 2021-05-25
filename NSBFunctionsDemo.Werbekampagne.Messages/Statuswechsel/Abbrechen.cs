using System;
using NServiceBus;

namespace Werbekampagne.Messages.Statuswechsel
{
    public class Abbrechen : ICommand
    {
        public Guid WerbekampagneId { get; set; }
    }
}