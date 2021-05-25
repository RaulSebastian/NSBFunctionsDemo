using System;
using System.Threading.Tasks;
using NSBFunctionsDemo.Printmedien.Data.Repositories;
using NSBFunctionsDemo.Printmedien.Domain.Model;
using NServiceBus;
using Printmedien.Messages;

namespace NSBFunctionsDemo.Printmedien.Functions.Handler
{
    public class DruckauftragAngefordertHandler : IHandleMessages<DruckauftragAngefordert>
    {
        private readonly IPrintmedienRepository _repository;

        public DruckauftragAngefordertHandler(IPrintmedienRepository repository)
        {
            _repository = repository;
        }
        
        public async Task Handle(DruckauftragAngefordert message, IMessageHandlerContext context)
        {
            var druckauftrag = await _repository.GetForWerbekampagneId(message.WerbekampagneId);
            
            if (druckauftrag == null)
            {
                return;
            }
            
            if (druckauftrag.Deadline < DateTime.Now.AddDays(7))
            {
                druckauftrag.Status = DruckauftragStatus.Abgelehnt;
                await _repository.Update(druckauftrag);
                await context.Publish(new DruckauftragZuKurzfristig
                {
                    Id = druckauftrag.Id,
                    WerbekampagneId = druckauftrag.WerbekampagneId
                });
            }
        }
    }
}