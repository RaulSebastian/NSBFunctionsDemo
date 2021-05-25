using System;
using System.Threading.Tasks;
using NServiceBus;
using WebShop.Messages;

namespace NSBFunctionsDemo.WebShop.Functions.Handler
{
    public class FordereWerbungAnHandler : IHandleMessages<FordereWerbungAn>
    {
        public async Task Handle(FordereWerbungAn message, IMessageHandlerContext context)
        {
            await context.Publish(new WerbungAngefordert
            {
                Id = Guid.NewGuid(),
                WerbekampagneId = message.WerbekampagneId
            });
        }
    }
}