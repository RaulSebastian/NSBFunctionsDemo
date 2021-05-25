using System;
using NServiceBus;

namespace WebShop.Messages
{
    public class FordereWerbungAn : ICommand
    {
        public Guid WerbekampagneId { get; set; }
        public string Bezeichnung { get; set; }
        public string Beschreibung { get; set; }
        public DateTime? LaufzeitBeginn { get; set; }
        public DateTime? LaufzeitEnde { get; set; }
    }
}