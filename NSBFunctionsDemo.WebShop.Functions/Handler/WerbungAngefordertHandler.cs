using System;
using System.Threading.Tasks;
using NServiceBus;
using WebShop.Messages;

namespace NSBFunctionsDemo.WebShop.Functions.Handler
{
    public class WerbungAngefordertHandler : IHandleMessages<WerbungAngefordert>
    {
        public async Task Handle(WerbungAngefordert message, IMessageHandlerContext context)
        {
            await context.Publish(new WerbungBestaetigt
            {
                Id = message.Id,
                WerbekampagneId = message.WerbekampagneId
            });
        }
    }
}