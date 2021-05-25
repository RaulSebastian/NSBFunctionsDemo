using System;

namespace NSBFunctionsDemo.Werbekampagne.API.Models
{
    [ApiModel]
    public class NeueWerbekampagne
    {
        public string Bezeichnung { get; set; }
        public string Beschreibung { get; set; }
        public DateTime LaufzeitBeginn { get; set; }
        public DateTime LaufzeitEnde { get; set; }
        public bool BeworbenAufPrintmedien { get; set; }
        public bool BeworbenImWebshop { get; set; }
    }
}
