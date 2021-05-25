using System;
using NServiceBus;

namespace Printmedien.Messages
{
    public class StorniereDruckauftrag : ICommand
    {
        public Guid WerbekampagneId { get; set; }
    }
}