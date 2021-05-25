using System;
using NServiceBus;

namespace Printmedien.Messages
{
    public class FordereDruckauftragAn : ICommand
    {
        public Guid WerbekampagneId { get; set; }
        public string Titel { get; set; }
        public string Inhalt { get; set; }
        public DateTime? Deadline { get; set; }
    }
}