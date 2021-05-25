using System;
using FluentValidation;
using NServiceBus;

namespace Werbekampagne.Messages
{
    public class WerbekampagneAngelegt : IEvent
    {
        public Guid WerbekampagneId { get; set; }
    }

    public class WerbekampagneAngelegtValidator :
        AbstractValidator<WerbekampagneAngelegt>
    {
        public WerbekampagneAngelegtValidator()
        {
            RuleFor(_ => _.WerbekampagneId)
                .NotNull()
                .NotEmpty();
        }
    }
}
