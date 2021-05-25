using System;
using FluentValidation;
using NServiceBus;

namespace Werbekampagne.Messages
{
    public class WerbekampagneGeloescht : IEvent
    {
        public Guid WerbekampagneId { get; set; }
    }

    public class WerbekampagneGeloeschtValidator :
        AbstractValidator<WerbekampagneGeloescht>
    {
        public WerbekampagneGeloeschtValidator()
        {
            RuleFor(_ => _.WerbekampagneId)
                .NotNull()
                .NotEmpty();
        }
    }
}