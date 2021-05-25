using System;
using FluentValidation;
using NServiceBus;

namespace Werbekampagne.Messages
{
    public class WerbemedienAktualisiert : IEvent
    {
        public Guid WerbekampagneId { get; set; }
        public bool BeworbenAufPrintmedien { get; set; }
        public bool BeworbenImWebshop { get; set; }
    }

    public class WerbemedienAktualisiertValidator :
        AbstractValidator<WerbemedienAktualisiert>
    {
        public WerbemedienAktualisiertValidator()
        {
            RuleFor(_ => _.WerbekampagneId)
                .NotNull()
                .NotEmpty();
            RuleFor(_ => _.BeworbenAufPrintmedien).NotNull();
            RuleFor(_ => _.BeworbenImWebshop).NotNull();
        }
    }
}