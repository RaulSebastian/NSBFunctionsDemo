using System;
using FluentValidation;
using NServiceBus;

namespace Werbekampagne.Messages
{
    public class WerbekampagnenDatenAktualisiert : IEvent
    {
        public Guid WerbekampagneId { get; set; }
        public string Bezeichnung { get; set; }
        public string Beschreibung { get; set; }
        public DateTime LaufzeitBeginn { get; set; }
        public DateTime LaufzeitEnde { get; set; }
    }

    public class WerbekampagnenDatenAktualisiertValidator :
        AbstractValidator<WerbekampagnenDatenAktualisiert>
    {
        public WerbekampagnenDatenAktualisiertValidator()
        {
            RuleFor(_ => _.WerbekampagneId)
                .NotNull()
                .NotEmpty();
        }
    }
}