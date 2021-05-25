using System;
using Newtonsoft.Json;

namespace NSBFunctionsDemo.Printmedien.Domain.Model
{
    public class Druckauftrag
    {
        [JsonProperty(PropertyName = "id")]
        public Guid Id { get; set; } = Guid.NewGuid();
        public Guid WerbekampagneId { get; set; }
        public string Titel { get; set; }
        public string Inhalt { get; set; }
        public DateTime? Deadline { get; set; }
        public DruckauftragStatus Status { get; set; } = DruckauftragStatus.Angefordert;
    }
}
