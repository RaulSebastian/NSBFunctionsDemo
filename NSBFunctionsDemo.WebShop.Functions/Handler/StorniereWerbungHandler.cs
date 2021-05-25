using System;
using System.Threading.Tasks;
using NServiceBus;
using WebShop.Messages;

namespace NSBFunctionsDemo.WebShop.Functions.Handler
{
    public class StorniereWerbungHandler : IHandleMessages<StorniereWerbung>
    {
        public async Task Handle(StorniereWerbung message, IMessageHandlerContext context)
        {
            await context.Publish(new WerbungStorniert
            {
                Id = Guid.NewGuid(),
                WerbekampagneId = message.WerbekampagneId
            });
        }
    }
}