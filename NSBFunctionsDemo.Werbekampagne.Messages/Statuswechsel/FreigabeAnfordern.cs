using System;
using NServiceBus;

namespace Werbekampagne.Messages.Statuswechsel
{
    public class FreigabeAnfordern : ICommand
    {
        public Guid WerbekampagneId { get; set; }
    }
}