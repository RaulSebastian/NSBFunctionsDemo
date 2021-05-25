using System;
using NServiceBus;

namespace NSBFunctionsDemo.Werbekampagne.API.WerbekampagnenManagement
{
    public class WerbekampagneLifecycleSagaData :
        ContainSagaData
    {
        public Guid WerbekampagneId { get; set; }
        public Domain.Model.Werbekampagne Werbekampagne { get; set; }
    }
}