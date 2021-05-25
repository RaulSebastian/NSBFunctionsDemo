using System;

namespace NSBFunctionsDemo.Werbekampagne.API.Models
{
    [ApiModel]
    public class WerbekampagneRahmendaten
    {
        public string Bezeichnung { get; set; }
        public string Beschreibung { get; set; }
        public DateTime LaufzeitBeginn { get; set; }
        public DateTime LaufzeitEnde { get; set; }
    }
}